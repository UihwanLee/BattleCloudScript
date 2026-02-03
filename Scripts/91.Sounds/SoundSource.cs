using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class SoundSource : MonoBehaviour
{
    private AudioSource audioSource;
    private string key;
    private SFXType type;

    public void Play(AudioClip clip, float soundEffectVolume, string _key, SFXType soundType)
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        CancelInvoke();

        type = soundType;

        if(soundType == SFXType.MonsterHit)
        {
            AudioManager.Instance.SFXSoundCountDict[soundType] += 1;
        }

        audioSource.clip = clip;
        audioSource.volume = soundEffectVolume;
        audioSource.Play();

        key = _key;

        Invoke("Disable", clip.length + 2);
    }

    public void Disable()
    {
        audioSource?.Stop();

        if(type == SFXType.MonsterHit)
        {
            AudioManager.Instance.SFXSoundCountDict[type] -= 1;
        }

        PoolManager.Instance.ReleaseObject(key, this.gameObject);
    }
}
