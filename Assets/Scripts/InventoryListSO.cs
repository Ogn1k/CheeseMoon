using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
public class InventoryListSO : ScriptableObject
{
    public List<Item> Items = new List<Item>();
}
