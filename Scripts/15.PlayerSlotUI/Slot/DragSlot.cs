using UnityEngine;
using UnityEngine.UI;

// DragSlot 
public class DragSlot : MonoBehaviour
{
    static public DragSlot instance;

    public Image itemImage;
    public Slot dragSlot;
    public Image aura;

    private RectTransform rectTransform;

    void Awake()
    {
        instance = this;
        itemImage = GetComponent<Image>();
        HideDragItem();
        rectTransform = GetComponent<RectTransform>();
        aura = transform.GetChild(0).GetComponent<Image>();
    }

    public void SetDragItem(Sprite sprite, RectTransform rectTransform, Image aura)
    {
        // 아이템 이미지 설정
        itemImage.sprite = sprite;
        itemImage.color = new Color(1, 1, 1, 1);

        // 아우라 설정
        if(aura != null )
        {
            this.aura.sprite = aura.sprite;
            aura.color = new Color(1, 1, 1, 1);
        }

        // 크기 설정
        Vector2 size = rectTransform.sizeDelta;
        this.rectTransform.sizeDelta = size;
        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UiButtonClick, WhereAreYouLookinAt.Enum.SFXPlayType.Multi);
    }

    public void HideDragItem()
    {
        // 드래그 종료 시 숨기기
        itemImage.color = new Color(1, 1, 1, 0);
        aura.color = new Color(1, 1, 1, 0);
        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UiButtonClick, WhereAreYouLookinAt.Enum.SFXPlayType.Multi);
    }
}
