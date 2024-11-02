using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "Item")]
public class Item : ScriptableObject
{

    public int itemId = 0;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;

}