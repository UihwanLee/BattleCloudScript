using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class MonsterDropHandler : MonoBehaviour
{
    [Header("리소스 풀 키")]
    [SerializeField] private string expDropKey = "Drop_Exp";
    [SerializeField] private string hpDropKey = "Drop_Hp";
    [SerializeField] private string itemDropKey = "DropItem";

    [Header("확률 설정 (%)")]
    private float expRate = 87f;       // 87
    private float hpRate = 10f;        // 10
    private float itemRate = 3f;       // 3

    public void Drop()
    {
        float roll = Random.Range(0f, 100f);


        GameRule rule = GameManager.Instance.GameRule;
        expRate = rule.GetDropRate(WhereAreYouLookinAt.Enum.DropType.Energy).DROP_RATE;
        hpRate = rule.GetDropRate(WhereAreYouLookinAt.Enum.DropType.Heal).DROP_RATE;

        if (roll < expRate)
        {
            SpawnResource(expDropKey);
        }
        else if (roll < expRate + hpRate)
        {
            SpawnResource(hpDropKey);
        }
        else
        {
            DropItem();
        }
    }

    private void SpawnResource(string poolKey)
    {
        GameObject drop = PoolManager.Instance.GetObject(poolKey);
        if (drop == null) return;

        Monster monster = GetComponent<Monster>();
        ResourceDrop resource = drop.GetComponent<ResourceDrop>();
        if (resource != null)
        {
            resource.SetGold(monster.Condition.Gold);
        }

        drop.transform.position = transform.position;
    }

    private void DropItem()
    {
        // 아이템 시스템 연동 지점

        GameRule rule = GameManager.Instance.GameRule;

        // DropItem 프리팹 가져오기
        DropItemPool dropPool = DropItemPool.Instance;
        DropItem item = dropPool.GetDropPrefab().GetComponent<DropItem>();
        ItemDataTable itemDataTable = GameManager.Instance.ItemDataTable;
        WeaponDataTable weaponDataTable = GameManager.Instance.WeaponDataTable;

        if (item == null) return;

        // 상호작용 못하게 설정
        item.SetMonsterDrop(true);

        // 조언자 or 조언인지 설정
        float roll = Random.Range(0f, 100f);
        if(roll < rule.GetWeaponOrItemRate(WhereAreYouLookinAt.Enum.DropItemType.Item, GameManager.Instance.DayManager.CurrentDay))
        {
            Item newItem = itemDataTable.GetRandomItemByTier();

            item.SetDropItem(newItem);
            item.InitItemIcon(newItem);
        }
        else
        {
            Weapon newWeapon = weaponDataTable.GetRandomWeaponByTier();

            item.SetDropItem(newWeapon);
            item.InitItemIcon(newWeapon);
        }

        // 위치 설정
        item.SetPosition(transform.position);
    }
}
