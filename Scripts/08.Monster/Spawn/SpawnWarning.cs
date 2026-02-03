using System.Collections;
using UnityEngine;

public class SpawnWarning : MonoBehaviour
{
    private int monsterId;
    private bool isAir;
    private Vector3 spawnPosition;
    private SpawnManager spawnManager;

    private bool isCanceled = false;
    private bool isDisposed = false;

    private int dayMin;
    private float damageAmount;
    private float dayDamageIncreaseAmount;
    private float hpAmount;
    private float dayHpIncreaseAmount;
    private float monsterGainGold;
    private float dayGoldIncreaseAmount;

    [Header("경고 연출")]
    [SerializeField] private SpriteRenderer warningIcon;
    [SerializeField] private float blinkInterval = 0.2f;
    [SerializeField] private int blinkCount = 2;

    [SerializeField] private MonsterPoolKey poolkey;
    public MonsterPoolKey PoolKey => poolkey;

    private Coroutine warningRoutine;

    public void Initialize(
        int monsterId,
        Vector3 spawnPosition,
        SpawnManager spawnManager,
        int dayMin,
        float damageAmount,
        float dayDamageIncreaseAmount,
        float hpAmount,
        float dayHpIncreaseAmount,
        float monsterGainGold,
        float dayGoldIncreaseAmount
    )
    {
        this.monsterId = monsterId;
        this.spawnPosition = spawnPosition;
        this.spawnManager = spawnManager;
        this.dayMin = dayMin;
        this.damageAmount = damageAmount;
        this.dayDamageIncreaseAmount = dayDamageIncreaseAmount;
        this.hpAmount = hpAmount;
        this.dayHpIncreaseAmount = dayHpIncreaseAmount;
        this.monsterGainGold = monsterGainGold;
        this.dayGoldIncreaseAmount = dayGoldIncreaseAmount;

        transform.position = spawnPosition;

        if (warningRoutine != null)
            StopCoroutine(warningRoutine);

        warningRoutine = StartCoroutine(WarningCoroutine());
    }

    private IEnumerator WarningCoroutine()
    {
        // 경고 점멸
        for (int i = 0; i < blinkCount; i++)
        {
            if (isCanceled)
            {
                Dispose();
                yield break;
            }

            warningIcon.enabled = true;
            yield return new WaitForSeconds(blinkInterval);

            warningIcon.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
        }

        // 취소 / 정리 중이면 스폰 안 함
        if (isCanceled || spawnManager == null)
        {
            Dispose();
            yield break;
        }

        // 몬스터 스폰 (딱 한 번)
        spawnManager.SpawnMonsterReal(
            monsterId,
            isAir,
            spawnPosition,
            dayMin,
            damageAmount,
            dayDamageIncreaseAmount,
            hpAmount,
            dayHpIncreaseAmount,
            monsterGainGold,
            dayGoldIncreaseAmount
        );

        Dispose();
    }

    public void Cancel()
    {
        if (isCanceled)
            return;

        isCanceled = true;

        if (warningRoutine != null)
            StopCoroutine(warningRoutine);

        Dispose();
    }

    private void Dispose()
    {
        if (isDisposed)
            return;

        warningRoutine = null;

        isDisposed = true;

        if (spawnManager != null)
            spawnManager.UnregisterWarning(this);

        PoolManager.Instance.ReleaseObject(poolkey.PoolKey, gameObject);
    }
}
