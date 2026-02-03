using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

// Player Stat 클래스
// Player Stat Data를 로드/세이브하여 사용한다.
// Data는 float 값이지만 Attribute 클래스를 사용
public class PlayerStat : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private int id;
    [SerializeField] private Attribute command;
    [SerializeField] private Attribute moveSpeed;
    [SerializeField] private Attribute evasion;
    [SerializeField] private Attribute defense;
    [SerializeField] private Attribute luck;

    // Stat Attribute Dictionary
    private Dictionary<AttributeType, Attribute> attributeDict;

    private void Start()
    {
        //Initialize();
    }

    public void InitStat(int ID_STAT)
    {
        if (DataManager.Instance == null) Debug.Log("Data 초기화 안됨");

        // PlayerStatData 가져오기
        PlayerStatData data = DataManager.Get<PlayerStatData>(ID_STAT);

        if (data == null) { Debug.Log("PlayerData가 없습니다!"); return; }

        id = data.ID;
        command = new Attribute(0, data.COMMAND, 1f);
        moveSpeed = new Attribute(1, data.MOVE_SPEED, 0.1f);
        evasion = new Attribute(2, data.EVASION, 0f);
        defense = new Attribute(3, data.DEFENSE, 0f);
        luck = new Attribute(4, data.LUCK, 0f);

        attributeDict = new Dictionary<AttributeType, Attribute>();
        attributeDict.Add(AttributeType.Command, command);
        attributeDict.Add(AttributeType.MoveSpeed, moveSpeed);
        attributeDict.Add(AttributeType.Evasion, evasion);
        attributeDict.Add(AttributeType.Defense, defense);
        attributeDict.Add(AttributeType.Luck, luck);
    }

    public void LoadStat(int ID_STAT)
    {
        // PlayerStatData 가져오기
        PlayerStatData data = DataManager.Get<PlayerStatData>(ID_STAT);

        if (data == null) { Debug.Log("PlayerStatData가 없습니다!"); return; }

        id = data.ID;

        // 로드 한 스탯 적용
        Set(AttributeType.Command, data.COMMAND);
        Set(AttributeType.MoveSpeed, data.MOVE_SPEED);
        Set(AttributeType.Evasion, data.EVASION);
        Set(AttributeType.Defense, data.DEFENSE);
        Set(AttributeType.Luck, data.LUCK);
    }

    public void Add(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out Attribute attribute))
        {
            attribute.Add(amount);

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void Sub(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out Attribute attribute))
        {
            attribute.Sub(amount);

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void Set(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out Attribute attribute))
        {
            attribute.Set(amount);

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }


    #region 프로퍼티

    public int ID { get { return id; } }
    public Attribute Command { get { return command; } }
    public Attribute MoveSpeed { get { return moveSpeed; } }
    public Attribute Evasion { get { return evasion; } }
    public Attribute Defense { get { return defense; } }
    public Attribute Luck {  get { return luck; } }

    #endregion
}
