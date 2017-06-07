using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class InputAction : ScriptableObject {
    
    /// <summary>
    /// The list of keywords you can use for this action
    /// </summary>
    public string[] keywords;

    public bool hasKeyword(string word) {
        for (int i = 0; i < keywords.Length; i++) {
            if (word.ToLower().Equals(keywords[i].ToLower()))
                return true;
        }
        return false;
    }

    public abstract void RespondToInput(GameController controller, string[] separatedInputWords);

}
