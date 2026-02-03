using UnityEngine;

public class DropPoolInitializer : MonoBehaviour
{
    [Header("리소스 드랍 프리팹")]
    [SerializeField] private GameObject expPrefab;
    [SerializeField] private GameObject hpPrefab;

    private void Start()
    {
        PoolManager.Instance.CreatePool("Drop_Exp", expPrefab, 20, transform);
        PoolManager.Instance.CreatePool("Drop_Hp", hpPrefab, 20, transform);
    }
}
