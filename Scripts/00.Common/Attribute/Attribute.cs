using UnityEngine;

// 모든 데이터 값과 관련된 정보를 가지고 있는 스크립트
// 변수 자료구조는 float로 고정한다. (필요한 경우 타입 캐스팅해서 써도 되지만 기본적으로 정수 값으로 고정)
// 클래스 형태로 Add, Sub, Set 메서드를 통해 Value를 조절할 수 있다.
// 후에 Dictinoary에서 Sort하기 위한 LocalIndex 필드도 보유하고 있다. 생성자에서 설정해줘야함
[System.Serializable]
public class Attribute
{
    [SerializeField] protected int localIndex;        // 로컬 Index 값
    [SerializeField] protected float value;           // 값
    [SerializeField] protected float minValue;        // 최소 값

    public Attribute(int localIndex, float value, float minValue)
    {
        this.localIndex = localIndex;
        this.value = value;
        this.minValue = minValue;
    }

    public virtual void Add(float amount)
    {
        this.value += amount;
    }

    public virtual void Sub(float amount)
    {
        this.value = Mathf.Max(this.value - amount, minValue);
    }

    public virtual void Set(float amount)
    {
        this.value = amount;
    }

    #region 프로퍼티

    public float LocalIndex { get { return localIndex; } }
    public float Value { get { return value; } }
    public float MinValue { get { return minValue; } }

    #endregion
}
