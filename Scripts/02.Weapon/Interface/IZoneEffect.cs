using UnityEngine;

public interface IZoneEffect
{
    WeaponBase Owner { get; }
    void Initialize(WeaponBase owner);
    void OnEnter(GameObject target);
    bool OnStay(GameObject target, float deltaTime);
    void OnExit(GameObject target);
}
