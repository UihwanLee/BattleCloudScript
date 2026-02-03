using System.Collections.Generic;
using UnityEngine;

public class WeaponMaterialController : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private List<Material> materials = new List<Material>();

    private WeaponBase owner;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        owner = GetComponentInParent<WeaponBase>();
    }

    private void OnEnable()
    {
        EventBus.OnWeaponLevelUp += UpdateMaterial;
    }

    private void OnDisable()
    {
        EventBus.OnWeaponLevelUp -= UpdateMaterial;
    }

    private void UpdateMaterial(int lv, WeaponBase weapon)
    {
        if (owner != weapon) return;

        spriteRenderer.material = materials[lv - 1];
    }
}
