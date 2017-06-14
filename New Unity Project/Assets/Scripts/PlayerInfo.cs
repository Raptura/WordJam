using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInfo
{
    public delegate void ItemAction();
    private Dictionary<string, ItemAction> inventory = new Dictionary<string, ItemAction>();

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

    public void addInventory(string item, ItemAction ac = null)
    {
        item = item.ToLower();

        if (ac == null)
        {
            ac = delegate () { }; //intentionally blank delegate
        }

        if (inventory.ContainsKey(item) == false)
        {
            ItemAction del = delegate ()
            {
                GameController.instance.message("You use the " + item);
            };

            del += ac;

            inventory.Add(item, del);
            GameController.StartListening("use " + item, new UnityAction(del));
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
        }
        else
        {
            Debug.Log("The item does not exist");
        }
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
