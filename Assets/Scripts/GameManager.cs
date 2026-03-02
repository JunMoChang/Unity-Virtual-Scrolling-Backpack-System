using Controller;
using Model;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private HotBarController hotBarController;
    [SerializeField] private ItemDragController itemDragController;
    
    private InventoryModel inventoryModel;
    private HotBarModel hotBarModel;
    void Awake()
    {
        InitializeModels();
        InitializeControllers();
    }

    private void InitializeModels()
    {
        inventoryModel = new InventoryModel();
        hotBarModel = new HotBarModel();
    }
    private void InitializeControllers()
    {
        inventoryController.Initialize(inventoryModel, hotBarModel);
        hotBarController.Initialize(hotBarModel);
        itemDragController.Initialize(hotBarController);
        ItemInteractionController.Initialize(inventoryController);
    }
}
