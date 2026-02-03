using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;
using static UnityEditor.Progress;

// ShopManager 스크립트
// 기본적으로 Weapon과 Item을 가지고 있다.
public class ShopManager : MonoBehaviour
{
    [Header("상품 슬롯")]
    [SerializeField] private List<ProductSlot> productSlots = new List<ProductSlot>();

    [Header("리롤 버튼")]
    [SerializeField] private TextMeshProUGUI rerollPriceTxt;

    [Header("Shop UI")]
    [SerializeField] private Button shopNPC;
    [SerializeField] private GameObject shopWindow;

    [Header("판매 시 구매 가격의 비율")]
    [SerializeField] private float saleRate = 0.25f;

    [Header("Lock/UnLock 이미지 리소스")]
    [SerializeField] private Image productLockImg;
    [SerializeField] private Sprite lockImg;
    [SerializeField] private Sprite unLockImg;

    [Header("Lock 여부")]
    [SerializeField] private bool productLock = false;

    private float rerollPrice = 0;
    private bool initailize = false;

    private DaysManager daysManager;
    private GameRule gameRule;

    private const int MAX_PROUDCT_COUNT = 3;
    private int currentProductCount = 0;

    private bool isPurchasing = false;

    private void Start()
    {
        shopWindow.SetActive(false);
        GameManager.Instance.ShopManager = this;
    }

    public void ResetInitailize()
    {
        initailize = false;
    }

    public void OnClickShopNPC()
    {
        if(shopWindow.activeSelf == false)
        {
            shopWindow.SetActive(true);
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UiPopUp, WhereAreYouLookinAt.Enum.SFXPlayType.Single);

            // 상점 NPC 한번 클릭했을때만 리셋 & 잠금 기능이 켜져 있을 경우 리셋 X
            if (initailize == false)
            {
                if (gameRule == null)
                    gameRule = GameManager.Instance.GameRule;

                if (daysManager == null)
                    daysManager = GameManager.Instance.DayManager;

                // 리롤 비용 정하기
                rerollPrice = gameRule.GetRerollPriceByWave(daysManager.CurrentDay);
                UpdateRerollPrice();
            }

            // 잠금 기능이 안걸려져 있으면 자동 리롤
            if(initailize == false && productLock == false)
            {
                currentProductCount = MAX_PROUDCT_COUNT;

                foreach (var productSlot in productSlots)
                {
                    productSlot.SelectBtn.onClick.RemoveAllListeners();
                    productSlot.SelectBtn.onClick.AddListener(() => SelectProduct(productSlot));
                    productSlot.SetProduct(GetRandomProduct());
                    productSlot.CanvasGroup.alpha = 1f;
                }
            }

            // 잠금 기능 다음 웨이브에서는 자동으로 풀리게 설정
            if (productLock) ClickLock();

            // 초기화 설정
            if (initailize == false) initailize = true;
        }
        else
        {
            shopWindow.SetActive(false);
        }
    }

    public void Reroll()
    {
        // 돈이 부족하거나 잠금 상태라면 Return
        if (rerollPrice > GameManager.Instance.Player.Condition.Gold.Value)
        {
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.impossible, WhereAreYouLookinAt.Enum.SFXPlayType.Single);
            return;
        }

        // 아이템 구매 연출 중이면 Return
        if(isPurchasing)
        {
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.impossible, WhereAreYouLookinAt.Enum.SFXPlayType.Single);
            return;
        }

        // 잠금 중이면 자동으로 해제
        if(productLock) ClickLock();

        if (gameRule == null)
            gameRule = GameManager.Instance.GameRule;

        rerollPrice += gameRule.GetRerollIncreasePriceByWave(daysManager.CurrentDay); ;
        UpdateRerollPrice();
        // Player 돈 업데이트
        GameManager.Instance.Player.Condition.Sub(AttributeType.Gold, rerollPrice);

        currentProductCount = MAX_PROUDCT_COUNT;

        foreach (var productSlot in productSlots)
        {
            productSlot.SetProduct(GetRandomProduct());
            productSlot.CanvasGroup.alpha = 1f;
        }
    }

    public void ClickLock()
    {
        if(productLock)
        {
            // 잠금 풀기
            productLockImg.sprite = unLockImg;
            productLock = false;
        }
        else
        {
            // 잠금
            productLockImg.sprite = lockImg;
            productLock = true;
        }
    }

    private ISlotItem GetRandomProduct()
    {
        GameRule rule = GameManager.Instance.GameRule;

        ItemDataTable itemDataTable = GameManager.Instance.ItemDataTable;
        WeaponDataTable weaponDataTable = GameManager.Instance.WeaponDataTable;

        // 조언자 or 조언인지 설정
        float roll = Random.Range(0f, 100f);
        if (roll < rule.GetWeaponOrItemRate(DropItemType.Item, GameManager.Instance.DayManager.CurrentDay))
        {
            Item newItem = itemDataTable.GetRandomItemByTier();

            return newItem;
        }
        else
        {
            Weapon newWeapon = weaponDataTable.GetRandomWeaponByTier();

            return newWeapon;
        }
    }

    public void CloseWindow()
    {
        // 구매 연출 중이면 Return
        if (isPurchasing) return;

        shopWindow.SetActive(false);
    }

    private void UpdateRerollPrice()
    {
        if (rerollPriceTxt == null) Debug.Log("없음");
        rerollPriceTxt.text = $"-{rerollPrice}";
    }

    public void SelectProduct(ProductSlot product)
    {
        // 상점 구매 할 돈이 충분한지 체크
        Player player = GameManager.Instance.Player;
        if (product.FinalPrice > player.Condition.Gold.Value)
        {
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.impossible, WhereAreYouLookinAt.Enum.SFXPlayType.Single);
            // 팝업창 뜨게하기?
            Debug.Log("골드가 부족합니다");
            return;
        }

        // 골드 소모
        player.Condition.Sub(AttributeType.Gold, product.FinalPrice);

        // 판매 가격 설정
        product.Item.SetPrice(product.FinalPrice);

        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold, WhereAreYouLookinAt.Enum.SFXPlayType.Single);

        var effect = product.GetComponent<ProductSlotEffect>();

        // 연출 변수 true
        isPurchasing = true;

        effect.Play(() =>
        {
            // 임시 인벤토리에 추가
            GameManager.Instance.TemporaryInventory.AddItemFromShopOrReward(product.Item);

            // 클릭한 지우기
            product.CanvasGroup.alpha = 0f;

            currentProductCount--;

            // 슬롯에 있는 아이템 다 사면 자동 리롤
            if(currentProductCount <= 0)
            {
                currentProductCount = MAX_PROUDCT_COUNT;

                foreach (var productSlot in productSlots)
                {
                    productSlot.SelectBtn.onClick.RemoveAllListeners();
                    productSlot.SelectBtn.onClick.AddListener(() => SelectProduct(productSlot));
                    productSlot.SetProduct(GetRandomProduct());
                    productSlot.CanvasGroup.alpha = 1f;
                }
            }

            // 연출 변수 false
            isPurchasing = false;
        });
    }
}
