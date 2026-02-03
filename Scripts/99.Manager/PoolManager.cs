using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// ObjectPool을 모아놓은 전역 PoolManager 스크립트
// ObjectPool Dictionary 자료구조로 관리
// MonoBehaivor 상속하고 있기 때문에 빈 오브젝트에서 컴포넌트 붙이고 사용해야함
public class PoolManager : MonoBehaviour
{
    [Header("씬 로드 시 파괴하지 않은 Key 리스트")]
    [SerializeField] string[] excludeKeys;

    private static PoolManager instance;

    private Dictionary<string, ObjectPool> objectPools = new Dictionary<string, ObjectPool>();

    public static PoolManager Instance { get; set; }

    private void OnEnable()
    {
        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private PoolManager() { }

    /// <summary>
    /// ObjectPool 생성
    /// </summary>
    /// <param name="key">Pool Key</param>
    /// <param name="prefab">Pool 오브젝트</param>
    /// <param name="initialSize">오브젝트 생성 개수</param>
    /// <param name="parent">오브젝트 생성 부모</param>
    public void CreatePool(string key, GameObject prefab, int initialSize, Transform parent = null)
    {
        if (objectPools.ContainsKey(key))
        {
            Debug.Log($"해당 {key}값을 가지고 있는 ObjectPool이 이미 존재합니다.");
            return;
        }

        Transform newParent = parent;
        if (newParent == null)
        {
            GameObject poolParent = new GameObject($"Pool_{key}");
            poolParent.transform.SetParent(this.transform);
            newParent = poolParent.transform;
        }

        ObjectPool pool = new ObjectPool(prefab, initialSize, newParent);
        objectPools.Add(key, pool);
    }

    /// <summary>
    /// ObjectPool에서 Object 가져오기
    /// </summary>
    /// <param name="key">Pool Key</param>
    /// <returns>Pool Object</returns>
    public GameObject GetObject(string key)
    {
        // Dictionary에서 해당 key에 맞는 object를 가져옴
        if (objectPools.TryGetValue(key, out ObjectPool pool))
        {
            return pool.Get();
        }

        Debug.Log($"해당 {key}값을 가지고 있는 ObjectPool이 존재하지 않습니다");
        return null;
    }

    /// <summary>
    /// ObjectPool에 Object 반납
    /// </summary>
    /// <param name="key">Pool Key</param>
    /// <param name="obj">반납할 Object</param>
    public void ReleaseObject(string key, GameObject obj)
    {
        // Dicitonay에서 해당 key에 맞는 Pool 가져옴
        if (objectPools.TryGetValue(key, out ObjectPool pool))
        {
            pool.Release(obj);
        }
        else
        {
            // Pool 없다면 오브젝트를 강제로 비활성화하여 디버그 출력
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
            }

            Debug.Log($"해당 {key}값을 가지고 있는 ObjectPool이 존재하지 않습니다");
        }
    }

    /// <summary>
    /// 해당 Key에 있는 모든 Object 반납
    /// </summary>
    /// <param name="key">Pool key</param>
    public void ReleaseAllObject(string key)
    {
        // Dicitonay에서 해당 key에 맞는 Pool 가져옴
        if (objectPools.TryGetValue(key, out ObjectPool pool))
        {
            pool.ReleaseAll();
        }
    }

    public bool HasPool(string key)
    {
        return objectPools.ContainsKey(key);
    }

    public bool TryGetObject(string key, out GameObject obj)
    {
        obj = null;

        if (objectPools.TryGetValue(key, out ObjectPool pool))
            return pool.TryGet(out obj);

        Debug.Log($"해당 {key}값을 가지고 있는 ObjectPool이 존재하지 않습니다");
        return false;
    }

    public int GetAvailableCount(string key)
    {
        if (objectPools.TryGetValue(key, out ObjectPool pool))
            return pool.AvailableCount;

        return 0;
    }

    #region Scene 로드 시 Pool 초기화
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 필요한 키 빼고 모두 삭제
        ClearAllPoolOnLoadedWithOutArray(excludeKeys);
    }

    private void ClearAllPoolOnLoadedWithOutArray(string[] outKeys)
    {
        // string[]에 포함된 key값만 빼고 모두 삭제
        List<string> keysToRemove = objectPools.Keys
            .Where(key => !outKeys.Contains(key))
            .ToList();

        // Pool 정리
        foreach (string key in keysToRemove)
        {
            if (objectPools.ContainsKey(key))
            {
                // ClearPool 호출
                objectPools[key].ClearPool();

                // 딕셔너리에서 제거
                objectPools.Remove(key);
            }
        }
    }
    #endregion
}
