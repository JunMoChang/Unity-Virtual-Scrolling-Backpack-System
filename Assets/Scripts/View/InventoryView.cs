using System.Collections.Generic;
using Model;
using ScriptableObjects.Items;
using UnityEngine;

namespace View
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryMenu; 
        [SerializeField] private SlotsScrollView scrollView;
        
        private InventoryModel inventoryModel;
        private HotBarModel hotBarModel;
    
        [Header("物品信息显示")]
        [SerializeField] private TMPro.TMP_Text itemNameText;
        [SerializeField] private TMPro.TMP_Text itemDescriptionText;
        [SerializeField] private UnityEngine.UI.Image itemDescriptionImage;
        
        public ItemScriptableObject.ItemType CurrentCategoryType { get; private set; }
    
        /// <summary>
        /// 绑定背包数据模型
        /// </summary>
        public void BindInventoryModel(InventoryModel model)
        {
            if (inventoryModel != null)
            {
                inventoryModel.OnInventoryChanged -= OnInventoryChanged;
            }
        
            inventoryModel = model;
            if (inventoryModel != null)
            {
                inventoryModel.OnInventoryChanged += OnInventoryChanged;
            }
        
            RefreshDisplay();
        }

        public void BindHotBarModel(HotBarModel model)
        {
            if (hotBarModel != null)
            {
                hotBarModel.OnSlotChanged -= OnHotBarChanged;
            }
            
            hotBarModel = model;
            if (hotBarModel != null)
            {
                hotBarModel.OnSlotChanged += OnHotBarChanged;
            }
        }
        private void OnInventoryChanged(InventoryModel model, ItemScriptableObject.ItemType categoryType)
        {
            if (categoryType != CurrentCategoryType && CurrentCategoryType != ItemScriptableObject.ItemType.All) return;
            
            RefreshDisplay();
        }
        private void OnHotBarChanged(int index, ItemDataModel dataModel)
        {
            RefreshDisplay();
        }
        private void RefreshDisplay()
        {
            IReadOnlyList<ItemDataModel> items = inventoryModel.GetItemsByType(CurrentCategoryType);
            IReadOnlyList<ItemDataModel> filteredItems = FilterHotBarItems(items);
                        
            scrollView.SetData(filteredItems, inventoryModel.GetItemCapacity(CurrentCategoryType));
        }
    
        /// <summary>
        /// 获取过滤快捷栏的数据
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private IReadOnlyList<ItemDataModel> FilterHotBarItems(IReadOnlyList<ItemDataModel> items)
        {
            if (hotBarModel == null) return items;
        
            HashSet<ItemDataModel> hotBarItems = hotBarModel.DataSet;
            if (hotBarItems.Count == 0) return items;
        
            List<ItemDataModel> filtered = new ();
        
            foreach (var item in items)
            {
                if (!hotBarItems.Contains(item))
                {
                    filtered.Add(item);
                }
            }
        
            return filtered;
        }
    
        /// <summary>
        /// 切换显示的物品分类
        /// </summary>
        /// <param name="categoryType">物品分类类型</param>
        public void SwitchCategory(ItemScriptableObject.ItemType categoryType)
        {
            CurrentCategoryType = categoryType; 
            RefreshDisplay();
        }
    
        /// <summary>
        /// 清空物品详情
        /// </summary>
        public void ClearItemInfo()
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";
            itemDescriptionImage.sprite = null;
        }
    
        public void UpdateDescriptionInfo(ItemDataModel currentDataModel)
        {
            if (currentDataModel == null || currentDataModel.IsEmpty())
            {
                ClearItemInfo();
                return;
            }
        
            itemNameText.text = currentDataModel.ItemSo.itemName;
            itemDescriptionText.text = currentDataModel.ItemSo.itemDescription;
            itemDescriptionImage.sprite = currentDataModel.ItemSo.itemSprite;
        }
    
        /// <summary>
        /// 显示/隐藏背包面板
        /// </summary>
        public void SetPanelActive()
        {
            bool active = inventoryMenu.activeInHierarchy;
            inventoryMenu.SetActive(!active);
        
            if (active)
            {
                RefreshDisplay();
            }
        }
    }
}