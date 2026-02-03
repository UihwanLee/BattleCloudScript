using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
//using UnityEngine.AI;
using WhereAreYouLookinAt.Enum;

#region 스폰 설정 데이터

public enum MonsterSpawnType
{
    Ground,
    Air
}

/*
 * SpawnSlot 규칙 (P1)
 * weight = 확률 아님
 * weight = 등장 비중 슬롯
 *
 * 0 = 등장 안 함
 * 1 = 보조
 * 2 = 기본
 * 3 = 주력
 * 4 = 위협
 * 5 = 재앙
 */
[Serializable]
public class SpawnSlot
{
    [Header("몬스터 나오는 최소 날짜")]
    public int dayMin;

    [Header("몬스터 나오는 최소 날짜")]
    public int dayMax;

    [Header("몬스터 DAMAGE")]
    public float damageAmount = 10f;

    [Header("몬스터 DAMAGE 증가량")]
    public float dayDamageIncreseAmount = 0.7f;

    [Header("몬스터 HP")]
    public float hp = 100f;

    [Header("몬스터 HP 증가량")]
    public float dayHpIncreaseAmount = 2f;

    [Header("몬스터 획득 골드량")]
    public float monsterGainGold = 50f;

    [Header("몬스터 골드 증가량")]
    public float dayGoldIncreaseAmount = 0.0f;

    [Header("몬스터 한 텀당 나오는 수")]
    public int spawnCount;

    [Header("몬스터 한정자?")]
    public bool isLimit;

    [Header("몬스터 한정자 텀 리스트")]
    public List<int> limitSpawnTerm = new List<int>();
}

[Serializable]
public class MonsterSpawnPair
{
    [Header("기본 정보")]
    [SerializeField] private int monsterId;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;

    [Header("몬스터 타입")]
    public MonsterType monsterType;

    [Header("스폰 슬롯 테이블")]
    [SerializeField] private List<SpawnSlot> spawnSlots = new();

    public int GetDayMin(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.dayMin;
            }
        }
        return 0;
    }

    public int GetSpawnCount(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.spawnCount;
            }
        }
        return 0;
    }

    public bool IsLimit(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.isLimit;
            }
        }
        return false;
    }

    public List<int> GetLimitTermList(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.limitSpawnTerm;
            }
        }
        return null;
    }

    public float GetDamage(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.damageAmount;
            }
        }
        return 0;
    }

    public float GetDamageMulitplier(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.dayDamageIncreseAmount;
            }
        }
        return 0;
    }

    public float GetHp(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.hp;
            }
        }
        return 0;
    }

    public float GetGold(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.monsterGainGold;
            }
        }
        return 0;
    }

    public float GetGoldMulitplier(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.dayGoldIncreaseAmount;
            }
        }
        return 0;
    }

    public float GetHpMulitplier(int day)
    {
        foreach (var slot in spawnSlots)
        {
            if (day >= slot.dayMin &&
                day <= slot.dayMax)
            {
                return slot.dayHpIncreaseAmount;
            }
        }
        return 0;
    }

    public int MonsterId => monsterId;
    public GameObject Prefab => prefab;
    public int PoolSize => poolSize;
}

[Serializable]
public class DaySpawnRule
{
    public int dayMin;
    public int dayMax;
    public int maxAlive;
    public float spawnInterval;
}

#endregion

public class SpawnManager : MonoBehaviour
{
    #region Inspector

    [Header("SpawnSetting 값")]
    [SerializeField] SpawnSettingSO spawnSettingSO = null;

    [Header("Day별 스폰 규칙")]
    [SerializeField] private List<DaySpawnRule> daySpawnRules = new();

    [Header("현재 스폰 텀 횟수")]
    [SerializeField] private float currentSpawnCount;

    [Header("스폰 영역")]
    [SerializeField] private SpawnArea spawnArea;

