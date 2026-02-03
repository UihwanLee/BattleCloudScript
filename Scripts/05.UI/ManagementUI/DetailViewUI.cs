using UnityEngine;

public class DetailViewUI : MonoBehaviour
{
    [Header("Detail Window")]
    [SerializeField] private GameObject detailViewWindow;

    [Header("무기 설명 UI")]
    [SerializeField] private WeaponDescriptionUI descriptionUI;

    [Header("무기 스탯 UI")]
    [SerializeField] private WeaponStatusUI weaponStatusUI;

    private WeaponSlot currentWeaponSlot;

    private void Reset()
    {
        descriptionUI = transform.FindChild<WeaponDescriptionUI>("Name Group");
        weaponStatusUI = transform.FindChild<WeaponStatusUI>("Weapon Stat Group");
    }

    private void Start()
    {
        GameManager.Instance.DetailViewUI = this;
        detailViewWindow.SetActive(false);
    }

    public void SetUI(int index)
    {
        currentWeaponSlot = GameManager.Instance.Player.PlayerSlotUI.WeaponSlots[index];

        // Highlight 키기
        GameManager.Instance.Player.PlayerSlotUI.CloseAllHighlight();
        currentWeaponSlot.AllHighlight();

        detailViewWindow.SetActive(true);
        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UiPopUp, WhereAreYouLookinAt.Enum.SFXPlayType.Single);

        // 선택 커서 위치 조정
        //selectedCursor.UpdatePosition(index);

        // 조언자 상세 정보 UI
        descriptionUI.SetUI(currentWeaponSlot);

        if (currentWeaponSlot.Item != null && currentWeaponSlot.WeaponBase != null)
        {
            weaponStatusUI.SetUI(currentWeaponSlot);
        }
        else
        {
            descriptionUI.SetUIEnabled(false);
            weaponStatusUI.SetUIEnabled(false);
        }
    }

    public void CloseUI()
    {
        // 현재 하이라이트 되고 있는 슬롯 비활성화
        GameManager.Instance.Player.PlayerSlotUI.HighlightAllResetInteractBG();

        detailViewWindow.SetActive(false);
    }

    #region 프로퍼티

    public GameObject DetailViewWindow { get { return detailViewWindow; } }
    public int CurrentViewWeaponIndex { get { return descriptionUI.CurrentWeaponIndex; } }

    #endregion
}
