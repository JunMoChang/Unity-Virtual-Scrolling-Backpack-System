using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemDataSort
{
    [SerializeField] private Dropdown sortTypeDropdown;
    [SerializeField] private Button sortOrderButton;
    [SerializeField] private Text sortOrderText;
    
    
    private ItemSortType currentSortType;
    private bool ascending;

    private InventoryManager inventoryManager;
    public void Initialize()
    {
        inventoryManager = InventoryManager.Instance;
        
        sortOrderText = sortOrderButton.GetComponentInChildren<Text>();
        ascending = true;
        RegisterButton();
    }

    private void RegisterButton()
    {
        sortTypeDropdown.ClearOptions();
        List<string> text = new List<string>
        {
            "默认顺序",
            "按数量",
            "按稀有度",
            "按类型",
            "按名称"
        };
        sortTypeDropdown.AddOptions(text);
        sortTypeDropdown.onValueChanged.AddListener(ChangeSortType);
        
        sortOrderButton.onClick.AddListener(ChangeSortOrder);
    }
    private void ChangeSortType(int index)
    {
        currentSortType = (ItemSortType) index;
        DataListSort();
    }

    private void ChangeSortOrder()
    {
        ascending = !ascending;
        sortOrderText.text = ascending ? "↑升序" : "↓降序";
        DataListSort();
    }
    public void DataListSort()
    {
        ItemDataContainer dataContainer = inventoryManager.GetCurrentDataContainer();
        
        List<ItemData> itemData = dataContainer.itemDataList;
        
        if (itemData.Count <= 1) return;
        
        switch (currentSortType)
        {
            case ItemSortType.None:
                break;
            case ItemSortType.Quantity:
                itemData.Sort((a, b) => ascending ? a.storageItemQuantity.CompareTo(b.storageItemQuantity): b.storageItemQuantity.CompareTo(a.storageItemQuantity));
                break;
            case ItemSortType.Rarity:
                itemData.Sort((a, b) => ascending ? a.itemSo.itemRarity.CompareTo(b.itemSo.itemRarity) : b.itemSo.itemRarity.CompareTo(a.itemSo.itemRarity));
                break;
            case ItemSortType.Type:
                itemData.Sort((a, b) => ascending ? a.itemSo.itemType.CompareTo(b.itemSo.itemType) : b.itemSo.itemType.CompareTo(a.itemSo.itemType));
                break;
            case ItemSortType.Name:
                itemData.Sort((a, b) => ascending ? String.CompareOrdinal(a.itemSo.itemName, b.itemSo.itemName) : b.itemSo.itemName.CompareTo(a));
                break;
        }
        
        inventoryManager.UpdateCurrentDataContainer();
    }
}