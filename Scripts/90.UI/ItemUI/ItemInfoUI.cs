using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using WhereAreYouLookinAt.Enum;

// ItemInfoUI 클래스
// Item에 대한 정보를 UI로 표시한다.
// Player가 Canvas를 가지고 있으며 하나의 InfoUI를 통해 관리한다.
// (오브젝트 개별 Canvas -> 하나의 Canvas에서 관리)
public class ItemInfoUI : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private TextMeshProUGUI nameLabel;     // Item 이름 라벨
    [SerializeField] private TextMeshProUGUI descLabel;     // Item 설명 라벨
    [SerializeField] private TextMeshProUGUI nameTxt;       // Item 이름
    [SerializeField] private TextMeshProUGUI descTxt;       // Item 설명
    [SerializeField] private TextMeshProUGUI interactTxt;   // 상호작용 Txt

    [Header("연출 정보")]
    [SerializeField] private AnimationCurve shakingCurve = new AnimationCurve(
                                new Keyframe(0f, 0f),
                                new Keyframe(0.15f, 1f),
                                new Keyframe(0.35f, -0.8f),
                                new Keyframe(0.55f, 0.5f),
                                new Keyframe(0.75f, -0.3f),
                                new Keyframe(1f, 0f)
                            );
    [SerializeField] private float shakingDuration = 0.5f;
    [SerializeField] private float shakingAmplitude = 0.5f;

    private string nameKey;
    private string descKey;
    private string interactKey;
    private List<float> valueList;
    private List<ItemApplyType> typeList;
    private string tableName;

    private Vector3 startPos;
    private Coroutine shakeCoroutine;

    private Coroutine changeInteractLocalCoroutine;
    private Coroutine changeItemLocalCoroutine;

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnChangeLocale;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLocale;
    }

    private void Reset()
    {
        nameLabel = transform.FindChild<TextMeshProUGUI>("ItemNameLabel");
        descLabel = transform.FindChild<TextMeshProUGUI>("ItemDescLabel");
        nameTxt = transform.FindChild<TextMeshProUGUI>("ItemNameTxt");
        descTxt = transform.FindChild<TextMeshProUGUI>("ItemDescTxt");
        interactTxt = transform.FindChild<TextMeshProUGUI>("InteractLabel");
    }

    public void SetUI(string name, string desc, string tableName, string interactMessage, List<float> _valueList, List<ItemApplyType> _typeList, Vector3 position, bool isInteract, Color? color = null)
    {
        nameKey = name;
        descKey = desc;
        this.tableName = tableName;
        transform.position = position;
        interactKey = interactMessage;
        valueList = _valueList;
        typeList = _typeList;
        Color newColor = (color != null) ? (Color)color : Color.white;
        if (newColor != null) interactTxt.color = newColor;
        interactTxt.gameObject.SetActive(isInteract);

        startPos = transform.localPosition;

        Locale currentLocale = LocalizationSettings.SelectedLocale;
        OnChangeLocale(currentLocale);
    }

    public void OnChangeLocale(Locale locale)
    {
        if(changeInteractLocalCoroutine != null) StopCoroutine(changeInteractLocalCoroutine);
        if(changeItemLocalCoroutine != null) StopCoroutine(changeItemLocalCoroutine);

        changeInteractLocalCoroutine = StartCoroutine(ChangeLocaleInteractRoutine(locale));
        changeItemLocalCoroutine =  StartCoroutine(ChangeLocaleItemRoutine(locale, tableName));
    }

    private IEnumerator ChangeLocaleInteractRoutine(Locale locale)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("UITable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(interactKey)?.GetLocalizedString();
            interactTxt.text = text;
        }
    }

    private IEnumerator ChangeLocaleItemRoutine(Locale locale, string Table)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(Table);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(nameKey)?.GetLocalizedString();
            nameTxt.text = text;

            text = table.GetEntry(descKey)
                    ?.GetLocalizedString(new {
                        value_01 = UIHelper.ColorTextWithType(valueList[0], typeList[0], UIHelper.ColorOrange),
                        value_02 = UIHelper.ColorTextWithType(valueList[1], typeList[1], UIHelper.ColorOrange),
                        value_03 = UIHelper.ColorTextWithType(valueList[2], typeList[2], UIHelper.ColorOrange),
                        value_04 = UIHelper.ColorTextWithType(valueList[3], typeList[3], UIHelper.ColorOrange),
                        value_05 = UIHelper.ColorTextWithType(valueList[4], typeList[4], UIHelper.ColorOrange),
                    });
            descTxt.text = text;
        }
    }

    public void Shaking()
    {
        // 아이템 흭득에 실패했을 때 흔들림 효과
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(Shake(shakingDuration, shakingAmplitude));
    }

    private IEnumerator Shake(float duration, float amplitude)
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration; 
            float offset = shakingCurve.Evaluate(t) * amplitude;

            transform.localPosition = startPos + Vector3.right * offset;

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
        shakeCoroutine = null;
    }
}
