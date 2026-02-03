using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Transform을 이용하여 '이름'으로 자식/부모 오브젝트를 찾을 수 있는 확장 메서드
// 찾을 수 있는 컴포넌트는 정해져 있으며 이를 주의해야함 (GameObject/Transform 안됨)
// 또한 .tranform을 기준으로 자식/부모 오브젝트를 찾으므로 이점도 주의해야함
public static class FindExtension
{
    /// <summary>
    /// 자식 오브젝트 중 컴포넌트 오브젝트 찾기
    /// </summary>
    /// <typeparam name="T">컴포넌트 타입</typeparam>
    /// <param name="transform">Transform 기준</param>
    /// <param name="name">오브젝트 이름</param>
    /// <returns></returns>
    public static T FindChild<T>(this Transform transform, string name) where T : MonoBehaviour
    {
        T[] children = transform.GetComponentsInChildren<T>();
        foreach (var child in children)
        {
            if (child.name == name)
                return child;
        }

        return null;
    }

    /// <summary>
    /// 부모 오브젝트 중 컴포넌트 오브젝트 찾기
    /// </summary>
    /// <typeparam name="T">컴포넌트 타입</typeparam>
    /// <param name="transform">Transform 기준</param>
    /// <param name="name">오브젝트 이름</param>
    /// <returns></returns>
    public static T FindParent<T>(this Transform transform, string name) where T : MonoBehaviour
    {
        T[] parents = transform.GetComponentsInParent<T>();
        foreach (var parent in parents)
        {
            if (parent.name == name)
                return parent;
        }

        return null;
    }
}
