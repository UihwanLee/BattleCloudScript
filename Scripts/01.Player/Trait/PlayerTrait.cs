using UnityEngine;

public class PlayerTrait : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private string playerName;
    [SerializeField] private string desc;
    [SerializeField] private Sprite image;
    [SerializeField] private int statId;

    private Player player;
    private SpriteRenderer spriteRenderer;
    private Weapon startWeapon;

    private void Awake()
    {
        player = GetComponent<Player>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        int characterId = Define.PLATER_BASE_ID;

        if (GameManager.Instance != null)
        {
            characterId = GameManager.Instance.SelectedCharacterId;
        }
        SetTrait(characterId);
    }

    public void SetSpriteRenderer() 
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        GameManager.Instance.Player.Controller.SetSpriteRenderer(spriteRenderer);
    }

    private void SetTrait(int PLAYER_BASE_ID)
    {
        Debug.Log($"[PlayerTrait] SetTrait CALLED with ID={PLAYER_BASE_ID} | frame={Time.frameCount}");
        if (DataManager.Instance == null) Debug.Log("Data 초기화 안됨");

        // PlayerTraitData 가져오기
        PlayerTraitData data = DataManager.Get<PlayerTraitData>(PLAYER_BASE_ID);

        if (data == null) { Debug.Log("PlayerData가 없습니다!"); return; }

        this.id = data.ID;
        this.playerName = data.NAME;
        this.desc = data.DESC;
        this.statId = data.ID_STAT;

        // 해당 Stat Id Stat 초기화
        player.Stat.InitStat(statId);

        // Condition 초기화
        player.Condition.InitCondition(data.LV, data.MAX_HP, data.HP_COEFFICIENT, data.MAX_EXP, data.EXP_COEFFICIENT, data.LEVEL_STAT_COEFFICIENT);

        // 조언자 초기화
        Weapon weapon = new Weapon();

        // 캐릭터 이미지 가져오기
        Sprite characterImg = DataManager.GetPrefab(data.PREFAB).GetComponent<SpriteRenderer>().sprite;
        EventBus.OnPlayerStart(characterImg);

        weapon.SetWeapon(data.ID_WEAPON);

        startWeapon = weapon;
    }

    public void InitPlayerWeapon()
    {
        player.PlayerSlotUI.AddWeapon(startWeapon);
    }

    #region 프로퍼티

    public int ID { get { return id; } }
    public string Name { get { return playerName; } }
    public string Desc { get { return desc; } }
    public int StatID { get { return statId; } }

    #endregion
}
