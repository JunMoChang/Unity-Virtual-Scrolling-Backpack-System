using System.Collections.Generic;
using ScriptableObjects.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {get; private set;}
    public GameObject inventoryMenu;
    [SerializeField] private SlotsScrollView scrollView;
    [SerializeField] private ItemDataSort slotSort;
    
    [Header("物品信息显示")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private Image itemDescriptionImage;
    
    [SerializeField] private Button[] categoryButtons;
    private readonly Dictionary<ItemScriptableObject.ItemType, ItemDataContainer> dataContainerDic = new();
    private ItemDataContainer allDataContainer;
    
    private ItemSlotView currentSelectedSlot;
    private bool menuActivated;
    
    private ItemScriptableObject.ItemType currentDataContainerType;
    void Awake()
    {
        Instance = this;
        dataContainerDic.Add(ItemScriptableObject.ItemType.Weapon, new ItemDataContainer(ItemScriptableObject.ItemType.Weapon));
        dataContainerDic.Add(ItemScriptableObject.ItemType.Consumable, new ItemDataContainer(ItemScriptableObject.ItemType.Consumable));
        dataContainerDic.Add(ItemScriptableObject.ItemType.Material, new ItemDataContainer(ItemScriptableObject.ItemType.Material));
        dataContainerDic.Add(ItemScriptableObject.ItemType.Other, new ItemDataContainer(ItemScriptableObject.ItemType.Other));
        
        allDataContainer = new ItemDataContainer(ItemScriptableObject.ItemType.All);
        dataContainerDic.Add(ItemScriptableObject.ItemType.All, allDataContainer);
        
        currentDataContainerType = ItemScriptableObject.ItemType. Weapon;
    }

    void Start()
    {
        inventoryMenu.SetActive(false);
        slotSort.Initialize();
        RegisterButtons();
        SwitchCategory(currentDataContainerType);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            menuActivated = !menuActivated;
            inventoryMenu.SetActive(menuActivated);
            
            if (menuActivated)
            {
                List<ItemData> slotsData = GetCurrentDataContainer().itemDataList;
                scrollView.SetData(slotsData, slotsData.Capacity);
            }
        }
    }

    private void RegisterButtons()
    {
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            int index = i;
            categoryButtons[i].onClick.AddListener(() => SwitchCategory((ItemScriptableObject.ItemType)index));
        }
    }
    
    public int AddItem(ItemScriptableObject itemSo, int quantity)
    {
        if (!dataContainerDic.TryGetValue(itemSo.itemType, out ItemDataContainer dataContainer)) return quantity;
        
        int extraItem = dataContainer.AddItem(itemSo, quantity);
        
        UpdateAllContainerReferences();
        if(itemSo.itemType == currentDataContainerType || currentDataContainerType == ItemScriptableObject.ItemType.All) 
            UpdateCurrentDataContainer();
        
        return extraItem;
    }
    
    /// <summary>
    /// 切换数据分类
    /// </summary>
    /// <param name="dataContainerType">数据容器类型</param>
    private void SwitchCategory(ItemScriptableObject.ItemType dataContainerType)
    {
        currentDataContainerType = dataContainerType;
        
        currentSelectedSlot?.CancelSelected();
        UpdateCurrentDataContainer();
    }
    
    /// <summary>
    /// 获取过滤后的显示数据（排除快捷栏中的物品）
    /// </summary>
    /// <param name="sourceItemDataList">原始数据</param>
    /// <returns></returns>
    private List<ItemData> GetFilteredDisplayData(List<ItemData> sourceItemDataList)
    {
        HotBarManager instance = HotBarManager.Instance;
        if(instance == null) return sourceItemDataList;
        
        HashSet<ItemData> hotBarItemData = instance.GetHotBarDataSet();
        if (hotBarItemData.Count <= 0) return sourceItemDataList;
        
        List<ItemData> filteredItemDataList = new List<ItemData>();

        foreach (ItemData sourceItemData in sourceItemDataList)
        {
            if (!hotBarItemData.Contains(sourceItemData))
            {
                filteredItemDataList.Add(sourceItemData);
            }
        }
        
        return filteredItemDataList;
    }
    
    /// <summary>
    /// 更新当前容器显示的数据
    /// </summary>
    public void UpdateCurrentDataContainer()
    {
        if(!dataContainerDic.TryGetValue(currentDataContainerType, out ItemDataContainer dataContainer)) return;
        
        List<ItemData> displayData = GetFilteredDisplayData(dataContainer.itemDataList);
        
        scrollView.SetData(displayData, dataContainer.itemDataList.Capacity);
        
    }
    
    /// <summary>
    /// 更新All容器的数据
    /// </summary>
    private void UpdateAllContainerReferences()
    {
        allDataContainer.itemDataList.Clear();
        
        foreach (ItemDataContainer dataContainer in dataContainerDic.Values)
        {
            if(dataContainer.itemType != ItemScriptableObject.ItemType.All)
            {
                allDataContainer.itemDataList.AddRange(dataContainer.itemDataList);
            }
        }
    }
    
    /// <summary>
    /// 设置当前选中的格子
    /// </summary>
    /// <param name="slotView">选中的格子</param>
    public void SetCurrentSelectedSlot(ItemSlotView slotView)
    {
        if (currentSelectedSlot != slotView)
        {
            currentSelectedSlot?.CancelSelected();
        }
        
        currentSelectedSlot = slotView;
        UpdateDescriptionInfo();
    }
    
    /// <summary>
    /// 更新数据描述UI
    /// </summary>
    private void UpdateDescriptionInfo()
    {
        ItemData itemData = currentSelectedSlot.currentData;
        if (itemData != null && !itemData.IsEmpty())
        {
            ItemScriptableObject itemSo = itemData.itemSo;
            
            itemNameText.text = itemSo.itemName;
            itemDescriptionText.text = itemSo.itemDescription;
            itemDescriptionImage.sprite = itemSo.itemSprite;
        }
        else
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";
            itemDescriptionImage.sprite = null;
        }
    }
    
    /// <summary>
    /// 获取当前显示的数据容器
    /// </summary>
    /// <returns></returns>
    public ItemDataContainer GetCurrentDataContainer()
    {
        return dataContainerDic.GetValueOrDefault(currentDataContainerType);
    }
    
}