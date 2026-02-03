using System.Collections.Generic;
using UnityEngine;

// 게임 내 사용할 오브젝트들을 Pool에 담을 수 있는 클래스
// pool 타입은 GameObject로 정의한다. 
// parent를 지정하여 특정 부모 Transform에서 생성할 수 있도록 설계
public class ObjectPool 
{
    private Queue<GameObject> pool = new Queue<GameObject>();      // 재활용 오브젝트에 담을 Queue
    private GameObject prefab;                                     // 복사하여 사용할 원본 오브젝트
    private Transform parent;                                      // 재활용할 오브젝트를 모아둘 부모 프로젝트

    private HashSet<GameObject> activeObjects = new HashSet<GameObject>(); // 사용중인 오브젝트

    /// <summary>
    /// ObjectPool 생성
    /// </summary>
    /// <param name="prefab">Pool에 생성할 오브젝트</param>
    /// <param name="initialSize">생성 개수</param>
    /// <param name="parent">생성 Transform</param>
    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Pool에서 오브젝트 가져오기
    /// </summary>
    /// <returns>Pool 오브젝트</returns>
    public GameObject Get()
    {
        // Pool에서 해당 오브젝트가 없을 시 생성해서 반환
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : GameObject.Instantiate(prefab, parent);

        // 오브젝트 활성화
        obj.gameObject.SetActive(true);
        activeObjects.Add(obj);

        // 오브젝트 반환
        return obj;
    }
    public bool TryGet(out GameObject obj)
    {
        if (pool.Count <= 0)
        {
            obj = null;
            return false;
        }

        obj = pool.Dequeue();
        obj.SetActive(true);
        return true;
    }
    public int AvailableCount => pool.Count;
    /// <summary>
    /// Pool에 오브젝트 반환
    /// </summary>
    public void Release(GameObject obj)
    {
        // 다 사용한 오브젝트는 비활성화하고 Pool에 반납
        obj.SetActive(false);
        activeObjects.Remove(obj);
        pool.Enqueue(obj);
    }

    /// <summary>
    /// Pool에 있는 모든 오브젝트 반환
    /// </summary>
    public void ReleaseAll()
    {
        foreach(var obj in activeObjects)
        {
            obj?.SetActive(false);
            pool.Enqueue(obj);
        }

        activeObjects.Clear();
    }

    /// <summary>
    /// Pool 부모 갱신
    /// </summary>
    /// <param name="newParent"></param>
    public void UpdateParent(Transform newParent)
    {
        this.parent = newParent;

        // 대기 중인 오브젝트들의 부모를 새 씬의 부모로 이동
        foreach (var obj in pool)
        {
            if (obj != null) obj.transform.SetParent(newParent);
        }

        // 활성화된 오브젝트들의 부모도 이동
        foreach (var obj in activeObjects)
        {
            if (obj != null) obj.transform.SetParent(newParent);
        }
    }

    /// <summary>
    /// Pool 내부 사용중인 오브젝트 모두 삭제
    /// </summary>
    public void ClearPool()
    {
        // 사용 중인 오브젝트 삭제
        foreach (var obj in activeObjects)
        {
            if (obj != null) GameObject.Destroy(obj);
        }
        activeObjects.Clear();

        // 대기 중 오브젝트 삭제
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (obj != null) GameObject.Destroy(obj);
        }
        pool.Clear();
    }
}
