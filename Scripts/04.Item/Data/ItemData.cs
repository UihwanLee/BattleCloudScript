using System;
using UnityEngine;

// Item Data 컨테이너 클래스
[Serializable]
public class ItemData
{
    [Header("ID")]
    public int ID;                          // ID
    [Header("아이템 이름")]
    public string NAME;                     // 아이템 이름
    [Header("아이템 설명")]
    public string DESC;                     // 아이템 설명
    [Header("아이템 이미지")]
    public string IMAGE;                    // 아이템 이미지
    [Header("아이템 효과")]
    public string EFFECT;                   // 아이템 효과
    [Header("아이템 트리거")]
    public string TRIGGER;                  // 아이템 트리거
    [Header("아이템 적용 대상")]
    public string APPLYTARGET;              // 아이템 적용 대상
    [Header("아이템 적용 타입")]
    public string APPLYTYPE;                // 아이템 적용 대상
    [Header("아이템 효과 수치")]
    public string VALUE;                    // 적용 수치 
    [Header("아이템 효과 증가율")]
    public string VALUE_MUTIPLIER;         // 수치 증가율
    [Header("아이템 상점 가격")]
    public float PRICE;                    // 아이템 상점 가격
    [Header("아이템 티어")]
    public string TIER;                      // 아이템 티어
}
