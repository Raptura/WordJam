using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInfo
{
    public delegate void ItemAction();
    private Dictionary<string, ItemAction> inventory = new Dictionary<string, ItemAction>();
    private Dictionary<string, string> inventoryDescriptions = new Dictionary<string, string>(); //What is said about each item when you use examine

    void listInventory()
    {
        if (inventory.Count > 0)
        {
            GameController.instance.message("<Current Inventory>");
            foreach (string item in inventory.Keys)
            {
                GameController.instance.message(item);
            }
        }
        else
        {
            GameController.instance.message("You have no items in your inventory...");
        }
    }

    public void addInventory(string item, ItemAction ac = null, string examineText = null)
    {
        item = item.ToLower();

        if (ac == null)
        {
            ac = delegate () { }; //intentionally blank delegate
        }

        if (examineText == null)
        {
            examineText = "It is a " + item + ", such an elegant design";
        }

        if (inventory.ContainsKey(item) == false)
        {
            ItemAction delUse = delegate ()
            {
                GameController.instance.message("You use the " + item);
            };

            ItemAction delEx = delegate ()
            {
                GameController.instance.message(examineText);
            };

            delUse += ac;

            inventory.Add(item, delUse);
            GameController.StartListening("use " + item, new UnityAction(delUse));
            GameController.StartListening("look " + item, new UnityAction(delEx));
        }
        else
        {
            inventory[item] += ac;
        }
    }

    public void removeInventory(string item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory.Remove(item);
            inventoryDescriptions.Remove(item);
        }
        else
        {
            Debug.Log("The item does not exist");
        }
    }

    public bool hasItem(string item)
    {
        return inventory.ContainsKey(item);
    }

    public PlayerInfo()
    {
        GameController.StartListening("inventory", new UnityAction(listInventory));
    }

    //ITEMS

    public void addTorch()
    {
        addInventory("torch", delegate ()
        {
            //reduce obfuscation
        });
    }
}
