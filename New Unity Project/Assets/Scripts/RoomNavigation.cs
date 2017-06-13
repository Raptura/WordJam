using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{

    public MapGen mapData;
    public MapNodeComp currNodeComp;
    public MapNode currentNode;
    public SnapCam2D cam;
    public GameObject playerBlip;

    GameController controller;

    public enum Direction
    {
        North,
        East,
        South,
        West,
        None //This will not be set intentionally, only reference
    }

    void Start()
    {
        playerBlip = new GameObject("Player Blip");
        playerBlip.transform.position = new Vector3(currNodeComp.transform.position.x, currNodeComp.transform.position.y, -1);
        playerBlip.AddComponent<SpriteRenderer>().sprite = mapData.playerSprite;
        cam.target = playerBlip.transform;
    }

    void Awake()
    {
        controller = GetComponent<GameController>();
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
        Direction dir = parseDirection(directionNoun);

        int x = currentNode.posX;
        int y = currentNode.posY;
        switch (dir)
        {
            case Direction.North:
                y++;
                break;
            case Direction.East:
                x++;
                break;
            case Direction.South:
                y--;
                break;
            case Direction.West:
                x--;
                break;
            default:
                break;
        }

        if (mapData.nodes[x, y] != null)
        {
            currentNode = mapData.nodes[x, y];
            currNodeComp = mapData.nodeComps[x, y];

            controller.message("You head off to the " + directionNoun);
            controller.DisplayRoomText();
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
    public Direction parseDirection(string noun)
    {
        string nounLower = noun.ToLower();
        switch (nounLower)
        {
            case "north":
            case "forward":
            case "up":
                return Direction.North;
            case "south":
            case "back":
            case "down":
                return Direction.South;
            case "east":
            case "left":
                return Direction.East;
            case "west":
            case "right":
                return Direction.West;
            default:
                return Direction.None;

        }
    }
}
