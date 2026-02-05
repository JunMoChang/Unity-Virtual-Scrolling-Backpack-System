using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private List<ItemData> displayDaraList;
    
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

    private void Start()
    {
        RefreshVisibleSlots();
    }

    private void Initialize()
    {
        cellHeight = cellSize.y + spacing.y;
        cellWidth =  cellSize.x + spacing.x;
        
        poolSize = (Mathf.CeilToInt(contentRect.rect.height / cellHeight) + 1) * columnCount ;
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
        }
    }
    
    public void SetData(List<ItemData> currentData, int dataContainerCapacity)
    {
        displayDaraList = currentData ;
        
        SetContentSize(dataContainerCapacity);
        
        contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
        lastStartRow = -1;
        RefreshVisibleSlots();
    }
    
    private void RefreshVisibleSlots()
    {
        if (!gameObject.activeInHierarchy || slotPool.Count == 0) return;

        float scrollY = contentRect.anchoredPosition.y; 
        int startRow = Mathf.FloorToInt(Mathf.Max(0, scrollY) / cellHeight);
        
        if (startRow == lastStartRow) return;
        
        lastStartRow = startRow;
        int startIndex = startRow * columnCount;
        for (int i = 0; i < slotPool.Count; i++)
        {
            int displayIndex = startIndex + i;
            ItemSlotView slotView = slotPool[i];
            DisplaySingleSlot(slotView, displayIndex);
            if (displayIndex >= 0 && displayIndex < displayDaraList.Count)
            {
                slotView.SetData(displayDaraList[displayIndex]);
            }
            else
            {
                slotView.ClearData();
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
        RectTransform rt = slotView.rectTransform;
        
        int row = currentIndex / columnCount;
        int col = currentIndex % columnCount;
        float x = cellWidth * col +  padding.x;
        float y = -cellHeight * row;
        
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = cellSize;
        slotView.gameObject.SetActive(true);
    }
    
}