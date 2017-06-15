using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WordJam/Inputs/LookAround")]
public class LookAround : InputAction
{
    public override void RespondToInput(string[] separatedInputWords)
    {
        if (separatedInputWords[1] == "around" || separatedInputWords[1] == "room")
        {
            GameController.instance.message("You look around the room");
            GameController.instance.roomNavigation.describeRoom();
            //Reduce obfuscation of the room

            if (GameController.instance.playerInfo.hasItem("torch"))
            {
                MapNode currNode = GameController.instance.roomNavigation.currentNode;
                if (currNode.room != null)
                {
                    foreach (MapNode node in currNode.room.roomnodes)
                    {
                        node.hidden = false;
                    }
                }
            }
        }
    }
}
