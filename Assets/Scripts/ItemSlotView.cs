using System;
using ScriptableObjects.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler,  IEndDragHandler
{
    [Header("UI组件")]
    [SerializeField] private TMP_Text itemQuantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] public GameObject selectedPanel;
    public Sprite defaultItemSlotSprite;
    public RectTransform rectTransform; 
    
    [Header("槽位类型")]
    [SerializeField] public SlotType slotType = SlotType.Inventory;
    [Range(0,7)] public int hotBarIndex = -1;
    
    private bool isSelected;
    [NonSerialized] public ItemData currentData;
    
    private InventoryManager inventoryManager;

    /// <summary>
    /// 槽位类型
    /// </summary>
    public enum SlotType
    {
        Inventory,
        HotBar
    }
    void Awake()
    {
        inventoryManager = InventoryManager.Instance;
    }
    
    public void SetData(ItemData data)
    {
        currentData = data;
        UpdateDataDisplay();
    }
    public void ClearData()
    {
        currentData = null;
        UpdateDataDisplay();
    }
    
    private void UpdateDataDisplay()
    {
        if (currentData == null || currentData.IsEmpty())
        {
            itemImage.sprite = defaultItemSlotSprite;
            itemQuantityText.enabled = false;
            CancelSelected();
        }
        else
        {
            itemImage.sprite = currentData.itemSo.itemSprite;
            itemQuantityText.text = currentData.storageItemQuantity.ToString();
            itemQuantityText.enabled = true;
            selectedPanel.SetActive(isSelected);
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentData != null && !currentData.IsEmpty())
        {
            ItemDragHandler.Instance.OnBeginDrag(this, eventData);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        ItemDragHandler.Instance.OnDrag(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        ItemDragHandler.Instance.OnEndDrag(eventData);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentData == null || currentData.IsEmpty()) return;
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }
    private void OnLeftClick()
    {
        if (isSelected)
        {
            if (!UseItem()) return;
        }
        else if(!currentData.IsEmpty())
        {
            isSelected = true;
        }
        
        UpdateDataDisplay();
        inventoryManager.SetCurrentSelectedSlot(this);
    }
    private void OnRightClick()
    {
        if (!isSelected || currentData.IsEmpty()) return;
        
        GameObject item = new GameObject(currentData.itemSo.itemName);
        Item newItemScr = item.AddComponent<Item>();
        newItemScr.itemSo = currentData.itemSo;
        newItemScr.quantity = 1;
        
        SpriteRenderer spriteRenderer = item.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = currentData.itemSo.itemSprite;
        item.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = item.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        item.transform.position = GameObject.Find("Player").transform.position + new Vector3(1f, 0, 0);
        
        currentData.storageItemQuantity -= 1;
        currentData.isFull = false;
        if (currentData.storageItemQuantity <= 0)
        {
            InventoryManager.Instance.GetDataContainer(currentData.itemSo.itemType).RemoveItem(currentData);
            
            currentData.EmptySlot();
            currentData = null;
        }
        
        UpdateDataDisplay();
        inventoryManager.UpdateCurrentDataContainer();
    }

    public void CancelSelected()
    {
        isSelected = false;
        selectedPanel.SetActive(false);
    }

    private bool UseItem()
    {
        bool success = currentData.itemSo.itemType == ItemScriptableObject.ItemType.Consumable;
        
        if (success)
        {
            Debug.Log($"使用{currentData.itemSo.itemName}");
            currentData.storageItemQuantity -= 1;
            currentData.isFull = false;
            if (currentData.storageItemQuantity <= 0)
            {
                InventoryManager.Instance.GetDataContainer(currentData.itemSo.itemType).RemoveItem(currentData);
                currentData.EmptySlot();
                currentData = null;
            }
        }
        return success;
    }
}