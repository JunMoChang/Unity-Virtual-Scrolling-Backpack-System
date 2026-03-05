using Controller;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int quantity;
    public ScriptableObjects.Items.ItemScriptableObject itemSo;
    
     private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            int extraItems = InventoryController.Instance.AddItem(itemSo, quantity);
            
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
