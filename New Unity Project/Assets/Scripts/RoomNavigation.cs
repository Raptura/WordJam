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
        MapNode.Direction dir = parseDirection(directionNoun);


        if (currentNode.exits.Contains(dir))
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

            currentNode = mapData.nodes[x, y];
            currNodeComp = mapData.nodeComps[x, y];

            controller.message("You move " + directionNoun);
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
}
