using System;
using UnityEngine;

// 게임 프로젝트 내 사용할 이벤트(Action, Func)을 접근 할 수 있는 전역 클래스
// 이벤트 등록은 등록할 오브젝트가 켜저 있는 상태에서 할 수 있기 때문에 주의해야함.
// 가장 좋은 방법은 이벤트 등록을 마치고 나서 비활성화 할 수 있도록 설계
// EventManager는 등록된 이벤트 델리게이트를 전역으로 참조할 수 있다.
// 따라서 EventManager에 Action/Func 델리게이트만 선언할 수 있도록 주의
public class EventManager : MonoBehaviour
{
    private static EventManager instance;

    public static EventManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = FindObjectOfType<EventManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private EventManager() { }

    //////////////////////////////////////////////////////////////////////////////////
    // 이벤트 정의
    public Action OnChangeLanuage;
}
