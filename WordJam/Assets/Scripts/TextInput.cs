using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour
{

    public InputField inputField;

    GameController controller;

    void Awake()
    {
        controller = GameController.instance;
        inputField.onEndEdit.AddListener(AcceptStringInput);
    }

    void AcceptStringInput(string userInput)
    {
        userInput = userInput.ToLower().Trim();
        controller.message(">> " + userInput);

        char[] delimiterCharacters = { ' ' };

        string[] ignoreWords = {
            "the", "a", "an"
        };

        string[] words = userInput.Split(delimiterCharacters);
        List<string> parsedWords = new List<string>(words);
        foreach (string s in ignoreWords)
        {
            parsedWords.Remove(s);
        }


        for (int i = 0; i < controller.inputActions.Length; i++)
        {
            InputAction inputAction = controller.inputActions[i];
            if (inputAction.hasKeyword(parsedWords[0]))
            {
                inputAction.invokeInputAction(parsedWords.ToArray());
            }
        }
        InputComplete();
    }

    void InputComplete()
    {
        controller.DisplayLoggedText();
        inputField.ActivateInputField();
        inputField.text = null;
    }

}
