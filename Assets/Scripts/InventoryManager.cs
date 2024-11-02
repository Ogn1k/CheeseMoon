using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//  using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();

    bool craft = false;

    public Transform itemContent;
    public Transform craftContent;
    public GameObject InventoryItem;

    public CraftListSO craftList;
    public GameObject craftItem;

    public void Awake()
    {
        Instance = this; 
    }

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public void Remove(Item item) 
    { 
        Items.Remove(item);
    }

    private void ListItems()
    {
        foreach(Transform item in itemContent)
            Destroy(item.gameObject);
        foreach (var item in Items) 
        { 
            GameObject obj = Instantiate(InventoryItem, itemContent);
            var itemName = obj.transform.GetComponentInChildren<TMP_Text>();
            var itemIcon = obj.transform.GetComponentInChildren<Image>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.itemIcon;
        }
    }

    private void ListCrafts()
    {
        foreach (Transform craft in craftContent)
            Destroy(craft.gameObject);
        foreach (var craft in craftList.craftList) 
        {
            GameObject obj = Instantiate(craftItem, craftContent);
            var craftName = obj.transform.GetComponentInChildren <TMP_Text>();
            var craftIcon = obj.transform.GetComponentInChildren<Image>();

            craftName.text = craft.craftName;
            craftIcon.sprite = craft.craftIcon;
        }
    }

    public void InlistInventory()
    {
        ListItems();
        ListCrafts();
    }

    public void CraftButton()
    {
        craft = !craft;
        if (craft)
        {
            itemContent.gameObject.SetActive(false);
            craftContent.gameObject.SetActive(true);
            gameObject.GetComponent<ScrollRect>().content = (RectTransform)craftContent;
        }
        else
        {
            itemContent.gameObject.SetActive(true);
            craftContent.gameObject.SetActive(false);
            gameObject.GetComponent<ScrollRect>().content = (RectTransform)itemContent;
        }
    }
}
