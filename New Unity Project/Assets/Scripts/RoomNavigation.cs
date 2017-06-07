using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{

    public Map currentMap;
    public Room currentRoom;


    Dictionary<Exit.ExitDir, Room> exitDictionary = new Dictionary<Exit.ExitDir, Room>();
    GameController controller;

    void Awake()
    {
        controller = GetComponent<GameController>();
    }

    public void UnpackExitsInRoom()
    {
        for (int i = 0; i < currentRoom.exits.Length; i++)
        {
            exitDictionary.Add(currentRoom.exits[i].exitDirection, currentRoom.exits[i].roomToEnter);
            controller.interactionDescriptionsInRoom.Add(currentRoom.exits[i].exitDescription);
        }
    }

    public void AttemptToChangeRooms(string directionNoun)
    {
        Exit.ExitDir dir = parseDirection(directionNoun);

        if (exitDictionary.ContainsKey(dir))
        {
            currentRoom = exitDictionary[dir];
            controller.message("You head off to the " + directionNoun);
            controller.DisplayRoomText();
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


    public Exit.ExitDir parseDirection(string noun)
    {
        string nounLower = noun.ToLower();
        switch (nounLower)
        {
            case "north":
            case "forward":
            case "up":
                return Exit.ExitDir.North;
            case "south":
            case "back":
            case "down":
                return Exit.ExitDir.South;
            case "east":
            case "left":
                return Exit.ExitDir.East;
            case "west":
            case "right":
                return Exit.ExitDir.West;
            default:
                return Exit.ExitDir.None;

        }
    }
}
