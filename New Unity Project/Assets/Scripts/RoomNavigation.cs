using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{

    public Map currentMap;
    public MapNode currentRoom;


    Dictionary<Corridor.Direction, Corridor> exitDictionary = new Dictionary<Corridor.Direction, Corridor>();
    GameController controller;

    void Awake()
    {
        controller = GetComponent<GameController>();
    }

    public void UnpackExitsInRoom()
    {
        for (int i = 0; i < currentRoom.exits.Length; i++)
        {
            exitDictionary.Add(currentRoom.exits[i].exitDirection, currentRoom.exits[i]);
            controller.interactionDescriptionsInRoom.Add(currentRoom.exits[i].exitDescription);
        }
    }

    public void AttemptToChangeRooms(string directionNoun)
    {
        Corridor.Direction dir = parseDirection(directionNoun);

        if (exitDictionary.ContainsKey(dir))
        {
            if (exitDictionary[dir].isBlocked)
            {
                controller.message("This route seems blocked...");
            }
            else
            {
                currentRoom = exitDictionary[dir].roomToEnter;
                controller.message("You head off to the " + directionNoun);
                controller.DisplayRoomText();
            }
        }
        else
        {
            controller.message("There is no path to the " + directionNoun);
        }

    }

    public void ClearExits()
    {
        exitDictionary.Clear();
    }

    public Corridor.Direction parseDirection(string noun)
    {
        string nounLower = noun.ToLower();
        switch (nounLower)
        {
            case "north":
            case "forward":
            case "up":
                return Corridor.Direction.North;
            case "south":
            case "back":
            case "down":
                return Corridor.Direction.South;
            case "east":
            case "left":
                return Corridor.Direction.East;
            case "west":
            case "right":
                return Corridor.Direction.West;
            default:
                return Corridor.Direction.None;

        }
    }
}
