using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public enum DialogState
{
    None,
    Typing,
    TypingDone,
    DialogDone
}

public class DialogManager : MonoBehaviour
{
    [SerializeField] private DialogUI dialogUI;

    private Queue<DialogData> dialogQueue = new Queue<DialogData>();

    private Dialog currentDialog;
    private DialogState currentState;
    private bool isFinishDioalog;

    private void Start()
    {
        currentState = DialogState.None;
        dialogUI.gameObject.SetActive(false);
        isFinishDioalog = false;
    }

    public void SetDialog(Dialog dialog)
    {
        currentDialog = dialog;

        dialogUI.gameObject.SetActive(true);

        dialogQueue.Clear();

        // Dialog Data List Queue에 넣기
        SetDialogQueue(currentDialog.dialogList);
    }

    private void SetDialogQueue(List<DialogData> messageList)
    {
        foreach (DialogData message in messageList)
            dialogQueue.Enqueue(message);
    }

    public bool StartDialogTyping()
    {
        if (dialogQueue.Count <= 0) return true;

        currentState = DialogState.Typing;

        DialogData data = dialogQueue.Dequeue();
        dialogUI.StartDialog(data);

        return false;
    }

    public void FinishDialog()
    {
        // Dialog Typing이 끝나면 TypingDone로 변환
        currentState = DialogState.TypingDone;
    }

    /// <summary>
    /// 대화 중일때 사용자가 Space나 마우스 버튼 누를 시 호출
    /// </summary>
    public void DialogInteract()
    {
        // 현재 대화 타이핑이 끝난 상태라면 상호작용
        if (currentState == DialogState.TypingDone)
        {
            // 이어서 대화 타이핑
            bool isDone = StartDialogTyping();

            // 모든 대화가 끝나면 동작
            if (isDone)
            {
                isFinishDioalog = false;
                dialogUI.gameObject.SetActive(false);
                currentState = DialogState.None;
            }
        }
    }
}
