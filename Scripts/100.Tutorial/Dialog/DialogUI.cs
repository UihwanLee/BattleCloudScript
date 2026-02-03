using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

public class DialogUI : MonoBehaviour
{
    [Header("Dialog UI")]
    [Header("Left Image")]
    [SerializeField] private Image npcImg_L;
    [SerializeField] private TextMeshProUGUI npcName_L;

    [Header("Right Image")]
    [SerializeField] private Image npcImg_R;
    [SerializeField] private TextMeshProUGUI npcName_R;

    [SerializeField] private TextMeshProUGUI dialogMessage;

    [Header("Dialog Manager")]
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private TypingEffect typer;

    [SerializeField] private int optionCount;

    private void Start()
    {
        typer = GetComponent<TypingEffect>();
    }

    public void SetDialogTitle(Sprite sprite, LocalizedString nameKey, DialogPositionType positionType)
    {
        npcImg_L.gameObject.SetActive(false);
        npcName_L.gameObject.SetActive(false);
        npcImg_R.gameObject.SetActive(false);
        npcName_R.gameObject.SetActive(false);
        // UI Setting
        if(positionType == DialogPositionType.LEFT)
        {
            npcImg_L.sprite = sprite;
            npcName_L.text = nameKey.GetLocalizedString();
            npcImg_L.gameObject.SetActive(true);
            npcName_L.gameObject.SetActive(true);
        }
        else if(positionType == DialogPositionType.RIGHT) 
        {
            npcImg_R.sprite = sprite;
            npcName_R.text = name;
            npcImg_R.gameObject.SetActive(true);
            npcName_R.gameObject.SetActive(true);
        }
        else
        {
            return;
        }
    }

    public void StartDialog(DialogData data)
    {
        // UI 세팅
        SetDialogTitle(data.icon, data.npcNameKey, data.positionType);

        // 타이핑 이펙트로 보여주고 행동반환
        typer.Typing(dialogMessage, data.message_EN, onCompleted: () =>
        {
            dialogManager.FinishDialog();
        });
    }
}
