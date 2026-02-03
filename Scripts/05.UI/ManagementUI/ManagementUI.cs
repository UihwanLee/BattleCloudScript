using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ManagementUI : MonoBehaviour
{
    [Header("Weapon List")]
    [SerializeField] private GameObject weaponListPanel;

    [Header("Views")]
    [SerializeField] private DetailViewUI detailView;

    private ManagementViewType currentView;

    #region 프로퍼티

    public DetailViewUI DetailView => detailView;
    #endregion

    private void OnEnable()
    {
        SetView(ManagementViewType.Detail);
    }

    public void SetView(ManagementViewType viewType)
    {
        currentView = viewType;

        detailView.gameObject.SetActive(viewType == ManagementViewType.Detail);
    }

    public void ShowDetail()
    {
        SetView(ManagementViewType.Detail);
    }

    public void CloseManagement()
    {
        UIManager.Instance.EnterGamePlay();
    }
}
