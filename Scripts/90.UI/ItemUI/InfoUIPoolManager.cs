using System.Collections.Generic;
using UnityEditor.Build.Player;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class InfoUIPoolManager : MonoBehaviour
{
    private static InfoUIPoolManager instance;

    public static InfoUIPoolManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = FindObjectOfType<InfoUIPoolManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private InfoUIPoolManager() { }

    private List<InfoUIPool> infoUIList = new List<InfoUIPool>();

    public void Add(InfoUIPool pool)
    {
        infoUIList.Add(pool);
    }

    public GameObject GetInfoUI(InfoUIType type)
    {
        foreach (var infoUI in infoUIList)
        {
            return infoUI.Get(type);
        }

        Debug.Log($"해당 {type} Pool을 찾을 수 없습니다!");
        return null;
    }

    public GameObject GetInfoUI(InfoUIType type, Vector3 position)
    {
        foreach (var infoUI in infoUIList)
        {
            return infoUI.Get(type, position);
        }

        Debug.Log($"해당 {type} Pool을 찾을 수 없습니다!");
        return null;
    }

    public void Release(InfoUIType type, GameObject obj)
    {
        foreach (var infoUI in infoUIList)
        {
            infoUI.Release(type, obj);
        }
    }

    public void ReleaseAll()
    {
        foreach (var infoUI in infoUIList)
        {
            infoUI.ReleaseAll();
        }
    }
}
