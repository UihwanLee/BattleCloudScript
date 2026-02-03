using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemBirdManager : MonoBehaviour
{
    [Header("Pool Key")]
    [SerializeField] private string key = "Bird";

    [Header("Bird 프리팹")]
    [SerializeField] private GameObject birdPrefab;

    [Header("포지션")]
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private float startRadius = 2.0f;

    [Header("담고 있는 아이템 리스트")]
    [SerializeField] private List<ISlotItem> items = new List<ISlotItem>();

    private float minWaitTime = 0.5f;
    private float maxWaitTime = 1.5f;

    private Coroutine dropStartCoroutine;

    private void Reset()
    {
        startPosition = GameObject.Find("BirdStartPosition").GetComponent<Transform>();
        dropPosition = GameObject.Find("BirdDropPosition").GetComponent<Transform>();
        endPosition = GameObject.Find("BirdEndPosition").GetComponent<Transform>();
    }

    private void OnEnable()
    {
        EventBus.OnGainItemByAnomaly += CheckGainDropItem;
    }

    private void OnDisable()
    {
        EventBus.OnGainItemByAnomaly -= CheckGainDropItem;
    }

    private void Start()
    {
        InitBird();
    }

    private void InitBird()
    {
        PoolManager.Instance.CreatePool(key, birdPrefab, 1, this.transform);
    }

    public void CheckGainDropItem(List<ISlotItem> _items)
    {
        items = _items;

        //foreach (HUDItemSlot slot in list)
        //{
        //    if (slot.gameObject.activeSelf)
        //    {
        //        items.Add(slot.Item);
        //        slot.gameObject.SetActive(false);
        //    }
        //}

        if (items.Count <= 0)
        {
            // 없다면 리셋
            return;
        }


        // 특정 시간 후 드랍 시작
        if (dropStartCoroutine != null) StopCoroutine(dropStartCoroutine);
        dropStartCoroutine = StartCoroutine(StartDropCoroutine());
    }


    private IEnumerator StartDropCoroutine()
    {
        foreach (ISlotItem item in items)
        {
            float randomWait = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(randomWait);

            // Bird Coroutine 시작
            DropItemBird bird = PoolManager.Instance.GetObject(key).GetComponent<DropItemBird>();
            bird.SetBird(key, GetRandomStartPosition(startPosition.position, startRadius), 
                endPosition.position, dropPosition.position, item);
        }
    }

    private Vector3 GetRandomStartPosition(Vector3 pivot, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return pivot + new Vector3(randomCircle.x, randomCircle.y, 0f);
    }
}
