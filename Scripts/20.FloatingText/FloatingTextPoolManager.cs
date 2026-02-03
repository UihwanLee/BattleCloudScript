using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

// FloatingText에 접근할 수 있는 전역 클래스
// FloatingTextType과 Color 속성을 지정하여 UI 텍스트를 표시할 수 있다.
public class FloatingTextPoolManager : MonoBehaviour
{
    public static FloatingTextPoolManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private FloatingTextPoolManager() { }

    private List<FloatingTextPool> floatingTexts = new List<FloatingTextPool>();

    public void Add(FloatingTextPool pool)
    {
        floatingTexts.Add(pool);
    }

    /// <summary>
    /// 데미지 폰트 생성
    /// </summary>
    /// <param name="key">FloatingTextType.ToString()</param>
    /// <param name="text">Damage.ToString</param>
    /// <param name="target">텍스트 생성 위치</param>
    /// <param name="color">텍스트 색상</param>
    public void SpawnText(string key, string text, Transform target, Color? color = null)
    {
        // Key로 찾기
        foreach (var floatText in floatingTexts)
        {
            if (floatText.Key == key)
            {
                floatText.SpawnText(text, target, color);
                return;
            }
        }

        Debug.Log($"해당 {key} Pool을 찾을 수 없습니다!");
    }

    public void SpawnText(FloatingTextType type, string text, Transform target, Color? color = null)
    {
        // Type으로 찾기
        foreach (var floatText in floatingTexts)
        {
            if (floatText.Type == type)
            {
                floatText.SpawnText(text, target, color);
                return;
            }
        }

        Debug.Log($"해당 {type} Pool을 찾을 수 없습니다!");
    }

    public void Release(string key, GameObject obj)
    {
        foreach (var floatText in floatingTexts)
        {
            if (floatText.Key == key)
            {
                floatText.Release(obj);
                return;
            }
        }
    }
}
