using System.Threading;
using UnityEngine;

public class MonsterTrait : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private string monsterName;
    [SerializeField] private string desc;
    [SerializeField] private int statId;
    [SerializeField] private int traitId; //입력

    private Monster monster;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        monster = GetComponent<Monster>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // MonsterTraitData 가져오기
        //MonsterTraitData data = DataManager.Get<MonsterTraitData>(Define.MONSTER_BASE_ID);
        MonsterTraitData data =
        DataManager.Get<MonsterTraitData>(traitId);

        if (data == null)
        {
            Debug.LogError($"{name}: MonsterTraitData가 없습니다. traitId={traitId}");
            return;
        }

        SetTrait(data);
    }
    private void OnEnable()
    {
        if (monster == null)
            monster = GetComponent<Monster>();

        if(id != 0)
        {
            MonsterCondition condition = monster.GetComponent<MonsterCondition>();
            MonsterTraitData data = DataManager.Get<MonsterTraitData>(traitId);
            condition.InitMonsterCondition(data.MAX_HP, data.HP_INCREASE_RATE);
        }

        // 풀에서 재사용될 때 기본 스탯으로 원복
        if (statId != 0)
            monster.Stat.LoadStat(statId);
    }

    // Data에 따라서 특성 지정하기 및 Stat 지정
    // Prefab으로 관리하므로 기타 다른 컴포넌트는 추가하지 않아도 된다.
    public void SetTrait(MonsterTraitData data)
    {
        if (data == null) { Debug.Log("MonsterTraitData가 없습니다!"); return; }

        this.id = data.ID;
        this.monsterName = data.NAME;
        this.desc = data.DESC;
        this.statId = data.ID_STAT;

        // 해당 Stat Id Stat 초기화
        monster.Stat.InitStat(statId);

        monster.Controller.Refresh();
        monster.Condition.InitMonsterCondition(data.MAX_HP, data.HP_INCREASE_RATE);
        //OldMonsterController mc = monster.GetComponent<OldMonsterController>();
        //if (mc != null)
        //    mc.RefreshStat();

        //MonsterCondition condition = monster.GetComponent<MonsterCondition>();
        //if (condition != null)
        //    condition.InitMonsterCondition(data.MAX_HP, data.HP_INCREASE_RATE);
    }

    #region 프로퍼티

    public int ID { get { return id; } }
    public string Name { get { return monsterName; } }
    public string Desc { get { return desc; } }
    public int StatID { get { return statId; } }

    #endregion
}
