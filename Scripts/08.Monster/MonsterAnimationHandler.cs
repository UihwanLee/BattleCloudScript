using UnityEngine;

public class MonsterAnimationHandler : MonoBehaviour
{
    [SerializeField] private Monster monster;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Play()
    {
        animator.speed = 1f;
    }

    public void PlayAnimationWithTrigger(int key)
    {
        animator.SetTrigger(key);
    }

    public void PlayAnimationWithBool(int key, bool value)
    {
        animator.SetBool(key, value);
    }

    public void Pause()
    {
        animator.speed = 0f;
    }

    public void OnFire()
    {
        if (monster.Controller.TryGetComponent<RangedMonsterController>(out var controller))
        {
            controller.Fire();
        }
    }
}
