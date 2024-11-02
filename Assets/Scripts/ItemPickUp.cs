using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;

    void Pickup()
    {
        InventoryManager.Instance.Add(item);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        Pickup();
    }

}
