using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class WeaponTrait : MonoBehaviour
{
    [Header("조언자 데이터")]
    [SerializeField] private int id;
    [SerializeField] private string weaponName;
    [SerializeField] private string desc;
    [SerializeField] private Sprite image;
    [SerializeField] private WeaponType type;
    [SerializeField] private ItemApplyTarget applyTarget;

    [SerializeField] private int statId;
    [SerializeField] private float scale;
    [SerializeField] private float price;

    private WeaponCondition condition;
    private WeaponStat weapon;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        weapon = GetComponent<WeaponStat>();
        condition = GetComponent<WeaponCondition>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Data에 따라서 특성 지정하기 및 Stat 지정
    // Prefab으로 관리하므로 기타 다른 컴포넌트는 추가하지 않아도 된다.
    public void SetTrait(WeaponTraitData data, Attribute lv)
    {
        if (data == null) { Debug.Log("WeaponTraitData가 없습니다!"); return; }

        this.id = data.ID;
        this.weaponName = data.NAME;
        this.desc = data.DESC;
        this.statId = data.ID_STAT;
        this.type = DataManager.GetWeaponType(data.TYPE);
        this.scale = data.SCALE;
        this.price = data.PRICE;
        this.applyTarget = DataManager.GetItemApplyTarget(data.ItemApplyTarget);

        // 해당 Stat Id Stat 초기화
        weapon.InitStat(statId);

        // 컨디션 초기화 - 레벨과 경험치는 설정된 값으로 가져와야함
        condition.InitCondition(lv.Value, data.MAX_EXP, data.EXP_COEFFICIENT, type);
    }
    #region 프로퍼티

    public int ID { get { return id; } }
    public string Name { get { return weaponName; } }
    public string Desc { get { return desc; } }
    public int StatID { get { return statId; } }
    public Sprite Image { get { return image; } }
    public float Scale { get { return scale; } }

    #endregion
}
