using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpEffectController : MonoBehaviour
{
    public void Play()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateOnAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
