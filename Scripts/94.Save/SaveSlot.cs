using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("세이브 데이터")]
    [SerializeField] private SaveData saveData;

    [Header("슬롯 컴포넌트")]
    [SerializeField] private Image higlight;
    [SerializeField] private Button deleteBtn;

    [Header("슬롯 저장 컴포넌트")]
    [SerializeField] private GameObject saveSlotWindow;     // 세이브 Window
    [SerializeField] private TextMeshProUGUI saveDateTxt;   // 세이브 저장 날짜
    [SerializeField] private TextMeshProUGUI saveDay;       // 세이브 Day
    [SerializeField] private TextMeshProUGUI saveWeapon;    // 세이브 Weapon
    [SerializeField] private TextMeshProUGUI saveItem;      // 세이브 Item
    [SerializeField] private Image savePlayer;              // 세이브 Player
    [SerializeField] private List<Image> savePlayerWeapons; // 세이브 Player Weapons
    [SerializeField] private List<Image> savePlayerItmes;   // 세이브 Player Items;

    private bool isActive = false;
    private bool isOnClick = false;
    private int index;

    private void Reset()
    {
        higlight = transform.FindChild<Image>("Highlight");
        deleteBtn = transform.FindChild<Button>("DeleteBtn");

        saveSlotWindow = GameObject.Find("SlotWindow");
        saveDateTxt = transform.FindChild<TextMeshProUGUI>("SaveDay");
        saveDay = transform.FindChild<TextMeshProUGUI>("DayTxt");
        saveWeapon = transform.FindChild<TextMeshProUGUI>("WeaponTxt");
        saveItem = transform.FindChild<TextMeshProUGUI>("ItemTxt");
        savePlayer = transform.FindChild<Image>("Player");
        foreach (Image img in savePlayer.GetComponentsInChildren<Image>())
        {
            if (img.gameObject != savePlayer.gameObject)
            {
                savePlayerWeapons.Add(img);
            }
        }
        GameObject items = GameObject.Find("Items");
        savePlayerItmes = items.transform.GetComponentsInChildren<Image>().ToList();
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void SetSlot(SaveManager saveManager, SaveData saveData)
    {
        this.saveData = saveData;

        isActive = true;

        // saveWindow 키기
        saveSlotWindow.SetActive(true);

        // SaveTime 갱신
        saveDateTxt.text = saveData.saveTime;

        // GameState 정보 갱신
        saveDay.text = saveData.gameStateData.Day.ToString();

        // Weapon/Item 개수 갱신
        saveWeapon.text = $"({saveData.playerSlotData.CurrentActiveWeaponSlotCount}/{saveData.playerSlotData.ActiveWeaponSlotCount})";
        saveItem.text = $"({saveData.playerSlotData.CurrentActiveItemSlotCount}/{saveData.playerSlotData.ActiveItemSlotCount})";

        // Player Weapon 갱신
        int itemIndex = 0;
        List<WeaponSlotData> weaponSlotDatas = saveData.weaponSlots;
        for(int i=0; i < savePlayerWeapons.Count; i++)
        {
            savePlayerWeapons[i].gameObject.SetActive(false);
            if (weaponSlotDatas[i].Item != null)
            {
                int id = weaponSlotDatas[i].ItemID;
                weaponSlotDatas[i].Item.Load(id);
                savePlayerWeapons[i].sprite = weaponSlotDatas[i].Item.GetIcon();
                savePlayerWeapons[i].gameObject.SetActive(true);
            }

            for(int j=0; j < weaponSlotDatas[i].ItemSlotsData.Count; j++)
            {
                savePlayerItmes[itemIndex].gameObject.SetActive(false);
                if (weaponSlotDatas[i].ItemSlotsData[j].Item != null)
                {
                    int id = weaponSlotDatas[i].ItemSlotsData[j].ItemID;
                    weaponSlotDatas[i].ItemSlotsData[j].Item.Load(id);
                    savePlayerItmes[itemIndex].sprite = weaponSlotDatas[i].ItemSlotsData[j].Item.GetIcon();
                    savePlayerItmes[itemIndex].gameObject.SetActive(true);
                }
                itemIndex++;
            }
        }

        // Player Item 갱신


        // Delete 설정
        deleteBtn.onClick.RemoveAllListeners();
        deleteBtn.onClick.AddListener(()=> saveManager.Delete());
        deleteBtn.gameObject.SetActive(true);
    }

    public void ResetSlot()
    {
        isActive = false;
        deleteBtn.gameObject.SetActive(false);
        saveSlotWindow.SetActive(false);
    }

    public void OnClick()
    {
        isOnClick = true;
        if (isActive) deleteBtn.gameObject.SetActive(true);
    }

    public void OffClick()
    {
        isOnClick = false;
        deleteBtn.gameObject.SetActive(false);
    }

    public void OnHighlight()
    {
        higlight.gameObject.SetActive(true);
    }

    public void OffDeletBtn()
    {
        deleteBtn.gameObject.SetActive(false);
    }

    public void OffHighlight()
    {
        higlight.gameObject.SetActive(false);
    }

    #region 마우스 포인터 인터페이스 구현

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHighlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isOnClick) return;
        OffHighlight();
    }

    #endregion

    #region 프로퍼티
    public int Index {  get { return index; } }

    public SaveData SaveData { get { return saveData; } }
    public bool IsActive { get { return isActive; } }   

    #endregion
}
