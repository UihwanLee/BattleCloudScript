using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private ParticlePool pool;

    private void Start()
    {
        GameManager.Instance.ParticleManager = this;
    }

    public void PlayDismantleEffect(Transform target)
    {
        pool.PlayParticle("DismantleEffect", target);
    }

    public void PlayHitEffect(Transform target)
    {
        pool.PlayParticle("HitEffect", target);
    }

    public void PlayLootEffect(Transform target)
    {
        pool.PlayParticle("LootEffect", target);
    }
    public void PlayDropEffect(Transform target)
    {
        pool.PlayParticle("DropEffect", target);
    }
    


    #region 파티클 테스트
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            PlayDismantleEffect(GameManager.Instance.Player.transform);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            PlayHitEffect(GameManager.Instance.Player.transform);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            PlayLootEffect(GameManager.Instance.Player.transform);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            PlayDropEffect(GameManager.Instance.Player.transform);
        }
    }
    #endregion
}
