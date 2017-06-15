using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInfo
{
    private List<string> inventory = new List<string>();
    private Dictionary<string, UnityAction> inventoryActions = new Dictionary<string, UnityAction>();
    private Dictionary<string, UnityAction> inventoryExamine = new Dictionary<string, UnityAction>();

    void listInventory()
    {
        if (inventory.Count > 0)
        {
            GameController.instance.message("<Current Inventory>");
            foreach (string item in inventory)
            {
                GameController.instance.message(item);
            }
        }
        else
        {
            GameController.instance.message("You have no items in your inventory...");
        }
    }

    public void addInventory(string item, UnityAction ac = null, string examineText = null)
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

        if (inventory.Contains(item) == false)
        {
            UnityAction delUse = delegate ()
            {
                GameController.instance.message("You use the " + item);
            };

            UnityAction delEx = delegate ()
            {
                GameController.instance.message(examineText);
            };

            delUse += ac;

            inventory.Add(item);
            inventoryActions.Add(item, delUse);
            inventoryExamine.Add(item, delEx);

            GameController.StartListening("use " + item, delUse);
            GameController.StartListening("look " + item, delEx);
        }
        else
        {
            inventoryActions[item] += ac;
        }
    }

    public void removeInventory(string item)
    {
        if (inventory.Contains(item))
        {
            GameController.StopListening("use " + item, inventoryActions[item]);
            GameController.StopListening("look " + item, inventoryExamine[item]);

            inventoryActions.Remove(item);
            inventoryExamine.Remove(item);
            inventory.Remove(item);

        }
        else
        {
            Debug.Log("The item does not exist");
        }
    }

    public bool hasItem(string item)
    {
        return inventory.Contains(item);
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
