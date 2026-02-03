using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterSlot : MonoBehaviour
{
    private PlayerTraitData data;

    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectedCursor;

    public PlayerTraitData Data => data;

    public void SetData(PlayerTraitData traitData)
    {
        data = traitData;

        iconImage.sprite = DataManager.GetImage(data.Image);
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectedCursor != null)
            selectedCursor.gameObject.SetActive(selected);
    }
}
