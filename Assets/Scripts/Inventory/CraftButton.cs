using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CraftButton : MonoBehaviour
{
    Button craftButton;
    public TMP_Text craftText;
    public Image craftImage;
    
    CraftSO craft;

    private void Start()
    {
        craftButton = GetComponentInChildren<Button>();
        //craftText = GetComponentInChildren<TMP_Text>();
        //craftImage = GetComponentInChildren<Image>();

        
    }

    public void SetCraft(CraftSO _craft)
    { 
        craftText.text = _craft.craftName;
        craftImage.sprite = _craft.craftIcon;
        craft = _craft;
    }

    public void TryCraft()
    {
        foreach (var part in craft.craft)
        {
            for(int i=0; i < InventoryManager.Instance.Items.Count; i++)
            {
                List<Item> inventory = InventoryManager.Instance.Items;
                if (inventory[i].item == part.item && part.amount <= inventory[i].amount)
                {
                    print("good!");
                    InventoryManager.Instance.Remove(part.item, part.amount);
                    InventoryManager.Instance.Add(craft.result, craft.amount);
                }
            }
        }
    }
}
