using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    [SerializeField] private GameObject[] particlePrefabs;
    private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        foreach (var prefab in particlePrefabs)
        {
            pool[prefab.name] = new Queue<GameObject>();
        }
    }

    public GameObject PlayParticle(string particleName, Transform target)
    {
        GameObject obj;
        if (pool[particleName].Count > 0)
        {
            obj = pool[particleName].Dequeue();
            obj.SetActive(true);
        }
        else
        {
            var prefab = System.Array.Find(particlePrefabs, p => p.name == particleName);
            obj = Instantiate(prefab, transform); // 풀 밑에만 생성
        }

        // 위치 초기화
        obj.transform.position = target.position + new Vector3(0, -0.3f, 0);

        // 타겟 따라가도록 설정
        var autoReturn = obj.GetComponent<ParticleAutoReturn>();
        if (autoReturn != null)
        {
            autoReturn.SetTarget(target);
        }

        return obj;
    }

    public void ReturnParticle(string particleName, GameObject obj)
    {
        obj.SetActive(false); // 풀 밑에 그대로 두고 비활성화만
        pool[particleName].Enqueue(obj);
    }
}
