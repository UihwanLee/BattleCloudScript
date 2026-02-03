using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactTxt;

    private void Reset()
    {
        interactTxt = transform.FindChild<TextMeshProUGUI>("InteractLabel");   
    }

    public void SetPosition(Vector3 position, string interactMessage)
    {
        interactTxt.text = interactMessage;
        transform.position = position;
    }
}
