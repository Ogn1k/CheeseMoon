using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "Item")]
public class ItemSO : ScriptableObject
{

    public int itemId = 0;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public int amount;
    
}

[System.Serializable]
public struct Item
{
    public ItemSO item;
    public int amount;
    public GameObject itemObj;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    static public bool operator ==(Item item1, Item item2)
    {
        if (ReferenceEquals(item1.item.name, item2.item.name)) return true;
        return false;
    }

    static public bool operator !=(Item item1, Item item2)
    {
        if (!ReferenceEquals(item1.item.name, item2.item.name)) return true;
        return false;
    }
}