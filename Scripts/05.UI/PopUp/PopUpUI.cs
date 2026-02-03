using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{
    static public PopUpUI instance;

    [SerializeField] private GameObject window;
    [SerializeField] private TextMeshProUGUI popUpTitle;
    [SerializeField] private TextMeshProUGUI popUpDesc;
    [SerializeField] private Button activeBtn;
    [SerializeField] private Button closeBtn;

    private void Reset()
    {
        window = GameObject.Find("PopUpWindow");
        popUpTitle = transform.FindChild<TextMeshProUGUI>("PopUpTitle");
        popUpDesc = transform.FindChild<TextMeshProUGUI>("PopUpDesc");
        activeBtn = transform.FindChild<Button>("Button");
        closeBtn = transform.FindChild<Button>("Close");
    }

    private void Awake()
    {
        instance = this;
    }

    public void SetUI()
    {

    }

}
