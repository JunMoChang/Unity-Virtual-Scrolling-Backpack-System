using System;
using System.Collections.Generic;
using ScriptableObjects.Items;
using UnityEngine;

namespace Model
{
    public class InventoryModel
    {
        private readonly Dictionary<ItemScriptableObject.ItemType, List<ItemDataModel>> itemsDataDic;
        private readonly List<ItemDataModel> allItemsCache;
        
        public int DefaultCapacity { get; private set; } = 50;
        private bool isAllItemsCacheChange; 
            
        public event Action<InventoryModel, ItemScriptableObject.ItemType> OnInventoryChanged;
    
        public InventoryModel()
        {
            Array types = Enum.GetValues(typeof(ItemScriptableObject.ItemType));
            
            itemsDataDic = new Dictionary<ItemScriptableObject.ItemType, List<ItemDataModel>>(types.Length - 1);//排除All分类
        
            foreach (ItemScriptableObject.ItemType type in types)
            {
                if(type != ItemScriptableObject.ItemType.All)
                    itemsDataDic.Add(type, new List<ItemDataModel>(DefaultCapacity));
            }
            allItemsCache = new List<ItemDataModel>(DefaultCapacity);
        }
    
        public int AddItem(ItemScriptableObject itemSo, int quantity)
        {
            if (itemSo == null || quantity <= 0) return quantity;
        
            List<ItemDataModel> dataList = itemsDataDic[itemSo.itemType];

            foreach (ItemDataModel data in dataList)
            {
                if (!data.IsFull && data.ItemSo == itemSo)
                {
                    quantity = data.AddQuantity(quantity);
                }
            }
        
            while (quantity > 0 && dataList.Count < DefaultCapacity)
            {
                ItemDataModel newItem = new ItemDataModel();
                quantity = newItem.AddNewData(itemSo, quantity);
            
                dataList.Add(newItem);
                NotifyInventoryChanged(itemSo.itemType);
            }
        
            return quantity;
        }

        public bool UseItem(ItemDataModel item)
        {
            if (item.ItemSo.itemType == ItemScriptableObject.ItemType.Consumable)
            {
                if (item.DecreaseQuantity() <= 0)
                {
                    NotifyInventoryChanged(ItemScriptableObject.ItemType.Consumable);
                    RemoveItem(item);
                    item.ClearData();
                }
                return true;
            }
            return false;
        }

        public bool DropItem(ItemDataModel item, int quantity)
        {
            if (item != null && quantity >= 0)
            {
                if(item.StorageItemQuantity <= quantity) quantity = item.StorageItemQuantity;
            
                CreateItemInWorld(item, quantity);
            
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 获取指定类型的物品集合
        /// </summary>
        public IReadOnlyList<ItemDataModel> GetItemsByType(ItemScriptableObject.ItemType categoryType)
        {
            if (categoryType == ItemScriptableObject.ItemType.All)
            {
                return GetAllItems(); 
            }
            return itemsDataDic[categoryType].AsReadOnly();
        }

        private IReadOnlyList<ItemDataModel> GetAllItems()
        {
            UpdateAllItems();
            
            return allItemsCache.AsReadOnly();
        }

        private void UpdateAllItems()
        {
            if(isAllItemsCacheChange)
            {
                isAllItemsCacheChange =  false;
                allItemsCache.Clear();
                foreach (List<ItemDataModel> typeList in itemsDataDic.Values)
                {
                    allItemsCache.AddRange(typeList);
                }
            }
        }
        
        private void RemoveItem(ItemDataModel item)
        {
            if (itemsDataDic[item.ItemSo.itemType].Remove(item))
            {
                NotifyInventoryChanged(item.ItemSo.itemType);
            }
        }
        
        /// <summary>
        /// 数据排序
        /// </summary>
        /// <param name="sortType">排序类型</param>
        /// <param name="categoryType">数据类型</param>
        /// <param name="ascending">升 | 降序</param>
        public void DataListSort(ItemSortType sortType, ItemScriptableObject.ItemType categoryType, bool ascending = true)
        {
            List<ItemDataModel> items;
            if (categoryType == ItemScriptableObject.ItemType.All)
            {
                UpdateAllItems();
                items = allItemsCache;
            }
            else
            {
                items = itemsDataDic[categoryType];
            }
            
            if (items.Count <= 1) return;
        
            switch (sortType)
            {
                case ItemSortType.None:
                    break;
                case ItemSortType.Quantity:
                    items.Sort((a, b) => ascending ? a.StorageItemQuantity.CompareTo(b.StorageItemQuantity): b.StorageItemQuantity.CompareTo(a.StorageItemQuantity));
                    break;
                case ItemSortType.Rarity:
                    items.Sort((a, b) => ascending ? a.ItemSo.itemRarity.CompareTo(b.ItemSo.itemRarity) : b.ItemSo.itemRarity.CompareTo(a.ItemSo.itemRarity));
                    break;
                case ItemSortType.Type:
                    items.Sort((a, b) => ascending ? a.ItemSo.itemType.CompareTo(b.ItemSo.itemType) : b.ItemSo.itemType.CompareTo(a.ItemSo.itemType));
                    break;
                case ItemSortType.Name:
                    items.Sort((a, b) => ascending ? String.Compare(a.ItemSo.itemName, b.ItemSo.itemName, StringComparison.Ordinal) : String.Compare(b.ItemSo.itemName, a.ItemSo.itemName, StringComparison.Ordinal));
                    break;
            }

            NotifyInventoryChanged(categoryType, false);
        }

        private void CreateItemInWorld(ItemDataModel item, int quantity)
        {
            GameObject newItem = new GameObject(item.ItemSo.itemName);
            Item newItemScr = newItem.AddComponent<Item>();
            newItemScr.itemSo = item.ItemSo;
            newItemScr.quantity = quantity;
        
            SpriteRenderer spriteRenderer = newItem.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.ItemSo.itemSprite;
            newItem.AddComponent<BoxCollider2D>();
            Rigidbody2D rb = newItem.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            newItem.transform.position = GameObject.Find("Player").transform.position + new Vector3(1f, 0, 0);

            int extraQuantity = item.DecreaseQuantity();
            if (extraQuantity <= 0)
            {
                NotifyInventoryChanged(item.ItemSo.itemType);
                RemoveItem(item);
                item.ClearData();
            }
        }
        private void NotifyInventoryChanged(ItemScriptableObject.ItemType itemType, bool isCacheChange = true)
        {
            isAllItemsCacheChange = isCacheChange;
            OnInventoryChanged?.Invoke(this, itemType);
        }
    }
}