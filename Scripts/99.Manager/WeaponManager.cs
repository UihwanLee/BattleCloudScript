using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Pivots")]
    [SerializeField] private Transform[] weaponPivots = new Transform[Define.MAX_WEAPONCOUNT];
    [Range(0f, 3f)]
    [SerializeField] private float radius;

    private int currentWeaponIndex = 0;
    [SerializeField] private WeaponBase[] activeWeapons = new WeaponBase[Define.MAX_WEAPONCOUNT];

    private void Start()
    {
        Arrangement();
    }

    public WeaponBase GetWeapon(int index)
    {
        WeaponBase weapon = activeWeapons[index];

        if (weapon == null)
            return null;

        return activeWeapons[index];
    }

    /// <summary>
    /// 무기 추가
    /// </summary>
    public WeaponBase AddWeapon(GameObject weapon, int id, ISlotItem item, int index)
    {
        if (currentWeaponIndex >= Define.MAX_WEAPONCOUNT) return null;

        if (index >= Define.MAX_WEAPONCOUNT) return null;

        for (int i = 0; i < Define.MAX_WEAPONCOUNT; i++)
        {
            if (activeWeapons[i] == null)
            {
                if (weapon.TryGetComponent<WeaponBase>(out var component))
                {
                    Transform pivot = weaponPivots[index];

                    WeaponBase instance = Instantiate(component);
                    instance.transform.position = pivot.position;

                    WeaponTraitData data = DataManager.Get<WeaponTraitData>(id);
                    instance.Trait.SetTrait(data, item.GetLv());

                    instance.Initialize(pivot);


                    activeWeapons[index] = instance;
                    return instance;
                    //break;
                }
                else
                {
                    Debug.Log($"{weapon.gameObject.name} doesn't have WeaponBase");
                    return null;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 무기의 위치 서로 변경
    /// </summary>
    public void SwapWeaponPivot(int firstIdx, int secondIdx)
    {
        activeWeapons[firstIdx]?.Controller?.SetPivot(weaponPivots[secondIdx]);
        activeWeapons[secondIdx]?.Controller?.SetPivot(weaponPivots[firstIdx]);

        WeaponBase tempWeapon = activeWeapons[firstIdx];

        activeWeapons[firstIdx] = activeWeapons[secondIdx];
        activeWeapons[secondIdx] = tempWeapon;
    }

    /// <summary>
    /// 무기 버리기
    /// </summary>
    /// <param name="index"></param>
    public bool DeleteWeapon(int index)
    {
        if (activeWeapons[index] == null) return false;

        Destroy(activeWeapons[index].gameObject);
        activeWeapons[index] = null;

        //currentWeaponIndex--;

        return true;
    }

    /// <summary>
    /// 360도를 기준으로 MAX_WEAPONCOUNT 개수에 맞춰 균등하게 배치
    /// </summary>
    private void Arrangement()
    {
        Vector3[] positions = new Vector3[Define.MAX_WEAPONCOUNT];
        float angleStep = 360f / Define.MAX_WEAPONCOUNT;
        for (int i = 0; i < Define.MAX_WEAPONCOUNT; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
            positions[i] = pos;
        }

        weaponPivots[0].position = positions[1];
        weaponPivots[1].position = positions[0];
        weaponPivots[2].position = positions[5];
        weaponPivots[3].position = positions[4];
        weaponPivots[4].position = positions[3];
        weaponPivots[5].position = positions[2];
    }

    private void OnDrawGizmos()
    {
        foreach (Transform pivot in weaponPivots)
        {
            if (pivot == null) continue;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pivot.position, 0.3f);
        }
    }
}
