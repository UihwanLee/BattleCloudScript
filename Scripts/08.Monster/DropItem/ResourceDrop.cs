using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public enum ResourceDropType
{
    Exp,
    Hp,
    Item
}

public class ResourceDrop : MonoBehaviour
{
    [Header("리소스 타입")]
    [SerializeField] private ResourceDropType dropType;

    [Header("EXP 증가량")]
    [SerializeField] private float expValue = 50f;

    [Header("GOLD 증가량")]
    [SerializeField] private float goldValue = 50f;

    [Header("HP 증가량")]
    [SerializeField] private float hpValue = 10f;

    [Header("풀 키")]
    [SerializeField] private string poolKey;

    #region 자석 설정 (P0 최종)

    [Header("자석 범위)")]
    [SerializeField] private float maxAttractRange = 1000f;

    [Header("자석 속도 (거리 기반)")]
    [SerializeField] private float minAttractSpeed = 1f;    // 멀리서
    [SerializeField] private float maxAttractSpeed = 10f;   // 근접 가속

    [Header("흡수 거리")]
    [SerializeField] private float snapDistance = 0.5f;

    [Header("설정")]
    [SerializeField] private bool isAttract = false;

    #endregion

    private Transform player;

    private void OnEnable()
    {
        // 풀링 재사용 대비
        if (player == null && GameManager.Instance != null)
        {
            player = GameManager.Instance.Player?.transform;
            isAttract = false;
        }
    }

    private void Update()
    {
        //// 전투 중에만 작동
        //if (!GameManager.Instance.IsStart)
        //    return;

        if (player == null)
            return;

        if (GameManager.Instance.Player == null)
            return;

        // 플레이어와 드롭 아이템의 거리 계산하여 Attact 키기
        float dist = Vector2.Distance(transform.position, player.position);
        isAttract = (dist < GameManager.Instance.Player.PlayerInteractHandler.AttractRadius);
  

        if (isAttract)
            Attracting();
    }

    public void OnAttract()
    {
        isAttract = true;
    }

    public void OffAttract()
    {
        isAttract = false;
    }

    public void SetGold(float amount)
    {
        goldValue = amount;
    }

    private void Attracting()
    {
        Vector2 dropPos = transform.position;
        Vector2 playerPos = player.position;

        float dist = Vector2.Distance(dropPos, playerPos);

        // 흡수 처리 (시각적 빨려들기)
        if (dist <= snapDistance)
        {
            transform.position = player.position;
            return;
        }

        // 거리 비율 (멀수록 0, 가까울수록 1)
        float t = 1f - Mathf.Clamp01(dist / maxAttractRange);

        // 가속 곡선 (Quadratic easing)
        t = t * t;

        float speed = Mathf.Lerp(minAttractSpeed, maxAttractSpeed, t);

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        GameRule rule = GameManager.Instance.GameRule;
        GameRulePhaseData rulePhase = rule.GetPhaseRule(GameManager.Instance.PhaseManager.CurrentPhase);

        PlayerCondition pc = other.GetComponent<PlayerCondition>();
        if (pc == null) return;

        if (dropType == ResourceDropType.Hp)
        {
            pc.Add(AttributeType.Hp, hpValue);

            FloatingTextPoolManager.Instance.SpawnText(FloatingTextType.Heal.ToString(), hpValue.ToString("0"), player.transform);

            //루트이펙트 호출
            GameManager.Instance.ParticleManager.PlayLootEffect(other.transform);
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootEat);

            PoolManager.Instance.ReleaseObject(poolKey, gameObject);
        }
        else if (dropType == ResourceDropType.Exp)
        {
            float exp = rulePhase.MONSTER_GAIN_EXP;
            float gaugeExp = exp * rulePhase.MONSTER_GAIN_ENERGY_RATE;
            Debug.Log($"[플레이어 경험치 +{exp}획득]");
            pc.Add(AttributeType.Exp, exp);
            EventBus.OnMonsterKilled?.Invoke(gaugeExp);

            // 골드도 얻게 하기
            float fianlGold = goldValue;
            pc.Add(AttributeType.Gold, goldValue);
            Debug.Log($"[플레이어 골드 +{fianlGold}획득]");

            //루트이펙트 호출
            GameManager.Instance.ParticleManager.PlayLootEffect(other.transform);
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold);
            PoolManager.Instance.ReleaseObject(poolKey, gameObject);
        }
        else if(dropType == ResourceDropType.Item)
        {
            if(TryGetComponent<DropItem>(out DropItem item))
            {
                DropItemPool dropItemPool = DropItemPool.Instance;

                // 상호작용 상태라면 Return
                if (item.IsMonsterDrop == false) return;

                // UI 컨테이너에 적용
                EventBus.OnGainItem?.Invoke(item.Item);

                //루트이펙트 호출
                GameManager.Instance.ParticleManager.PlayLootEffect(player.transform);
                AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold);
                // DropItem Release
                dropItemPool.Release(gameObject);
            }
        }
    }

    public void StartMagnet(Vector3 targetWorldPos, float duration, AnimationCurve curve)
    {
        StartCoroutine(MoveToTarget(targetWorldPos, duration, curve));
    }

    private IEnumerator MoveToTarget(Vector3 targetWorldPos, float duration, AnimationCurve curve)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float curveT = curve.Evaluate(t);

            // Transform 대신 미리 계산된 Vector3 위치로 이동
            transform.position = Vector3.Lerp(startPos, targetWorldPos, curveT);
            yield return null;
        }

        // 목적지 도착 후 풀에 반환 (태그나 타입에 따라 분기 처리 가능)
        GameRule rule = GameManager.Instance.GameRule;
        GameRulePhaseData rulePhase = rule.GetPhaseRule(GameManager.Instance.PhaseManager.CurrentPhase);
        Player player = GameManager.Instance.Player;
        if (dropType == ResourceDropType.Exp)
        {
            float exp = rulePhase.MONSTER_GAIN_EXP;
            float gaugeExp = exp * rulePhase.MONSTER_GAIN_ENERGY_RATE;
            Debug.Log($"[플레이어 경험치 +{exp}획득]");
            player.Condition.Add(AttributeType.Exp, exp);
            EventBus.OnMonsterKilled?.Invoke(gaugeExp);

            // 골드도 얻게 하기
            float fianlGold = goldValue;
            player.Condition.Add(AttributeType.Gold, goldValue);
            Debug.Log($"[플레이어 골드 +{fianlGold}획득]");

            //루트이펙트 호출
            GameManager.Instance.ParticleManager.PlayLootEffect(player.transform);
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold);
            PoolManager.Instance.ReleaseObject(poolKey, gameObject);
        }
        else if(dropType == ResourceDropType.Exp)
        {
            player.Condition.Add(AttributeType.Hp, hpValue);

            //루트이펙트 호출
            GameManager.Instance.ParticleManager.PlayLootEffect(player.transform);
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootEat);

            PoolManager.Instance.ReleaseObject(poolKey, gameObject);
        }
        else if (dropType == ResourceDropType.Item)
        {
            if (TryGetComponent<DropItem>(out DropItem item))
            {
                DropItemPool dropItemPool = DropItemPool.Instance;

                // UI 컨테이너에 적용
                EventBus.OnGainItem?.Invoke(item.Item);

                //루트이펙트 호출
                GameManager.Instance.ParticleManager.PlayLootEffect(player.transform);
                AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold);
                // DropItem Release
                dropItemPool.Release(gameObject);
            }
        }
    }
}
