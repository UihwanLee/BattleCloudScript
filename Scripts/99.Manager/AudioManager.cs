using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

[System.Serializable]
public class SoundBGM
{
    public BGMType type; // 곡의 타입.
    public AudioClip clip; // 곡.
}

[System.Serializable]
public class SoundSFX
{
    public SFXType type; // 곡의 타입.
    public AudioClip clip; // 곡.
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
            }
            return instance;
        }
    }

    private AudioManager() { }

    [Header("Setting")]
    [SerializeField][Range(0f, 1f)] private float musicVolume;
    [SerializeField][Range(0f, 1f)] private float soundEffectVolume;

    [Header("SoundSourceList")]
    [SerializeField] private Transform soundSourceParent;
    [SerializeField] private GameObject soundSourcePrefab;
    [SerializeField] private static string soundSourceKey = "SoundSource";

    [Header("SoundEffect")]
    [SerializeField] List<SoundBGM> backGroundList;
    [SerializeField] List<SoundSFX> soundEffectList;

    [Header("Sound Setting UI")]
    [SerializeField] private Slider backGroundSlider;
    [SerializeField] private Slider soundEffectSlider;

    public Dictionary<SFXType, int> SFXSoundCountDict = new Dictionary<SFXType, int>();

    private AudioSource musicAudiosource;
    private AudioSource sfxAudiosource;
    private AudioSource walkAudiosource;

    private int lastMusicStep;
    private int lastSfxStep;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicAudiosource = GetComponent<AudioSource>();
        musicVolume = 0.5f;
        soundEffectVolume = 0.5f;
        musicAudiosource.volume = musicVolume;
        musicAudiosource.loop = true;
        sfxAudiosource = transform.GetChild(0).GetComponent<AudioSource>();
        walkAudiosource = transform.GetChild(1).GetComponent<AudioSource>();

        InitSFXCount();
    }

    private void Start()
    {
        InitSlider();
        GenerateAvailableSoundSource();
        ChangeBackGroundMusic(BGMType.Main);
    }

    #region AudioManager 초기화

    private void InitSFXCount()
    {
        SFXSoundCountDict.Clear();

        foreach (SFXType type in Enum.GetValues(typeof(SFXType)))
            SFXSoundCountDict[type] = 0;
    }

    private void InitSlider()
    {
        if (backGroundSlider != null)
        {
            backGroundSlider.value = musicVolume;
        }

        if (soundEffectSlider != null)
        {
            soundEffectSlider.value = soundEffectVolume;
        }
    }

    private void GenerateAvailableSoundSource()
    {
        // 게임 내에서 사용할 SoundSource 오브젝트 생성
        PoolManager.Instance.CreatePool(soundSourceKey, soundSourcePrefab, 5, soundSourceParent);
    }

    #endregion

    #region Audio Play

    public void ChangeBackGroundMusic(BGMType bgmType)
    {
        AudioClip bgm = FindBGMByType(bgmType);

        if (bgm == null)
        {
            Debug.Log("해당 BGM이 존재하지 않습니다.");
            return;
        }

        // 배경음악 변경
        if (instance.musicAudiosource != null) instance.musicAudiosource.Stop();
        if (bgm != null && instance.musicAudiosource != null)
        {
            instance.musicAudiosource.clip = bgm;
            instance.musicAudiosource.Play();
        }
    }

    public AudioClip FindBGMByType(BGMType soundType)
    {
        for (int i = 0; i < instance.backGroundList.Count; i++)
        {
            if (soundType == instance.backGroundList[i].type) return instance.backGroundList[i].clip;
        }

        return null;
    }

    public void PlayClip(SFXType soundType, SFXPlayType playType = SFXPlayType.Multi)
    {
        // 1. Clip 찾기
        AudioClip clip = FindSFXByType(soundType);
        if (clip == null)
        {
            Debug.Log("해당 SFX가 존재하지 않습니다.");
            return;
        }

        SoundSource availableSoundSource;

        if (playType == SFXPlayType.Multi)
        {
            // 최대 상한 개수 도달하면 return
            if (SFXSoundCountDict[soundType] > Define.MAX_SFX_COUNT) return;

            availableSoundSource = GetAvailableSoundSource();
            // 2. 가용 가능한 SoundSource 찾기
            if (availableSoundSource == null)
            {
                Debug.Log("사용할 수 있는 SoundSource가 없습니다!");
                return;
            }

            // 효과음 재생
            availableSoundSource.Play(clip, instance.soundEffectVolume, soundSourceKey, soundType);
        }
        else
        {
            if (sfxAudiosource != null) sfxAudiosource.Stop();
            if (sfxAudiosource != null)
            {
                sfxAudiosource.volume = soundEffectVolume;
                sfxAudiosource.clip = clip;
                sfxAudiosource.Play();
            }
        }
    }

    public AudioClip FindSFXByType(SFXType soundType)
    {
        for (int i = 0; i < instance.soundEffectList.Count; i++)
        {
            if (soundType == instance.soundEffectList[i].type) return instance.soundEffectList[i].clip;
        }

        return null;
    }

    public static SoundSource GetAvailableSoundSource()
    {
        return PoolManager.Instance.GetObject(soundSourceKey).GetComponent<SoundSource>();
    }

    #endregion

    #region Audio Setting

    public void SetBackGroundVolume(float value)
    {
        musicAudiosource.volume = value;

        int step = Mathf.FloorToInt(value * 10f);

        if (step != lastMusicStep)
        {
            AudioClip clip = FindSFXByType(SFXType.UiOption);
            if (clip != null && sfxAudiosource != null)
            {
                sfxAudiosource.Stop();
                sfxAudiosource.clip = clip;
                sfxAudiosource.volume = soundEffectVolume;
                sfxAudiosource.Play();
            }
            lastMusicStep = step;
        }

    }

    public void SetSoundEffectVolume(float value)
    {
        soundEffectVolume = value;

        int step = Mathf.FloorToInt(value * 10f);

        if (step != lastSfxStep)
        {
            AudioClip clip = FindSFXByType(SFXType.UiOption);
            if (clip != null && sfxAudiosource != null)
            {
                sfxAudiosource.Stop();
                sfxAudiosource.clip = clip;
                sfxAudiosource.volume = soundEffectVolume;
                sfxAudiosource.Play();
            }
            lastSfxStep = step;
        }

    }

    #endregion

    private void OnEnable()
    {
        EventBus.OnPlayerMoveStart += PlayWalkingSound;
        EventBus.OnPlayerMoveStop += StopWalkingSound;
    }

    private void OnDisable()
    {
        EventBus.OnPlayerMoveStart -= PlayWalkingSound;
        EventBus.OnPlayerMoveStop -= StopWalkingSound;
    }


    public void PlayWalkingSound()
    {
        AudioClip clip = FindSFXByType(SFXType.PlayerWalk);
        if (clip == null) return;

        if (walkAudiosource == null)
        {
            walkAudiosource.loop = true;
        }

        if (!walkAudiosource.isPlaying)
        {
            walkAudiosource.clip = clip;
            walkAudiosource.volume = soundEffectVolume;
            walkAudiosource.Play();
        }
    }

    public void StopWalkingSound()
    {
        if (walkAudiosource != null && walkAudiosource.isPlaying)
        {
            walkAudiosource.Stop();
        }
    }

}
