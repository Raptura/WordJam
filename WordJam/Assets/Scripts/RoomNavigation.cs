using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{

    public MapGen mapGen;
    public MapNodeComp currNodeComp;
    public MapNode currentNode;
    public SnapCam2D cam;
    public GameObject playerBlip;
    public List<MapNode> traversedNodes = new List<MapNode>();

    GameController controller;

    public void describeRoom()
    {
        if (currentNode.room != null)
        {
            GameController.instance.message(currentNode.room.description);
        }
        else
        {
            GameController.instance.message("This is an extremely dark corridor...");
        }
    }

    void Start()
    {
        playerBlip = new GameObject("Player Blip");
        playerBlip.transform.position = new Vector3(currNodeComp.transform.position.x, currNodeComp.transform.position.y, -1);
        playerBlip.AddComponent<SpriteRenderer>().sprite = mapGen.playerSprite;
        cam.target = playerBlip.transform;
        GameController.instance.changeFloor(1);
        GameController.TriggerEvent("enter node " + "(" + currentNode.posX + "," + currentNode.posY + ")");
        if (currentNode.room != null)
            GameController.TriggerEvent("enter room " + "(" + currentNode.room.roomNum + ")");
        traversedNodes.Add(currentNode);
        controller.DisplayLoggedText();
    }

    void Awake()
    {
        controller = GameController.instance;
        controller.roomNavigation = this;
    }

    void Update()
    {
        if (currNodeComp.transform != null)
        {
            playerBlip.transform.position = new Vector3(currNodeComp.transform.position.x, currNodeComp.transform.position.y, -1);
        }
    }

    public void AttemptToChangeNodes(string directionNoun)
    {
        MapNode.Direction dir = parseDirection(directionNoun);

        if (currentNode.exits.Contains(dir) && currentNode.blockedExits.Contains(dir) == false)
        {

            int x = currentNode.posX;
            int y = currentNode.posY;
            switch (dir)
            {
                case MapNode.Direction.North:
                    y++;
                    break;
                case MapNode.Direction.East:
                    x++;
                    break;
                case MapNode.Direction.South:
                    y--;
                    break;
                case MapNode.Direction.West:
                    x--;
                    break;
                default:
                    break;
            }

            currentNode = mapGen.nodes[x, y];
            currNodeComp = mapGen.nodeComps[x, y];

            controller.message("You move " + directionNoun);
            GameController.TriggerEvent("enter node " + "(" + x + "," + y + ")");

            if (currentNode.room != null)
                GameController.TriggerEvent("enter room " + "(" + currentNode.room.roomNum + ")");

            traversedNodes.Add(currentNode);
        }
        else if (currentNode.blockedExits.Contains(dir))
        {
            controller.message("The way " + directionNoun + " is blocked!");
        }
        else
        {
            controller.message("There is no path to the " + directionNoun);
        }
    }

    /// <summary>
    /// Parses user input to get direction
    /// </summary>
    /// <param name="noun"></param>
    /// <returns></returns>
    public MapNode.Direction parseDirection(string noun)
    {
        string nounLower = noun.ToLower();
        switch (nounLower)
        {
            case "north":
            case "forward":
            case "up":
                return MapNode.Direction.North;
            case "south":
            case "back":
            case "down":
                return MapNode.Direction.South;
            case "east":
            case "right":
                return MapNode.Direction.East;
            case "west":
            case "left":
                return MapNode.Direction.West;
            default:
                return MapNode.Direction.None;

        }
    }

    public void ExitFloor()
    {
        traversedNodes.Clear();
        GameController.instance.changeFloor(mapGen.floorNum + 1);
        mapGen.SwitchFloors(mapGen.floorNum + 1);

        GameController.TriggerEvent("enter node " + "(" + currentNode.posX + "," + currentNode.posY + ")");
        GameController.TriggerEvent("enter room " + "(" + currentNode.room.roomNum + ")");
        traversedNodes.Add(currentNode);
        currentNode.hidden = false;
    }

}
