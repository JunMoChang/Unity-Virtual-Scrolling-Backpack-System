using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class SlotsScrollView : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private RectTransform contentRect;

        [SerializeField] private int columnCount = 5;
        public Vector2 cellSize = new (150, 150);
        public Vector2 spacing = new (10, 10);
        public Vector2 padding = new (10, 10);
    
        private readonly List<ItemSlotView> slotPool = new();
        private IReadOnlyList<ItemDataModel> displayDaraList;
    
        private int poolSize;
        private float cellHeight;
        private float cellWidth;
    
        private int lastStartRow = -1;

        void Awake()
        {
            contentRect = scrollRect.content;
            scrollRect.onValueChanged.AddListener(OnScroll);
        
            Initialize();
        }   
    
        private void Initialize()
        {
            cellHeight = cellSize.y + spacing.y;
            cellWidth =  cellSize.x + spacing.x;
        
            poolSize = (Mathf.CeilToInt(scrollRect.viewport.rect.height / cellHeight) + 1) * columnCount ;
            for (int i = 0; i < poolSize; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, contentRect);
                RectTransform rt = slotObj.GetComponent<RectTransform>();

                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.sizeDelta = cellSize;
            
                ItemSlotView slotView = slotObj.GetComponent<ItemSlotView>();
                slotPool.Add(slotView);
                DisplaySingleSlot(slotView, i);
            }
        }
    
        public void SetData(IReadOnlyList<ItemDataModel> currentData, int dataContainerCapacity)
        {
            displayDaraList = currentData;
        
            SetContentSize(dataContainerCapacity);
        
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
            lastStartRow = -1;
        
            RefreshVisibleSlots();
        }
    
        private void RefreshVisibleSlots()
        {
            if (!gameObject.activeInHierarchy && slotPool.Count == 0) return;

            float scrollY = contentRect.anchoredPosition.y; 
            int startRow = Mathf.FloorToInt(scrollY / cellHeight);
        
            if (startRow == lastStartRow && scrollY != 0) return;
        
            int endRow = Mathf.CeilToInt((scrollY + scrollRect.viewport.rect.height) / cellHeight);

            lastStartRow = startRow;
            int startIndex = startRow * columnCount;
            for (int i = 0; i < slotPool.Count; i++)
            {
                int dataIndex = startIndex + i;
                ItemSlotView slotView = slotPool[i];
            
                int dataRow = dataIndex / columnCount;
            
                DisplaySingleSlot(slotView, dataIndex);
                bool isInViewport = dataRow < endRow;
                if (isInViewport)
                {
                    slotView.gameObject.SetActive(true);
                
                    slotView.SetData(dataIndex < displayDaraList.Count ? displayDaraList[dataIndex] : null);
                }
                else
                {
                    slotView.gameObject.SetActive(false);
                }
            }
        }

        private void OnScroll(Vector2 scrollPos)
        {
            RefreshVisibleSlots();
        }
    
        private void SetContentSize(int count)
        {
            int rowCount = Mathf.CeilToInt((float)count / columnCount);
            float height = rowCount * cellHeight;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, height);
        }
    
        private void DisplaySingleSlot(ItemSlotView slotView, int currentIndex)
        {
            RectTransform rt = slotView.RectTransform;
        
            int row = currentIndex / columnCount;
            int col = currentIndex % columnCount;
            float x = cellWidth * col +  padding.x;
            float y = -cellHeight * row;
        
            rt.anchoredPosition = new Vector2(x, y);
        
            slotView.gameObject.SetActive(true);
        }
    
    }
}