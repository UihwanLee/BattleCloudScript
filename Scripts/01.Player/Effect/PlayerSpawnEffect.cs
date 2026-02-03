using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnEffect : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private GameObject spawnEffect;

    private void OnEnable()
    {
        EventBus.SpawnPlayer += StartSpawnEffect;
    }

    private void OnDisable()
    {
        EventBus.SpawnPlayer -= StartSpawnEffect;
    }

    public void StartSpawnEffect()
    {
        spawnEffect.SetActive(true);

        // 몇 초뒤 사라지게 
        Invoke("DisableEffect", 1.5f);
    }

    private void DisableEffect()
    {
        spawnEffect.SetActive(false);

        // 플레이어 움직임 풀기
        GameManager.Instance.Player.Controller.SetMovementEnabled(true);
    }
}
