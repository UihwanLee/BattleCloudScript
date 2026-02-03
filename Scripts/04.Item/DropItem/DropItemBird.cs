using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropItemBird : MonoBehaviour
{
    [Header("애니메이션 시간")]
    [SerializeField] private float flyDuration = 10f;
    [SerializeField] private float fallDuration = 4f;

    [Header("떨어지는 높이")]
    [SerializeField] private float heightMin = 5f;
    [SerializeField] private float heightMax = 7f;

    private Coroutine startDropItem;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 dropPosition;

    private string key;

    ISlotItem dropItem;

    [Header("드롭 속성")]
    [SerializeField] private float dropRadius = 0.5f;
    [SerializeField] private float dropDelayMin = 0.2f;
    [SerializeField] private float dropDelayMax = 0.6f;

    private bool isInDropArea = false;
    private bool hasDropped = false;
    private Coroutine dropCoroutine;

    public void SetBird(string _key, Vector3 start, Vector3 end, Vector3 drop, ISlotItem item)
    {
        startPosition = start;
        endPosition = end;
        dropPosition = drop;
        key = _key;

        dropItem = item;

        transform.position = startPosition;

        hasDropped = false;

        if (startDropItem != null) StopCoroutine(StartDropItemCoroutine());
        startDropItem = StartCoroutine(StartDropItemCoroutine());
    }

    private IEnumerator StartDropItemCoroutine()
    {
        float time = 0f;

        while (time < flyDuration)
        {
            time += Time.deltaTime;

            float t = time / flyDuration;

            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            CheckDropPosition();

            yield return null;
        }

        // 다 끝나면 Pool에 반납
        PoolManager.Instance.ReleaseObject(key, this.gameObject);
    }

    private void CheckDropPosition()
    {
        if (hasDropped)
            return;

        if(Vector3.Distance(transform.position, dropPosition) < 1f)
        {
            if(dropCoroutine == null)
                dropCoroutine = StartCoroutine(DropWithRandomDelay());
        }
    }

    private IEnumerator DropWithRandomDelay()
    {
        hasDropped = true;
        float delay = Random.Range(dropDelayMin, dropDelayMax);
        yield return new WaitForSeconds(delay);

        float height = Random.Range(heightMin, heightMax);
        DropItemPool.Instance.SpawnDropItemWithFall(
            dropItem,
            transform.position,
            height,
            fallDuration
        );

        dropCoroutine = null;
    }
}
