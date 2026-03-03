using Controller;
using ScriptableObjects;
using ScriptableObjects.Items;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int quantity;
    public ItemScriptableObject itemSo;
    
     [SerializeField] private InventoryController inventoryController;
     
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            int extraItems = inventoryController.AddItem(itemSo, quantity);
            
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
