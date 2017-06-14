using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WordJam/Inputs/LookAround")]
public class LookAround : InputAction
{
    public override void RespondToInput(string[] separatedInputWords)
    {
        GameController.instance.message("You look around the room");
        GameController.instance.roomNavigation.describeRoom();
        //Reduce obfuscation of the room
    }
}
