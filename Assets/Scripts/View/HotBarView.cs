using Model;
using UnityEngine;

namespace View
{
    public class HotBarView : MonoBehaviour
    {
        [Header("快捷栏格子")]
        [SerializeField] private ItemSlotView[] hotBarSlots;
        
        private HotBarModel hotBarModel;
    
        private void Awake()
        {
            for (int i = 0; i < hotBarSlots.Length; i++)
            {
                hotBarSlots[i].slotType = ItemSlotView.SlotType.HotBar;
                hotBarSlots[i].slotIndex = i;
            }
        }
        
        public void BindModel(HotBarModel model)
        {
            if (hotBarModel != null)
            {
                hotBarModel.OnSlotChanged -= OnSlotChanged;
            }
        
            hotBarModel = model;
        
            if (hotBarModel != null)
            {
                hotBarModel.OnSlotChanged += OnSlotChanged;
            }
        
            RefreshDisplay();
        }
        
        
        private void OnSlotChanged(int slotIndex, ItemDataModel newData)
        {
            if (slotIndex >= 0 && slotIndex < hotBarSlots.Length)
            {
                hotBarSlots[slotIndex].SetData(newData);
            }
        }

        private void RefreshDisplay()
        {
            ItemDataModel[] dataModels = hotBarModel.GetDataModels();
            for (int i = 0; i < hotBarSlots.Length; i++)
            {
                hotBarSlots[i].SetData(dataModels[i]);
            }
        }
    
        public ItemSlotView GetSlotView(int index)
        {
            if (index >= 0 && index < hotBarSlots.Length)
            {
                return hotBarSlots[index];
            }
            return null;
        }
    
        public ItemSlotView[] GetAllSlots()
        {
            return hotBarSlots;
        }
    }
}