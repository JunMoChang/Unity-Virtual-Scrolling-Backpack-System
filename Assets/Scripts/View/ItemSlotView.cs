using Model;
using UnityEngine;

namespace View
{
    public class ItemSlotView : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private TMPro.TMP_Text itemQuantityText;
        [SerializeField] private UnityEngine.UI.Image itemImage;
        [SerializeField] private GameObject selectedPanel;
        [SerializeField] private Sprite defaultItemSlotSprite;
        public RectTransform RectTransform { get; private set; } 
    
        [Header("槽位类型")]
        [SerializeField] public SlotType slotType = SlotType.Inventory;
        [Range(0,7)] public int slotIndex = -1;
    
        public bool isSelected;
        [System.NonSerialized] public ItemDataModel currentDataModel;
        
        public enum SlotType
        {
            Inventory,
            HotBar
        }
        
        void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }
        
        public void SetData(ItemDataModel dataModel)
        {
            if (currentDataModel != null)
            {
                currentDataModel.OnDataChanged -= OnModelDataChanged;
            }
        
            currentDataModel = dataModel;
            
            if (currentDataModel != null)
            {
                currentDataModel.OnDataChanged += OnModelDataChanged;
            }
            
            UpdateDataDisplay();
        }
        
        private void OnModelDataChanged(ItemDataModel dataModel)
        {
            UpdateDataDisplay();
        }
        private void UpdateDataDisplay()
        {
            if (currentDataModel == null || currentDataModel.IsEmpty())
            {
                itemImage.sprite = defaultItemSlotSprite;
                itemQuantityText.enabled = false;
                CancelSelected();
            }
            else
            {
                itemImage.sprite = currentDataModel.ItemSo.itemSprite;
                itemQuantityText.text = currentDataModel.StorageItemQuantity.ToString();
                itemQuantityText.enabled = true;
                selectedPanel.SetActive(isSelected);
            }
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            selectedPanel.SetActive(selected);
        }
        public void CancelSelected()
        {
            isSelected = false;
            selectedPanel.SetActive(false);
        }
    }
}