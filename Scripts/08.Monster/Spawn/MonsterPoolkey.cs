using UnityEngine;

public class MonsterPoolKey : MonoBehaviour
{
    [Header("Trait ID")]
    [SerializeField] private int monsterId;

    public int MonsterId => monsterId;
    public string PoolKey => $"Monster_{monsterId}";
}
