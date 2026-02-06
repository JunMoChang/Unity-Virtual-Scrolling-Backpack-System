using System.Collections.Generic;
using ScriptableObjects;
using ScriptableObjects.Items;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDataContainer
{
    public readonly ItemScriptableObject.ItemType itemType;
    public readonly List<ItemData> itemDataList;
    private readonly int defaultDataCapacity = 50;
    public ItemDataContainer(ItemScriptableObject.ItemType type)
    {
        itemDataList =  new List<ItemData>(defaultDataCapacity);
        itemType = type;
    }
    
    public int AddItem(ItemScriptableObject itemSo, int quantity)
    {
        while (quantity > 0)
        {
            bool isFound = false;
            
            foreach (ItemData slotData in itemDataList)
            {
                if (!slotData.isFull && slotData.itemSo.itemName == itemSo.itemName)
                {
                    isFound = true;
                    quantity = slotData.AddData(itemSo, quantity);
                    break;
                }
            }

            if (!isFound)
            {
                if (itemDataList.Count < defaultDataCapacity)
                {
                    ItemData newSlotData = new ItemData();
                    quantity = newSlotData.AddData(itemSo, quantity);
                    itemDataList.Add(newSlotData);
                }
                else
                {
                    Debug.Log($"背包已满，最大存储量{defaultDataCapacity}");
                    break;
                }
            }
        }
        
        return quantity;
    }

    public void RemoveItem(ItemData data)
    {
        itemDataList.Remove(data);
    }
   

    public void SetDataCapacity(int maxCapacity)
    {
        if(maxCapacity < itemDataList.Capacity) return;
        
        itemDataList.Capacity = maxCapacity;
    }
}