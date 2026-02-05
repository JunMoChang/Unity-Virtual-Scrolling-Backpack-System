using System.Collections.Generic;
using UnityEngine;

public class HotBarManager : MonoBehaviour
{
    public static HotBarManager Instance { get; private set; }
    private readonly int hotBarSlotCount = 8;
    private List<ItemData> hotBarItemData;
    private HashSet<ItemData> hotBarItemDataSet; 
    [SerializeField] private ItemSlotView[] hotBarSlots;
    void Awake()
    {
        Instance = this;
        
        hotBarItemData = new List<ItemData>(hotBarSlotCount);
        for (int i = 0; i < hotBarSlotCount; i++)
        {
            hotBarItemData.Add(null);
        }
        hotBarItemDataSet = new HashSet<ItemData>(hotBarSlotCount);
    }

    void Start()
    {
        RefreshDisplay();
    }
    /// <summary>
    /// 添加物品到快捷栏
    /// </summary>
    /// <param name="itemData">添加的物品</param>
    /// <param name="slotIndex">添加到快捷栏的索引</param>
    /// <returns></returns>
    public bool AddToHotBar(ItemData itemData, int slotIndex)
    {
        if(slotIndex < 0 || slotIndex >= hotBarSlotCount) return false;
        
        if(itemData == null || itemData.IsEmpty()) return false;
        
        int existingSlotIndex = hotBarItemData.IndexOf(itemData);
        if (existingSlotIndex >= 0 && existingSlotIndex != slotIndex)
        {
            hotBarItemData[existingSlotIndex] = null;
        }
        else
        {
            hotBarItemDataSet.Add(itemData);
        }
        
        hotBarItemData[slotIndex] = itemData;
        
        InventoryManager.Instance?.UpdateCurrentDataContainer();
        return true;
    }
    
    /// <summary>
    /// AddToHotBar的重载，支持当原数据来自快捷栏时，交换两个快捷栏的数据
    /// </summary>
    /// <param name="sourceSlot">原快捷栏</param>
    /// <param name="hotBarSlot">目标快捷栏</param>
    public bool AddToHotBar(ItemSlotView sourceSlot, ItemSlotView hotBarSlot)
    {
        if (sourceSlot.slotType == ItemSlotView.SlotType.HotBar)
        {
            SwapItem(sourceSlot.currentData, sourceSlot.hotBarIndex, hotBarSlot.hotBarIndex);
            return true;
        }
        
        return AddToHotBar(sourceSlot.currentData, hotBarSlot.hotBarIndex);
    }
    
    /// <summary>
    /// 交换两个快捷栏的数据
    /// </summary>
    /// <param name="itemData">原数据</param>
    /// <param name="slotIndex1">原快捷栏索引</param>
    /// <param name="slotIndex2">目标快捷栏索引</param>
    private void SwapItem(ItemData itemData, int slotIndex1, int slotIndex2)
    {
        hotBarItemData[slotIndex1] = hotBarItemData[slotIndex2];
        hotBarItemData[slotIndex2] = itemData;
        RefreshDisplay();
    }
    
    /// <summary>
    /// 移除快捷栏中的物品
    /// </summary>
    /// <param name="slotIndex">物品在快捷栏中的索引</param>
    public void RemoveFromHotBar(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotBarSlotCount) return;
        
        hotBarItemDataSet.Remove(hotBarItemData[slotIndex]);
        hotBarItemData[slotIndex] = null;
        
        InventoryManager.Instance?.UpdateCurrentDataContainer();
        
        RefreshDisplay();
    }
    
    /// <summary>
    /// 获取快捷栏中已有的物品数据
    /// </summary>
    /// <returns></returns>
    public HashSet<ItemData> GetHotBarDataSet()
    {
        return hotBarItemDataSet;
    }
    
    /// <summary>
    /// 获取快捷栏的全部物品数据
    /// </summary>
    /// <returns></returns>
    public List<ItemData> GetAllHotBarData()
    {
        return hotBarItemData;
    }
    
    public void RefreshDisplay()
    {
        if (HotBarManager.Instance == null) return;
        
        List<ItemData> hotBarItems = HotBarManager.Instance.GetAllHotBarData();
        
        for (int i = 0; i < hotBarSlots.Length; i++)
        {
            if (hotBarSlots[i] == null) continue;
            
            if (i < hotBarItems.Count)
            {
                hotBarSlots[i].SetData(hotBarItems[i]);
            }
            else
            {
                hotBarSlots[i].ClearData();
            }
        }
    }
}