
// Weapon 정보를 담은 Slot
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlot : Slot
{
    [Header("슬롯 정보")]
    [SerializeField] List<ItemSlot> itemSlots = new List<ItemSlot>();
    [SerializeField] private InteractBG interactBG;
    private WeaponBase weaponBase;

    private Vector3 originPosition;
    private Vector3 interactPosition;
    private bool isHighlightBG = false;

    public override void Reset()
    {
        base.Reset();

        itemSlots = transform.GetComponentsInChildren<ItemSlot>().ToList();
        foreach(var item in itemSlots)
        {
            item.Reset();
        }
    }

    public void LoadSlot(WeaponSlotData _weaponSlot)
    {
        if (_weaponSlot == null) return;

        if (_weaponSlot.Item != null)
        {
            _weaponSlot.Item.Load(_weaponSlot.ItemID);
            AddItem(_weaponSlot.Item, Index, false);
        }

        for(int i=0; i< _weaponSlot.ItemSlotsData.Count; i++)
        {
            itemSlots[i].LoadSlot(_weaponSlot.ItemSlotsData[i], _weaponSlot.Index);
        }
    }

    public void ResetAllSlot()
    {
        base.ResetSlot();
        foreach (ItemSlot item in itemSlots)
        {
            item.ResetSlot();
        }
    }

    public override void SetIndex(int index)
    {
        // WeaponSlot과 WeaponSlot이 지니고 있는 ItemSlot은 같은 index를 가짐
        base.SetIndex(index);
        foreach (ItemSlot item in itemSlots)
        {
            item.SetIndex(index);
        }

        if (interactBG == null) Debug.Log($"{index}번째 슬롯에 interactBG가 없음");
        interactBG.InitIndex(index);
    }

    public void SetInteractBG(float offset)
    {
        originPosition = interactBG.gameObject.transform.localPosition;
        interactPosition = interactBG.gameObject.transform.localPosition + new Vector3(offset, 0f, 0f);

        interactBG.SetOffset(originPosition, interactPosition);
    }

    public override void InitAnimationData()
    {
        base.InitAnimationData();
        foreach (ItemSlot item in itemSlots)
        {
            item.InitAnimationData();
        }
    }

    public override void InitIcon()
    {
        base.InitIcon();
        foreach (ItemSlot item in itemSlots)
        {
            item.InitIcon();
        }
    }

    public void InitPlayer(Player player, PlayerSlotUI playerSlotUI)
    {
        base.SetPlayer(player, playerSlotUI);
        foreach (ItemSlot item in itemSlots)
        {
            item.SetPlayer(player, playerSlotUI);
        }
    }

    public void InitSlot()
    {
        SetHighlight(false);
        foreach (ItemSlot item in itemSlots)
        {
            item.SetHighlight(false);
        }
    }

    public void HighlightInteractBG(bool actvive)
    {
        interactBG.HighlightInteractBG(actvive);
    }

    public void SetInteractBGValue(bool active)
    {
        isHighlightBG = active;
    }


    public override void AddItem(ISlotItem item, int index, bool isLog, bool? isSwap = null)
    {
        base.AddItem(item, index, isLog, isSwap);
        Debug.Log($"{Index}번째 슬롯에서 아우라 활성화");
        aura.gameObject.SetActive(true);

        // 스왑이라면 return
        if (isSwap != null) return;
        if (playerSlotUI == null) 
        {
            playerSlotUI = GameManager.Instance.Player.GetComponentInChildren<PlayerSlotUI>(); 
        }
        // Weapon 소환
        if (item != null)
        {
            WeaponManager weaponManager = GameManager.Instance.Player.transform.GetComponentInChildren<WeaponManager>();
            if (weaponManager != null) 
            {
                weaponBase = weaponManager.AddWeapon(item.GetPrefab(), item.GetID(), item, Index);
                ApplyItemSlot(true);
                playerSlotUI.CurrentActiveWeaponSlotCount++;
                EventBus.OnChangeWeaponSlotCount?.Invoke(playerSlotUI.CurrentActiveWeaponSlotCount, playerSlotUI.ActiveWeaponSlotCount);
                if(isLog) EventBus.OnDropItemGain?.Invoke(item, item.GetLv());
                RecordManager.Instance.GetWeapon(item);
                base.UpdateLv(item.GetLv());

                // DetailView 갱신
                if (GameManager.Instance.DetailViewUI)
                {
                    if (GameManager.Instance.DetailViewUI.DetailViewWindow.activeSelf)
                    {
                        if (GameManager.Instance.DetailViewUI.CurrentViewWeaponIndex == index)
                            GameManager.Instance.DetailViewUI.SetUI(index);
                    }
                }
            }
        }
        
        playerSlotUI.UpdateWeaponSlot();
    }

    public override void DeleteItem(ISlotItem item, int index, bool? isSwap = null)
    {
        base.DeleteItem(item, index);
        aura.gameObject.SetActive(false);
        Debug.Log($"{Index}번째 슬롯에서 아우라 제거");

        // 스왑이라면 return
        if (isSwap != null) return;

        // Weapon 빼기
        if (item != null)
        {
            WeaponSlotData data = GetWeaponData();
            ApplyItemSlot(false);
            player.WeaponManager.DeleteWeapon(data.Index);
            weaponBase = null;
            playerSlotUI.CurrentActiveWeaponSlotCount--;
            EventBus.OnChangeWeaponSlotCount?.Invoke(playerSlotUI.CurrentActiveWeaponSlotCount, playerSlotUI.ActiveWeaponSlotCount);

            // DetailView 갱신
            if (GameManager.Instance.DetailViewUI.DetailViewWindow.activeSelf)
            {
                if (GameManager.Instance.DetailViewUI.CurrentViewWeaponIndex == index)
                    GameManager.Instance.DetailViewUI.SetUI(index);
            }
        }

        playerSlotUI.UpdateWeaponSlot();
    }

    private void ApplyItemSlot(bool add)
    {
        // 빈 슬롯에 들어 갔다면 아이템 적용
        foreach (ItemSlot slot in itemSlots)
        {
            if(slot.Item != null)
            {
                Item item = (Item)slot.Item;
                item.ApplyItem(player, add, Index);
            }
        }
    }

    public WeaponSlotData GetWeaponData()
    {
        WeaponSlotData data = new WeaponSlotData
        {
            Index = this.Index,
            Item = this.Item,
            IsActvie = this.IsActive,
            ItemSlotsData = new List<ItemSlotData>()
        };

        // 서브 슬롯 (itemSlots) 데이터 추출
        foreach (var subSlot in itemSlots)
        {
            data.ItemSlotsData.Add(new ItemSlotData
            {
                Item = subSlot.Item,
                IsActvie = subSlot.IsActive,
            });
        }
        return data;
    }

    // 다른 Weapon Slot과 Swap
    // WeaponSlot 끼리 스왑이라서 Weapon이 새로 생성하거나 삭제하는 일이 없게 해야 한다.
    public void SetWeaponData(WeaponSlotData otherWeaponSlot)
    {
        // 일단 다 삭제
        DeleteAllSlot();

        // WeaponSlot 적용
        if (otherWeaponSlot.Item != null)
        {
            AddItem(otherWeaponSlot.Item, Index, false, true);
        }

        // ItemSlot 상태 그대로 적용
        for(int i=0; i<itemSlots.Count; i++)
        {
            // Item을 가지고 있다면 Item 적용
            if (otherWeaponSlot.ItemSlotsData[i].Item != null)
            {
                itemSlots[i].AddItem(otherWeaponSlot.ItemSlotsData[i].Item, Index, false, true);
            }
        }
    }

    private void DeleteAllSlot()
    {
        base.DeleteItem(Item, Index, true);
        foreach (ItemSlot item in itemSlots)
        {
            item.DeleteItem(item.Item, Index, true);
        }
    }

    /// <summary>
    /// 해금된 아이템 슬롯에 Item 추가
    /// </summary>
    /// <returns></returns>
    public bool AddItemAtEmtpyItemSlot(ISlotItem item)
    {
        for(int i=0; i<itemSlots.Count; i++)
        {
            if (itemSlots[i].Item == null)
            {
                // 비워있는 ItemSlot에 item 추가
                itemSlots[i].AddItem(item, Index, true);
                return true;
            }
        }

        return false;
    }

    public void UpdateItemSlot()
    {
        if (playerSlotUI == null) return;

        if(playerSlotUI.ActiveItemSlotCount <= playerSlotUI.CurrentActiveItemSlotCount)
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                itemSlots[i].SetHighlight(!itemSlots[i].IsActive);
                itemSlots[i].UpdateLvUI();
            }
        }
        else
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                itemSlots[i].SetHighlight(false);
                itemSlots[i].UpdateLvUI();
            }
        }
    }

    public override void UpdateLv(Attribute _Lv)
    {
        // 현재 아이템에도 레벨 적용
        item.SetLv(_Lv.Value);
        lv.text = (_Lv.Value < Define.MAX_WEAPON_LV) ? $"Lv. {_Lv.Value}" : Define.MAX_LV_LABEL;
        lvContainer.color = item.GetTierColor();

        UpdateIconMaterial(_Lv);
    }

    public void WeaponDragHighlight()
    {
        // Weapon 드래그 시 Item 슬롯은 모두 하이라이트 키고 Weapon 슬롯은 하이라이트 끄기
        SetHighlight(false);
        foreach (ItemSlot item in itemSlots)
        {
            item.SetHighlight(true);
        }
    }

    public void ItemDragHighlight()
    {
        // Item 드래그 시 Weapon 슬롯은 모두 하이라이트 키고 Item 슬롯은 하이라이트 끄기
        SetHighlight(true);
        foreach (ItemSlot item in itemSlots)
            item.SetHighlight(false);
    }

    public void CloseAllHighlight()
    {
        SetHighlight(false);
        foreach (ItemSlot item in itemSlots)
            item.SetHighlight(false);
    }

    public void AllHighlight()
    {
        SetHighlight(true);
        foreach (ItemSlot item in itemSlots)
            item.SetHighlight(true);
    }

    public void CloseLvUI()
    {
        SetLvUI(false);
        foreach (ItemSlot item in itemSlots)
            item.SetLvUI(false);
    }

    public override void UpdateLvUI()
    {
        base.UpdateLvUI();
        foreach (ItemSlot item in itemSlots)
            item.UpdateLvUI();
    }

    public void UpdateWeaponLevelUp(WeaponBase weapon)
    {
        if (weaponBase == null) return;

        // 현재 참조하고 있는 WeaponBase와 일치할 때
        if(weaponBase == weapon)
        {
            // 레벨 업데이트
            UpdateLv(weapon.Condition.Lv);
        }
    }

    public void UpdateReinforcementCostItemSlot()
    {
        foreach (ItemSlot item in itemSlots)
            item.SetReinforcementCost();
    }

    public void ResetReinforcementCostItemSlot()
    {
        foreach (ItemSlot item in itemSlots)
            item.ResetReinforcementCost();
    }

    public void GainExp(float value)
    {
        if (IsActive == false) return;

        // 현재 weaponBase가 있을 때만 적용
        if(weaponBase)
        {
            // 오브젝트에 경험치 적용
            weaponBase.Condition.Add(WhereAreYouLookinAt.Enum.AttributeType.Exp, value);
        }
    }

    public void LevelUp()
    {
        if(IsActive == false) return;

        // 현재 weaponBase가 있을 때만 적용
        if (weaponBase)
        {
            // 현재 부족한 경험치 계산
            float remainExp = weaponBase.Condition.MaxExp.Value - weaponBase.Condition.Exp.Value;

            // 오브젝트에 레벨업
            weaponBase.Condition.Add(WhereAreYouLookinAt.Enum.AttributeType.Exp, remainExp);
        }
    }

    #region 마우스 포인터 관련 로직

    public override void OnDrag(PointerEventData eventData)
    {
        if (playerSlotUI.IsReinforcementMode) return;

        if (item != null)
        {
            // 무기 슬롯 드래그 중에서는 이동할 수 있는 모든 슬롯을 보여준다.
            DragSlot.instance.transform.position = eventData.position;
            playerSlotUI.ItemDragHighlight();
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (playerSlotUI.IsReinforcementMode) return;

        base.OnDrop(eventData);

        // 인벤토리 슬롯에 Advice를 놓으면 미적용
        bool isChange = false;
        if (DragSlot.instance.dragSlot != null)
        {
            ISlotItem item = DragSlot.instance.dragSlot.Item;
            if (item != null)
            {
                // Item Type이 Weapon인지 확인
                if(item.GetType() == WhereAreYouLookinAt.Enum.DropItemType.Weapon)
                {
                    // 자기 슬롯에 놓은것이 아닌지 확인
                    if (DragSlot.instance.dragSlot != this)
                    {
                        // DragSlotType이 임시 인벤토리인지 확인
                        if (DragSlot.instance.dragSlot is TemporarySlot temporarySlot)
                        {
                            SwapTemporarySlot(temporarySlot);
                            return;
                        }

                        isChange = true;
                    }
                }
            }
        }

        if (DragSlot.instance.dragSlot != null && isChange)
        {
            SwapWeaponSlot();
        }
    }

    private void SwapTemporarySlot(TemporarySlot temporarySlot)
    {
        if(this.Item == null)
        {
            AddItem(temporarySlot.Item, Index, false);

            // 임시 인벤토리에서 삭제
            GameManager.Instance.TemporaryInventory.DeleteItem(temporarySlot.Item);
            temporarySlot.DeleteItem(temporarySlot.Item, temporarySlot.Index);
        }
        else
        {
            WeaponSlotData weaponSlotData = GetWeaponData();

            DeleteItem(Item, Index);
            AddItem(temporarySlot.Item, Index, false);

            GameManager.Instance.TemporaryInventory.DeleteItem(temporarySlot.Item);
            temporarySlot.AddItem(weaponSlotData.Item, weaponSlotData.Index, false);
        }

        GameManager.Instance.TemporaryInventory.UpdateLvUI();
    }

    // WeaponSlot Swap
    private void SwapWeaponSlot()
    {
        WeaponSlot sourceSlot = (WeaponSlot)DragSlot.instance.dragSlot;
        WeaponSlot targetSlot = this;

        // WeaponData 추출
        WeaponSlotData sourceData = sourceSlot.GetWeaponData();
        WeaponSlotData targetData = targetSlot.GetWeaponData();

        // WeaponData 적용
        sourceSlot.SetWeaponData(targetData);
        targetSlot.SetWeaponData(sourceData);

        WeaponBase temp = sourceSlot.weaponBase;
        sourceSlot.weaponBase = targetSlot.weaponBase;
        targetSlot.weaponBase = temp;

        // 스왑 정보를 넘기기

        // DetailView 갱신
        if (GameManager.Instance.DetailViewUI)
        {
            // 만약 켜져 있는 상태에서 Swap 시 그 Swap된 Slot으로 다시 Set하기
            if (GameManager.Instance.DetailViewUI.DetailViewWindow.activeSelf)
            {
                GameManager.Instance.DetailViewUI.SetUI(targetSlot.Index);

                // Interact 설정
                playerSlotUI.SetHighlightInteract(targetSlot.Index);
            }
        }

        playerSlotUI.UpdateWeaponSlot();

        player.WeaponManager.SwapWeaponPivot(sourceData.Index, targetData.Index);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        playerSlotUI.HighlightInteractBG(Index);

        if (DragSlot.instance.dragSlot == null)
        {
            if (item == null) return;

            if (weaponBase == null) return;

            // LogInfoUI 켜기
            EventBus.OnMouseOnSlot?.Invoke(item, weaponBase.Condition.Lv, transform.position);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot == null)
        {
            if (item == null) return;

            // LogInfoUI 끄기
            EventBus.OnMouseOffSlot?.Invoke();
        }
    }

    #endregion

    #region 애니메이션 연출

    public void StartSpreadOutIncludeItem(float duration, float durationItem)
    {
        if (spreadCoroutine != null) StopCoroutine(spreadCoroutine);
        spreadCoroutine = StartCoroutine(SpreadOutCoroutine(duration, durationItem));
    }

    private IEnumerator SpreadOutCoroutine(float duration, float durationItem)
    {
        // LevelUp UI 비활성화
        CloseLvUI();

        // 원래 위치 저장
        Vector3 targetPosition = originLocalPosition;

        // 원점에서 출발
        transform.localPosition = Vector3.zero;

        float time = 0.0f;

        // 색상 설정
        float targetBgAlpha = bgColor.a;
        float targetHighlightAlpha = highlightColor.a;
        float targetIconAlpha = iconColor.a;


        while (time < duration)
        {
            time += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, (time / duration));

            float bgAlpha = Mathf.Lerp(0f, targetBgAlpha, (time / duration));
            float highlighAlpha = Mathf.Lerp(0f, targetHighlightAlpha, (time / duration));
            float iconAlpha = Mathf.Lerp(0f, targetIconAlpha, (time / duration));

            SetColorWithChangedAlpha(bg, bgColor, bgAlpha);
            SetColorWithChangedAlpha(highlight, highlightColor, highlighAlpha);
            SetColorWithChangedAlpha(icon, iconColor, iconAlpha);

            yield return null;
        }

        SetColorWithChangedAlpha(bg, bgColor, targetBgAlpha);
        SetColorWithChangedAlpha(highlight, highlightColor, targetHighlightAlpha);
        SetColorWithChangedAlpha(icon, iconColor, targetIconAlpha);

        if (bgColor.a == 0f) Debug.Log("Bg가 0임");

        transform.localPosition = targetPosition;

        // 자기 펄치는 게 끝나면 Item 슬롯 코루틴 함수 작동
        foreach (var slot in itemSlots)
            slot.StartSpreadOut(durationItem);

        yield return new WaitForSeconds(durationItem + 0.01f);

        // LevelUp UI 업데이트
        UpdateLvUI();

        // 강화모드면 강화비용 업데이트
        if (playerSlotUI.IsReinforcementMode)
            UpdateReinforcementCostItemSlot();
    }

    public void StartSpreadInIncludeItem(float duration, float durationItem)
    {
        if (spreadCoroutine != null) StopCoroutine(spreadCoroutine);
        spreadCoroutine = StartCoroutine(SpreadInCoroutine(duration, durationItem));
    }

    private IEnumerator SpreadInCoroutine(float duration, float durationItem)
    {
        // LevelUp UI 비활성화
        CloseLvUI();

        // 원래 위치가 타켓
        Vector3 targetPosition = Vector3.zero;

        float time = 0.0f;

        // 색상 설정
        float originBgAlpha = bgColor.a;
        float originHighlightAlpha = highlightColor.a;
        float originIconAlpha = iconColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);

            float moveT = t;
            float alphaT = Mathf.Clamp01(time / (duration / 3));

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, moveT);

            float bgAlpha = Mathf.Lerp(originBgAlpha, 0f, alphaT);
            float highlighAlpha = Mathf.Lerp(originHighlightAlpha, 0f, alphaT);
            float iconAlpha = Mathf.Lerp(originIconAlpha, 0f, alphaT);

            SetColorWithChangedAlpha(bg, bgColor, bgAlpha);
            SetColorWithChangedAlpha(highlight, highlightColor, highlighAlpha);
            SetColorWithChangedAlpha(icon, iconColor, iconAlpha);

            yield return null;
        }

        SetColorWithChangedAlpha(bg, bgColor, 0f);
        SetColorWithChangedAlpha(highlight, highlightColor, 0f);
        SetColorWithChangedAlpha(icon, iconColor, 0f);

        transform.localPosition = targetPosition;

        // 자기 펄치는 게 끝나면 Item 슬롯 코루틴 함수 작동
        foreach (var slot in itemSlots)
            slot.StartSpreadIn(durationItem);

        yield return new WaitForSeconds(durationItem + 0.01f);

        playerSlotUI.SetPlayerSlotWindow(false);

        // 강화모드면 리셋
        if(playerSlotUI.IsReinforcementMode)
        {
            playerSlotUI.ResetReinforcementCostItemSlot();
        }
    }

    #endregion

    #region 버튼 로직

    public void OnClickWeaponSlot()
    {
        //if (!IsActive) return;

        //if (Item == null) return;

        if (GameManager.Instance.DetailViewUI != null)
        {
            // DetailViewUI 슬롯 열기
            GameManager.Instance.DetailViewUI.SetUI(Index);

            // Interact 설정
            playerSlotUI.SetHighlightInteract(Index);
        }
    }

    #endregion

    #region 프로퍼티

    public List<ItemSlot> ItemSlots { get {  return this.itemSlots; } }
    public WeaponBase WeaponBase { get { return this.weaponBase; } }
    public bool IsHighlightBG { get { return this.isHighlightBG; } }

    #endregion
}
