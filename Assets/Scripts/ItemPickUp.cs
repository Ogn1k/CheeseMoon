using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemSO item;

    void Pickup()
    {
        InventoryManager.Instance.Add(item, 1); 
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        Pickup();
    }

}
