using Model;
using ScriptableObjects.Items;
using UnityEngine;
using View;

namespace Controller
{
    public class InventoryController : MonoBehaviour
    {
        public static InventoryController Instance { get; private set; }
        [Header("InventoryView")]
        [SerializeField] private InventoryView inventoryView;
    
        [Header("分类按钮")]
        [SerializeField] private UnityEngine.UI.Button[] categoryButtons;
        
        [Header("排序设置")]
        [SerializeField] private UnityEngine.UI.Dropdown sortTypeDropdown;
        [SerializeField] private UnityEngine.UI.Button sortOrderButton;
        [SerializeField] private UnityEngine.UI.Text sortOrderText;
        private ItemSortType currentSortType;
        private bool ascending;
        
        private InventoryModel inventoryModel;
        private HotBarModel hotBarModel;
    
        private ItemSlotView currentSelectedSlot;

        
        private void Start()
        { 
            if (Instance == null)  Instance = this;
            inventoryView.SetPanelActive();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                inventoryView.SetPanelActive();
            }
        }
        
        public void Initialize(InventoryModel inventoryMod, HotBarModel hotBarMod)
        {
            inventoryModel = inventoryMod;
            hotBarModel = hotBarMod;
            
            inventoryView.BindInventoryModel(inventoryModel);
            inventoryView.BindHotBarModel(hotBarModel);
        
            RegisterCategoryButtons();
            RegisterSortButton();
        }
        
        private void RegisterCategoryButtons()
        {
            for (int i = 0; i < categoryButtons.Length; i++)
            {
                int categoryIndex = i;
                categoryButtons[i].onClick.AddListener(() => OnCategoryButtonClicked((ItemScriptableObject.ItemType)categoryIndex));
            }
        }
        private void RegisterSortButton()
        {
            sortTypeDropdown.ClearOptions();
            System.Collections.Generic.List<string> text = new System.Collections.Generic.List<string>
            {
                "默认顺序",
                "按数量",
                "按稀有度",
                "按类型",
            };
            sortTypeDropdown.AddOptions(text);
            sortTypeDropdown.onValueChanged.AddListener(ChangeSortType);
        
            sortOrderButton.onClick.AddListener(ChangeSortOrder);
        }
        
        public int AddItem(ItemScriptableObject itemSo, int quantity)
        {
            int extra = inventoryModel.AddItem(itemSo, quantity);
            if (extra > 0)
            {
                Debug.Log($"背包已满，剩余 {extra} 个 {itemSo.itemName} 无法添加");
            }
        
            return extra;
        }
        public void UseItem(ItemDataModel item)
        {
            if (item == null || item.IsEmpty()) return;
        
            bool success = inventoryModel.UseItem(item);
            
            Debug.Log(success ? $"使用了 {item.ItemSo?.itemName}" : "该物品无法使用");
        }
        public void DropItem(ItemDataModel item, int quantity = 1)
        {
            if (item == null || item.IsEmpty()) return;
        
            inventoryModel.DropItem(item, quantity);
        }
    
        /// <summary>
        /// 切换物品分类
        /// </summary>
        private void OnCategoryButtonClicked(ItemScriptableObject.ItemType category)
        {
            currentSelectedSlot?.CancelSelected();
            currentSelectedSlot = null;
        
            inventoryView.SwitchCategory(category);
            inventoryView.ClearItemInfo();
        }
        
        private void ChangeSortType(int index)
        {
            currentSortType = (ItemSortType) index;
            SortItem();
        }
        private void ChangeSortOrder()
        {
            ascending = !ascending;
            sortOrderText.text = ascending ? "↑升序" : "↓降序";
            SortItem();
        } 
        private void SortItem()
        {
            inventoryModel.DataListSort(currentSortType, inventoryView.CurrentCategoryType, ascending);
        }
        
        /// <summary>
        /// 设置当前选中的格子
        /// </summary>
        public void SetSelectedSlot(ItemSlotView slotView)
        {
            if (currentSelectedSlot != slotView)
            {
                currentSelectedSlot?.CancelSelected();
            }
        
            currentSelectedSlot = slotView;
            currentSelectedSlot?.SetSelected(true);
            
            if (slotView != null && slotView.currentDataModel != null)
            {
                inventoryView.UpdateDescriptionInfo(slotView.currentDataModel);
            }
            else
            {
                inventoryView.ClearItemInfo();
            }
        }
    }
}