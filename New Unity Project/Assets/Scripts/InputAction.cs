using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class InputAction : ScriptableObject
{

    //The key used for the messaging system
    public string key;
    /// <summary>
    /// The list of keywords you can use for this action
    /// </summary>
    public string[] keywords;

    public bool hasKeyword(string word)
    {
        for (int i = 0; i < keywords.Length; i++)
        {
            if (word.ToLower().Equals(keywords[i].ToLower()))
                return true;
        }
        return false;
    }

    public void invokeInputAction(string[] separatedInputWords)
    {
        string data = "";
        for (int i = 1; i < separatedInputWords.Length; i++)
        {
            data += " ";
            data += separatedInputWords[i];
        }

        GameController.TriggerEvent(key + data);
        RespondToInput(separatedInputWords);
    }

    public abstract void RespondToInput(string[] separatedInputWords);

}
