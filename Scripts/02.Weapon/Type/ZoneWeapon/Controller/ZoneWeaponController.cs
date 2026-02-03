using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ZoneWeaponController : WeaponBaseController
{
    [Header("적용 중인 Zone 효과")]
    [SerializeField] List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>();
    private HashSet<IZoneEffect> effects = new HashSet<IZoneEffect>();
    private HashSet<GameObject> targets = new HashSet<GameObject>();

    [Header("Zone Sprite")]
    [SerializeField] private Transform zoneTransform;

    [SerializeField] private float zoneOffset = 1f;

    protected override void Reset()
    {
        base.Reset();

        // 인스펙터 확인용 / 안정화 이후 삭제
        foreach (var c in GetComponents<MonoBehaviour>())
        {
            if (c is IZoneEffect effect)
            {
                monoBehaviours.Add(c);
            }
        }
    }

    protected virtual void OnEnable()
    {
        EventBus.Subscribe(AttributeType.Range, UpdateZoneSize);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        EventBus.UnSubscribe(AttributeType.Range, UpdateZoneSize);
    }

    protected override void Start()
    {
        base.Start();
        InitEffect();
        UpdateZoneSize(stat.Range.Value);
    }

    protected override void Update()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        base.Update();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stat.Range.Value - zoneOffset, target);

        var current = new HashSet<GameObject>();
        bool isGetExp = false;

        foreach (Collider2D hit in hits)
        {
            GameObject go = hit.gameObject;
            current.Add(go);

            if (!targets.Contains(go))
                foreach (var e in effects)
                    e.OnEnter(go);

            foreach (var e in effects)
            {
                bool success = e.OnStay(go, Time.deltaTime);

                if (!isGetExp && success)
                {
                    isGetExp = true;
                    weapon.Condition.Add(AttributeType.Exp, 10);
                }
            }
        }

        foreach (var prev in targets)
            if (!current.Contains(prev))
                foreach (var e in effects)
                    e.OnExit(prev);

        targets = current;
    }

    public void InitEffect()
    {
        foreach (var c in GetComponents<MonoBehaviour>())
        {
            if (c is IZoneEffect effect)
            {
                effect.Initialize(weapon);
                effects.Add(effect);
            }
        }
    }

    protected void UpdateZoneSize(float size)
    {
        if (zoneTransform != null)
        {
            zoneTransform.localScale = new Vector3(stat.Range.Value, stat.Range.Value, stat.Range.Value);
        }
    }

    protected override BehaviorTree CreateBehaviorTree()
    {
        return null;
    }
}
