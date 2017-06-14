using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WordJam/Inputs/Test")]
public class Test : InputAction
{
    public override void RespondToInput(string[] separatedInputWords)
    {
        GameController.TriggerEvent("testSuccess");
        //GameController.TriggerEvent("testFail");
    }
}
