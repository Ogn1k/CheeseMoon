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

    private void Start()
    {
        foreach (var craft in craftList.craftList)
        {
            GameObject obj = Instantiate(craftItem, craftContent);

            obj.GetComponent<CraftButton>().SetCraft(craft);
        }
    }

    public void Add(ItemSO item, int amount)
    {
        

        bool isInInv = false;
        for(int i = 0; i<Items.Count;i++) 
        {
            if (Items[i].item == item) 
            { 

                isInInv = true;
                Item temp = Items[i];
                temp.amount++;
                Items[i] = temp;
                Items[i].itemObj.transform.Find("amount").GetComponent<TMP_Text>().text = temp.amount.ToString();
            
            }   
        }

        if(!isInInv) 
        { 
            


            GameObject obj = Instantiate(InventoryItem, itemContent);
            var itemName = obj.transform.GetComponentInChildren<TMP_Text>();
            var itemAmount = obj.transform.Find("amount").GetComponent<TMP_Text>();
            var itemIcon = obj.transform.GetComponentInChildren<Image>();

            Items.Add(new Item { amount = amount, item = item, itemObj = obj });

            itemName.text = item.itemName;
            itemAmount.text = item.amount.ToString();
            itemIcon.sprite = item.itemIcon;
        }
        
    }

    public void Remove(ItemSO _item, int amount) 
    { 

        for(int i=0; i<Items.Count;i++)
        {
            if (Items[i].item == _item)
            {
                if(Items[i].amount < amount)
                { 
                    print("error here");
                    break;
                }
                Item temp = Items[i];
                temp.amount = temp.amount - amount;
                Items[i] = temp;
                Items[i].itemObj.transform.Find("amount").GetComponent<TMP_Text>().text = temp.amount.ToString();
            }
        }
        
    }

/*    private void ListCrafts()
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
    }*/

/*    public void InlistInventory()
    {
        ListCrafts();
    }*/

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
