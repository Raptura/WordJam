﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text displayText, objectiveText;
    public ScrollRect displayTextScrollBar;

    [HideInInspector]
    public InputAction[] inputActions;

    [HideInInspector]
    public RoomNavigation roomNavigation;
    [HideInInspector]
    public List<string> interactionDescriptionsInRoom = new List<string>();

    private Dictionary<string, UnityEvent> eventDictionary;
    private static GameController controller;
    public static GameController instance
    {
        get
        {
            if (!controller)
            {
                controller = FindObjectOfType(typeof(GameController)) as GameController;

                if (!controller)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    controller.Init();
                }
            }

            return controller;
        }
    }

    public static string lastEvent;

    List<string> actionLog = new List<string>();

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    // Use this for initialization
    void Awake()
    {
        roomNavigation = GetComponent<RoomNavigation>();
    }

    void Start()
    {
        DisplayRoomText();
        DisplayLoggedText();

        inputActions = Resources.LoadAll<InputAction>("Input Actions");
    }

    void Update()
    {
        displayTextScrollBar.verticalNormalizedPosition = 0;
    }

    public void DisplayLoggedText()
    {
        string logAsText = string.Join("\n", actionLog.ToArray());

        displayText.text = logAsText;
    }

    public void DisplayRoomText()
    {
        string joinedInteractionDescriptions = string.Join("\n", interactionDescriptionsInRoom.ToArray());

        string combinedText = joinedInteractionDescriptions; //roomNavigation.currentRoom.description + "\n" + joinedInteractionDescriptions;

        message(combinedText);
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (controller == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        Debug.Log("Event Triggered: " + eventName);
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            lastEvent = eventName;

            thisEvent.Invoke();

        }
    }

    /// <summary>
    /// Writes a message to the console
    /// </summary>
    /// <param name="stringToAdd"></param>
    public void message(string stringToAdd)
    {
        actionLog.Add(stringToAdd + "\n");
    }

    public void changeObjective(string text)
    {
        objectiveText.text = text;
    }

}
