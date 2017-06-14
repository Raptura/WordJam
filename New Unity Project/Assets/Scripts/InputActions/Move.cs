using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WordJam/Inputs/Move")]
public class Move : InputAction
{
    public override void RespondToInput(string[] separatedInputWords)
    {
        if (separatedInputWords.Length > 1)
            GameController.instance.roomNavigation.AttemptToChangeNodes(separatedInputWords[1]);
    }
}
