using Model;
using UnityEngine;
using View;

namespace Controller
{
    public class HotBarController :  MonoBehaviour
    {
        [Header("HotBarView")]
        [SerializeField] private HotBarView hotBarView;
        
        private HotBarModel hotBarModel;

        public void Initialize(HotBarModel hotBarMod)
        {
            hotBarModel = hotBarMod;
            hotBarView.BindModel(hotBarModel); 
        }

        public bool AddToHotBar(ItemDataModel itemDataModel, int slotIndex)
        {
            bool success = hotBarModel.AddToHotBar(itemDataModel, slotIndex);
            return success;
        }

        public bool RemoveFromHotBar(int slotIndex)
        {
            return hotBarModel.RemoveData(slotIndex);
        }
        public bool OnItemDropEnd(ItemSlotView sourceSlot, ItemSlotView targetSlot)
        {
            if (sourceSlot == null || targetSlot == null) return false;
        
            ItemDataModel sourceItem = sourceSlot.currentDataModel;
            if (sourceItem == null || sourceItem.IsEmpty()) return false;
            
            if (sourceSlot.slotType == ItemSlotView.SlotType.HotBar)
            {
                if (targetSlot.slotType == ItemSlotView.SlotType.HotBar)
                {
                    return SwapSlots(sourceSlot.slotIndex, targetSlot.slotIndex);  
                }
                if(targetSlot.slotType == ItemSlotView.SlotType.Inventory)
                {
                    return RemoveFromHotBar(sourceSlot.slotIndex);
                }
            }
            if(sourceSlot.slotType == ItemSlotView.SlotType.Inventory && targetSlot.slotType == ItemSlotView.SlotType.HotBar)
            {
                return AddToHotBar(sourceItem, targetSlot.slotIndex);
            }

            return false;
        }

        private bool SwapSlots(int slotIndex1, int slotIndex2)
        {
            return hotBarModel.SwapSlots(slotIndex1, slotIndex2);
        }
    }
}