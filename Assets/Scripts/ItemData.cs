using ScriptableObjects.Items;

public class ItemData
{
    public ItemScriptableObject itemSo;
    public int storageItemQuantity;
    public bool isFull;
    
    private int itemMaxSuperposition;

    public int AddData(ItemScriptableObject targetItemSo, int quantity)
    {
        if (isFull) return quantity;
        
        itemSo = targetItemSo;
        itemMaxSuperposition = targetItemSo.itemMaxSuperposition;
        storageItemQuantity += quantity;
        
        int extraItems = 0;
        if (storageItemQuantity >= itemMaxSuperposition)
        {
            isFull = true;
            extraItems = storageItemQuantity - itemMaxSuperposition;
            storageItemQuantity = itemMaxSuperposition;
        }
        
        return extraItems;
    }

    public void EmptySlot()
    {
        isFull = false;
        itemMaxSuperposition = 0;
        storageItemQuantity = 0;
        itemSo = null;
    }
    
    public bool IsEmpty()
    {
        return storageItemQuantity <= 0;
    }
}