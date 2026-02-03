using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using WhereAreYouLookinAt.Enum;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Label Texts")]
    [SerializeField] private TextMeshProUGUI maxHpLabel;
    [SerializeField] private TextMeshProUGUI commandLabel;
    [SerializeField] private TextMeshProUGUI moveSpeedLabel;
    [SerializeField] private TextMeshProUGUI evasionLabel;
    [SerializeField] private TextMeshProUGUI defenceLabel;
    [SerializeField] private TextMeshProUGUI luckLabel;

    [Header("Status Texts")]
    [SerializeField] private TextMeshProUGUI maxHpText;
    [SerializeField] private TextMeshProUGUI commandText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI evasionText;
    [SerializeField] private TextMeshProUGUI defenceText;
    [SerializeField] private TextMeshProUGUI luckText;

    private Coroutine changeLocaleCoroutine;

    private void Reset()
    {
        maxHpLabel = transform.FindChild<TextMeshProUGUI>("MaxHp Label");
        commandLabel = transform.FindChild<TextMeshProUGUI>("Command Label");
        moveSpeedLabel = transform.FindChild<TextMeshProUGUI>("MoveSpeed Label");
        evasionLabel = transform.FindChild<TextMeshProUGUI>("Evasion Label");
        defenceLabel = transform.FindChild<TextMeshProUGUI>("Defence Label");
        luckLabel = transform.FindChild<TextMeshProUGUI>("Luck Label");

        maxHpText = transform.FindChild<TextMeshProUGUI>("MaxHp Text");
        commandText = transform.FindChild<TextMeshProUGUI>("Command Text");
        moveSpeedText = transform.FindChild<TextMeshProUGUI>("MoveSpeed Text");
        evasionText = transform.FindChild<TextMeshProUGUI>("Evasion Text");
        defenceText = transform.FindChild<TextMeshProUGUI>("Defence Text");
        luckText = transform.FindChild<TextMeshProUGUI>("Luck Text");
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnChangeLabelLocale;

        EventBus.Subscribe(AttributeType.MaxHp, UpdateUI);
        EventBus.Subscribe(AttributeType.Command, UpdateUI);
        EventBus.Subscribe(AttributeType.MoveSpeed, UpdateUI);
        EventBus.Subscribe(AttributeType.Evasion, UpdateUI);
        EventBus.Subscribe(AttributeType.Defense, UpdateUI);
        EventBus.Subscribe(AttributeType.Luck, UpdateUI);

        if(GameManager.Instance != null)
        {
            if(GameManager.Instance.Player != null) UpdateUI(0);
        }
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLabelLocale;

        EventBus.UnSubscribe(AttributeType.MaxHp, UpdateUI);
        EventBus.UnSubscribe(AttributeType.Command, UpdateUI);
        EventBus.UnSubscribe(AttributeType.MoveSpeed, UpdateUI);
        EventBus.UnSubscribe(AttributeType.Evasion, UpdateUI);
        EventBus.UnSubscribe(AttributeType.Defense, UpdateUI);
        EventBus.UnSubscribe(AttributeType.Luck, UpdateUI);
    }

    public void UpdateUI(float value)
    {
        Player player = GameManager.Instance.Player;

        maxHpText.text = ((int)player.Condition.MaxHp.Value).ToString();
        commandText.text = ((int)player.Stat.Command.Value).ToString();
        moveSpeedText.text = ((int)player.Stat.MoveSpeed.Value).ToString();
        evasionText.text = $"{(int)player.Stat.Evasion.Value}%";
        defenceText.text = $"{(int)player.Stat.Defense.Value}%";
        luckText.text = $"{(int)player.Stat.Luck.Value}%";
    }

    private void OnChangeLabelLocale(Locale locale)
    {
        if (changeLocaleCoroutine != null)
            StopCoroutine(changeLocaleCoroutine);

        changeLocaleCoroutine = StartCoroutine(ChangeLabelLocaleRoutine(locale));
    }

    private IEnumerator ChangeLabelLocaleRoutine(Locale locale)
    {
        // label locale
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("StatusTable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry("Status_MaxHp").GetLocalizedString();
            maxHpLabel.text = text;

            text = table.GetEntry("Status_Command").GetLocalizedString();
            commandLabel.text = text;

            text = table.GetEntry("Status_MoveSpeed").GetLocalizedString();
            moveSpeedLabel.text = text;

            text = table.GetEntry("Status_EvasionRate").GetLocalizedString();
            evasionLabel.text = text;

            text = table.GetEntry("Status_DefenceRate").GetLocalizedString();
            defenceLabel.text = text;

            text = table.GetEntry("Status_Luck").GetLocalizedString();
            luckLabel.text = text;
        }
    }
}