    [Header("몬스터 스폰 테이블")]
    [SerializeField] private List<MonsterSpawnPair> monsterPairs = new();

    [Header("스폰 경고")]
    [SerializeField] private SpawnWarning spawnWarningPrefab;
    [SerializeField] private float spawnWarningDelay = 0.4f;

    [SerializeField] private float phase1HpIncreaseMultiplier = 1.0f;
    [SerializeField] private float phase2HpIncreaseMultiplier = 1.5f;
    [SerializeField] private float phase3HpIncreaseMultiplier = 2.5f;

    [Header("이상현상 추가 스탯")]
    [SerializeField] private float additonalDamageStat = 0.0f;

    private GameObject currentBoss;


    #endregion

    #region 내부 상태

    private bool isSpawning;
    private float spawnTimer;

    private bool isClearing = false;

    private readonly List<Monster> activeMonsters = new();
    private readonly List<SpawnWarning> activeWarnings = new();

    // 이상현상 오답 벌칙
    private int spawnPenaltyLevel = 0;

    #endregion

    #region 생명주기
    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.spawnManager = this;
        }
    }
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.spawnManager = this;
        }

        InitializePools();
        InitSpawnSetting();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        if (!isSpawning)
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f)
        {
            return;
        }

        DaySpawnRule rule = GetCurrentDayRule();
        if (rule == null)
            return;

        // 1. 동시 존재 상한 초과 → 초과분 제거
        if (activeMonsters.Count > rule.maxAlive)
        {
            int removeCount = activeMonsters.Count - rule.maxAlive;
            RemoveOldestMonsters(removeCount);
            spawnTimer = rule.spawnInterval;
            return;
        }

        // 몬스터 별 batchCount만큼 소환
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.DayManager != null)
            {
                int day = GameManager.Instance.DayManager.CurrentDay;
                foreach (var pair in monsterPairs)
                {
                    int spawnCount = pair.GetSpawnCount(day);
                    if (spawnCount > 0)
                    {
                        Debug.Log($"[ID {pair.MonsterId}] 몬스터 {currentSpawnCount}번째 텀에 {spawnCount}개 소환 시도");
                        for (int i = 0; i < spawnCount; i++)
                            TrySpawnWithWarningOnce(pair);
                    }
                }
            }
        }

        spawnTimer = rule.spawnInterval;
        currentSpawnCount++;
    }

    #endregion

    #region 외부 제어

    public void ResetCurrentSpawnCount()
    {
        // 전체 SpawnCount 초기화
        currentSpawnCount = 0;
    }

    public void StartSpawn()
    {
        isClearing = false;
        isSpawning = true;
        spawnTimer = 0f;
        if (GameManager.Instance.DayManager.CurrentDay == GameManager.Instance.DayManager.MaxDay ||
            GameManager.Instance.DayManager.CurrentDay == GameManager.Instance.DayManager.MidDay)
        {
            GameObject bossPrefab = Resources.Load<GameObject>("Prefab/Monster/Troll");

            Debug.Log("보스소환");
            currentBoss = Instantiate(bossPrefab, new Vector3(17f, -47f, 0f), Quaternion.identity);
            MonsterVisualEffect visualEffect = currentBoss.GetComponentInChildren<MonsterVisualEffect>();
            visualEffect.ApplyWaveOutLine(GameManager.Instance.PhaseManager.CurrentPhase);
        }
    }

    public void StopSpawn()
    {
        isSpawning = false;
        if (currentBoss != null)
        {
            Destroy(currentBoss);
            currentBoss = null;
        }
    }

    public void AddSpawnPenalty(int value)
    {
        spawnPenaltyLevel += value;
    }

    public void UnregisterWarning(SpawnWarning warning)
    {
        activeWarnings.Remove(warning);
    }

    public void UnregisterMonster(Monster monster)
    {
        if (monster == null)
            return;

        if (!activeMonsters.Contains(monster))
            return;

        activeMonsters.Remove(monster);

        if (isClearing && activeMonsters.Count == 0)
        {
            isClearing = false;
            Debug.Log("[CLEAR DONE] All monsters removed");
        }
    }

    public void ClearAllWarnings()
    {
        for (int i = activeWarnings.Count - 1; i >= 0; i--)
        {
            if (activeWarnings[i] != null)
                activeWarnings[i].Cancel();
        }

        activeWarnings.Clear();
    }

    public void ClearAllMonsters()
    {
        isClearing = true;
        isSpawning = false;

        ClearAllWarnings();

        for (int i = activeMonsters.Count - 1; i >= 0; i--)
        {
            Monster monster = activeMonsters[i];
            if (monster == null || monster.gameObject == null)
                continue;

            monster.Controller.Despawn();
        }
    }

    #endregion

    #region 초기화

    private void InitSpawnSetting()
    {
        if (spawnSettingSO == null) return;

        daySpawnRules = spawnSettingSO.daySpawnRules;
        monsterPairs = spawnSettingSO.monsterSpawnPairs;
    }

    private void InitializePools()
    {
        foreach (var pair in monsterPairs)
        {
            if (pair == null || pair.Prefab == null)
                continue;

            string key = $"Monster_{pair.MonsterId}";

            PoolManager.Instance.CreatePool(
                key,
                pair.Prefab,
                pair.PoolSize,
                transform
            );
        }

        PoolManager.Instance.CreatePool(
            key: spawnWarningPrefab.PoolKey.PoolKey,
            prefab: spawnWarningPrefab.gameObject,
            initialSize: 30,
            parent: transform
            );
    }

    #endregion

    #region 스폰 선택 로직

    private bool TrySpawnWithWarningOnce(MonsterSpawnPair pair)
    {
        if (isClearing)
            return false;

        if (spawnWarningPrefab == null)
            return false;

        if (pair == null)
            return false;

        int day = GameManager.Instance.DayManager.CurrentDay;

        if (pair.IsLimit(day))
        {
            // 몬스터가 한정자라면 현재 스폰 타이밍인지 체크
            List<int> termList = pair.GetLimitTermList(day);
            bool isCanSpawn = false;
            for (int i = 0; i < termList.Count; i++)
            {
                if (termList[i] == currentSpawnCount) isCanSpawn = true;
            }

            if (!isCanSpawn) return false;
        }

        Vector3 pos = GetSpawnPosition();

        GameObject go = PoolManager.Instance.GetObject(spawnWarningPrefab.PoolKey.PoolKey);

        if (!go.TryGetComponent<SpawnWarning>(out var warning))
        {
            go.SetActive(false);
            return false;
        }

        go.transform.position = pos;
        go.transform.rotation = Quaternion.identity;

        // Day별 공격력, 공격력 증가량, 체력, 체력 증가량 모두 설정
        int minDay = pair.GetDayMin(day);
        float damageAmount = pair.GetDamage(day);
        float damageIncreaseAmount = pair.GetDamageMulitplier(day);
        float hpAmount = pair.GetHp(day);
        float hpIncreaseAmount = pair.GetHpMulitplier(day);
        float gold = pair.GetGold(day);
        float goldIncreaseAmount = pair.GetGoldMulitplier(day);

        warning.Initialize(pair.MonsterId, pos, this,
            minDay, damageAmount, damageIncreaseAmount, hpAmount, hpIncreaseAmount, gold, goldIncreaseAmount);

        activeWarnings.Add(warning);
        return true;
    }

    public void SpawnMonsterReal(int monsterId, bool isAir, Vector3 position, int dayMin, float damageAmount, float dayDamageIncreaseAmount, float hpAmount, float dayHpIncreaseAmount,
        float monsterGold, float dayGoldIncreaseAmount)
    {
        // 전투 종료 / 정리 중이면 스폰 금지
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        if (isClearing)
            return;

        string key = $"Monster_{monsterId}";
        GameObject go = PoolManager.Instance.GetObject(key);

        if (!go.TryGetComponent<Monster>(out var monster))
        {
            go.SetActive(false);
        }

        monster.gameObject.transform.position = position;

        activeMonsters.Add(monster);
        monster.Controller.Refresh();
        StartCoroutine(ApplyMonsterStat(
            monster: monster,
            dayMin: dayMin,
            damageAmount: damageAmount,
            dayDamageIncreaseAmount: dayDamageIncreaseAmount,
            hpAmount: hpAmount,
            dayHpIncreaseAmount: dayHpIncreaseAmount,
            monsterGold: monsterGold,
            dayGoldIncreaseAmount: dayGoldIncreaseAmount
            ));

        monster.VisualEffect.ApplyWaveOutLine(GameManager.Instance.PhaseManager.CurrentPhase);
    }

    private IEnumerator ApplyMonsterStat(Monster monster, int dayMin, float damageAmount, float dayDamageIncreaseAmount, float hpAmount, float dayHpIncreaseAmount,
        float monsterGold, float dayGoldIncreaseAmount)
    {
        yield return null;

        //mc.ResetValue();

        int day = (GameManager.Instance.DayManager.CurrentDay - 1 > 0) ? GameManager.Instance.DayManager.CurrentDay - 1 : 0;

        // 누적 순차 공격력 = (Day 시작 - 최소 날짜) * (Damage 증가량)
        float damageMulitplier = (Mathf.Max(0, day - dayMin) * dayDamageIncreaseAmount);

        // 최종 공격력 = 시작 공격력 + 누적 순차 공격력
        float finalDamage = damageAmount + damageMulitplier;

        // 누적 순차 체력 = (Day 시작 - 최소 날짜) * (HP 증가량)
        float hpMulitplier = (Mathf.Max(0, day - dayMin) * dayHpIncreaseAmount);

        // 최종 체력 = 시작 체력 + 누적 순차 체력
        float finalHp = hpAmount + hpMulitplier;

        Debug.Log($"기본공격력 : {damageAmount}, 추가 공격력: {Mathf.Max(0, day - dayMin)} * {dayDamageIncreaseAmount} = {damageMulitplier}");
        Debug.Log($"기본체력 : {hpAmount}, 추가 체력: {Mathf.Max(0, day - dayMin)} * {dayHpIncreaseAmount} = {hpMulitplier}");

        monster.AdditionalStatByDay(finalDamage, finalHp);
        //mc.SetSpawnProtected(false);

        // 누적 순차 골드량 = (Day 시작 - 최소 날짜) * (Gold 증가량)
        float goldMulitplier = (Mathf.Max(0, day - dayMin) * dayGoldIncreaseAmount);

        // 최종 골드 획득량
        float finalGold = monsterGold + goldMulitplier;

        // 골드 적용
        monster.Condition.AdditionalGoldByDay(finalGold);
    }

    #endregion

    #region 보조 로직

    private DaySpawnRule GetCurrentDayRule()
    {
        int day = GameManager.Instance.DayManager.CurrentDay;
        return daySpawnRules.Find(r => day >= r.dayMin && day <= r.dayMax);
    }

    private void RemoveOldestMonsters(int count)
    {
        for (int i = 0; i < count && activeMonsters.Count > 0; i++)
        {
            Monster monster = activeMonsters[0];
            monster.Controller.Despawn();
        }
    }

    private Vector3 GetSpawnPosition()
    {
        return spawnArea.GetSpawnPosition(GameManager.Instance.Player.transform.position);
    }

    #endregion

    #region 이상현상 외부 제어

    public void AddAdditionalDamageStat(float value)
    {
        additonalDamageStat += value;
        if (additonalDamageStat < 0f)
            additonalDamageStat = 0f;
    }

    #endregion
}
