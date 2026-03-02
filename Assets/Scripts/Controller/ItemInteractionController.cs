using UnityEngine;
using UnityEngine.EventSystems;

namespace Controller
{
    public class ItemInteractionController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private View.ItemSlotView slotView;
        
        private static InventoryController inventoryController;
        void Start()
        {
            slotView = GetComponent<View.ItemSlotView>();
        }

        public static void Initialize(InventoryController inventoryCon)
        {
            inventoryController = inventoryCon;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (slotView.currentDataModel == null || slotView.currentDataModel.IsEmpty()) return;
        
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick();
            }
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(slotView.currentDataModel == null || slotView.currentDataModel.IsEmpty()) return;
            
            ItemDragController.Instance.OnBeginDrag(slotView, eventData);
        }
        public void OnDrag(PointerEventData eventData)
        {
            ItemDragController.Instance.OnDrag(eventData);
        }                                 
        public void OnEndDrag(PointerEventData eventData)
        {
            ItemDragController.Instance.OnEndDrag(eventData);
        }
    
        private void OnLeftClick()
        {
            if (slotView.isSelected)
            {
                inventoryController.UseItem(slotView.currentDataModel);
            }
            else
            {
                inventoryController.SetSelectedSlot(slotView);
            }
        }
        private void OnRightClick()
        {
            if (slotView.isSelected)
            {
                inventoryController.DropItem(slotView.currentDataModel);
            }
        }
    }
} 