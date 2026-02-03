using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

//[Serializable]
//public class FloatingTextPair
//{
//    [SerializeField] private FloatingTextSO data;
//    [SerializeField] private GameObject prefab;
//    [SerializeField] private int poolSize;

//    public FloatingTextSO Data => data;
//    public GameObject Prefab => prefab;
//    public int PoolSize => poolSize;
//}

// FloatingText를 담고 있는 Pool 클래스
// 담고 있는 Canvas가 WorldSpace로 세팅되어 있는지 확인
public class FloatingTextPool : MonoBehaviour
{
    [Header("FloatingText 프리팹")]
    [SerializeField] private GameObject prefab;
    [Header("FloatingText 초기 생성 개수")]
    [SerializeField] private int maxCount;
    [Header("FloatingText SO 데이터")]
    [SerializeField] private FloatingTextSO data;

    private string key;
    private PoolManager poolManager;
    private FloatingTextPoolManager manager;

    private float offsetX = 0.5f;
    private float offsetY = 0.5f;

    private void Start()
    {
        poolManager = PoolManager.Instance;
        manager = FloatingTextPoolManager.Instance;
        Initialize();
    }

    private void Initialize()
    {
        //Key 저장
        key = data.Type.ToString();

        // Damage 생성
        poolManager.CreatePool(key, prefab, maxCount, transform);

        // Pool 전달
        manager.Add(this);
    }

    public GameObject SpawnText(string text, Transform target, Color? color = null)
    {
        // Text를 세팅하고 넘기기
        GameObject newText = poolManager.GetObject(key);

        if (newText != null)
        {
            if (newText.TryGetComponent<FloatingText>(out FloatingText floatingText))
            {
                // 초기화
                floatingText.Initialize();

                // 텍스트 설정
                floatingText.SetText(text);

                //// 색상 설정
                Color newColor = (Color)((color != null) ? color : data.Color);
                floatingText.SetColor(newColor);

                // Duration 설정
                floatingText.SetDuration(data.Duration);

                // 위치 설정 -> WorldSpace에서 그대로 설정해야함
                //Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position);
                Vector3 pos = target.position;
                pos.x = Random.Range(pos.x - offsetX, pos.x + offsetX);
                pos.y += offsetY;
                floatingText.SetPosition(pos);

                // Floating 코루틴은 몇 초 뒤에 자동으로 반환
                floatingText.StartFadeCoroutine(data);
            }
        }

        return newText;
    }

    public void Release(GameObject obj)
    {
        poolManager.ReleaseObject(key, obj);
    }

    #region 프로퍼티

    public string Key { get { return key; } }
    public FloatingTextSO Data { get { return data; } }
    public FloatingTextType Type { get { return data.Type; } }

    #endregion
}
