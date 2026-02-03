using SixLabors.Fonts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class FontManager : MonoBehaviour
{
    public static FontManager Instance { get; private set; }

    [Header("Font Assets")]
    [SerializeField] private TMP_FontAsset fontKR;
    [SerializeField] private TMP_FontAsset fontEN;
    [SerializeField] private TMP_FontAsset fontJP;
    [SerializeField] private TMP_FontAsset fontSP;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyFont(LanguageType type)
    {
        TMP_FontAsset font = type switch
        {
            LanguageType.KR => fontKR,
            LanguageType.EN => fontEN,
            LanguageType.JP => fontJP,
            LanguageType.SP => fontSP,
            _ => fontEN
        };

        ApplyToAllTMP(font);
    }

    public void ApplyToAllTMP(TMP_FontAsset font)
    {
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>(true);

        foreach (var txt in texts)
        {
            txt.font = font;
            txt.SetAllDirty(); 
        }
    }

    public TMP_FontAsset GetFont(LanguageType type)
    {
        return type switch
        {
            LanguageType.KR => fontKR,
            LanguageType.EN => fontEN,
            LanguageType.JP => fontJP,
            LanguageType.SP => fontSP,
            _ => fontEN
        };
    }
}
