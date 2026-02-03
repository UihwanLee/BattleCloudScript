using UnityEngine;

public interface IKnockbackable
{
    /// <summary>
    /// 넉백을 적용시키는 메서드
    /// </summary>
    /// <param name="other">투사체 같은 공격의 주체</param>
    /// <param name="knockbackPower">넉백을 통해 밀려날 힘</param>
    public void ApplyKnockback(Transform other, float knockbackPower);
}
