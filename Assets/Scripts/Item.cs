using ScriptableObjects;
using ScriptableObjects.Items;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int quantity;
    public ItemScriptableObject itemSo;
    
    private InventoryManager inventoryManager;
    void Start()
    {
        inventoryManager =  GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            int extraItems = inventoryManager.AddItem(itemSo, quantity);
            
            if(extraItems > 0)
            {
                quantity = extraItems;
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
    }
}
