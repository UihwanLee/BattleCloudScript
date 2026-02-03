using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

// 다양한 Record를 가지고 있는 클래스
public class RecordManager : MonoBehaviour
{
    private static RecordManager instance;

    public static RecordManager Instance
    {
        get
        {
            if(instance == null)
                instance = GameObject.FindFirstObjectByType<RecordManager>();
            return instance;
        }
    }

    private RecordManager() { }

    [SerializeField] private Dictionary<string, bool> weaponRecord = new Dictionary<string, bool>();

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    #region Weapon Record

    /// <summary>
    /// 조언자 획득 시 지정
    /// </summary>
    /// <param name="type"></param>
    public void GetWeapon(ISlotItem item)
    {
        if(item is Weapon weapon)
        {
            weaponRecord[weapon.Type.ToString()] = true;
        }
    }

    /// <summary>
    /// 주어진 타입이 Record에 기록되어 있는지 체크
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool CanWeapon(string type)
    {
        if (weaponRecord.ContainsKey(type))
        {
            return weaponRecord[type];
        }

        return false;
    }

    #endregion
}
