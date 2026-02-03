using System;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

[Serializable]
public class InfoUI
{
    [Header("Key 이름")]
    public string key;
    [Header("InfoUI 프리팹")]
    public GameObject prefab;
    [Header("InfoUI 초기 생성 개수")]
    public int maxCount;
    [Header("Type")]
    public InfoUIType type;
}


// ItemInfoUIPool 스크립트
public class InfoUIPool : MonoBehaviour
{
    [Header("InfoUI 리스트")]
    [SerializeField] private List<InfoUI> infoUIs = new List<InfoUI>();
    [Header("Canvas 부모")]
    public GameObject canvasParent;

    private PoolManager poolManager;
    private InfoUIPoolManager manager;

    private void Start()
    {
        poolManager = PoolManager.Instance;
        manager = InfoUIPoolManager.Instance;
        Initialize();
    }

    private void Initialize()
    {
        foreach(var infoUI in infoUIs)
        {
            // Damage 생성
            poolManager.CreatePool(infoUI.key, infoUI.prefab, infoUI.maxCount, canvasParent.transform);

            // Pool 전달
            manager.Add(this);
        }
    }

    public GameObject Get(InfoUIType type)
    {
        // 가용 가능한 UI 넘기기
        foreach (var infoUI in infoUIs)
        {
            if (infoUI.type == type)
            {
                return poolManager.GetObject(infoUI.key);
            }
        }

        Debug.Log($"해당 {type} Pool을 찾을 수 없습니다!");
        return null;
    }

    public GameObject Get(InfoUIType type, Vector3 position)
    {
        // 가용 가능한 UI 넘기기
        foreach (var infoUI in infoUIs)
        {
            if (infoUI.type == type)
            {
                GameObject go = poolManager.GetObject(infoUI.key);
                go.transform.position = position;
                return go;
            }
        }

        Debug.Log($"해당 {type} Pool을 찾을 수 없습니다!");
        return null;
    }

    public void Release(InfoUIType type, GameObject obj)
    {
        foreach (var infoUI in infoUIs)
        {
            if (infoUI.type == type)
            {
                poolManager.ReleaseObject(infoUI.key, obj);
            }
        }
    }

    public void ReleaseAll()
    {
        foreach(var infoUI in infoUIs)
        {
            poolManager.ReleaseAllObject(infoUI.key);
        }
    }

    #region 프로퍼티

    public string Key { get { return infoUIs[0].key; } }
    public InfoUIType Type { get { return infoUIs[0].type; } }

    #endregion
}
