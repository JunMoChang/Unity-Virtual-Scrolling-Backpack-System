using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler :  MonoBehaviour
{
    public static ItemDragHandler Instance{get; private set;}
    
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject dragItemPrefab;
    
    private ItemSlotView currentDragSlot;
    private Image dragItemImage;
    private RectTransform dragItemRectTransform;
    
    void Awake()
    {
        Instance = this;
    }
    
    public void OnBeginDrag(ItemSlotView slotView, PointerEventData eventData)
    {
        currentDragSlot = slotView;
        
        CreateDragImage(slotView);
        UpdateDragItemPosition(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        UpdateDragItemPosition(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        ItemSlotView targetSlot = GetHotBarSlotUnderMouse(eventData);
        
        if (currentDragSlot != null && targetSlot != null)
        {
            switch (targetSlot.slotType)
            {
                case ItemSlotView.SlotType.HotBar:
                {
                    bool success = HotBarManager.Instance.AddToHotBar(currentDragSlot, targetSlot);

                    if (success) HotBarManager.Instance.RefreshDisplay();
                    
                    break;
                }
                case ItemSlotView.SlotType.Inventory:
                    if (currentDragSlot.slotType == ItemSlotView.SlotType.HotBar)
                        HotBarManager.Instance.RemoveFromHotBar(currentDragSlot.hotBarIndex);
                    break;
            }
        }
        
        if (dragItemRectTransform != null)
        {
            Destroy(dragItemRectTransform.gameObject);
            dragItemRectTransform = null;
            dragItemImage = null;
        }
        
        currentDragSlot = null;
    }
    
    /// <summary>
    /// 创建拖拽时的图像
    /// </summary>
    /// <param name="itemSlot"></param>
    private void CreateDragImage(ItemSlotView itemSlot)
    {
        GameObject dragItem = Instantiate(dragItemPrefab, canvas.transform);
        
        dragItemImage = dragItem.GetComponent<Image>();
        dragItemRectTransform = dragItem.GetComponent<RectTransform>();
        
        dragItemImage.sprite = itemSlot.currentData.itemSo.itemSprite;
        dragItemImage.raycastTarget = false;
    }
    
    /// <summary>
    /// 更新拖拽时图像的位置
    /// </summary>
    /// <param name="eventData"></param>
    private void UpdateDragItemPosition(PointerEventData eventData)
    {
        if(dragItemRectTransform == null) return;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector2 localPoint);
        
        dragItemRectTransform.localPosition = localPoint;
    }
    
    /// <summary>
    /// 获取距离鼠标最近格子
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    private ItemSlotView GetHotBarSlotUnderMouse(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent(out ItemSlotView itemSlot))
            {
                return  itemSlot;
            }
        }
        
        return null;
    }
}