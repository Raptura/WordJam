using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{

    public Map mapData;
    public SnapCam2D cam;
    public Sprite roomSprite, corridorSprite; //To be removed in favor for generated sprites
    public Sprite playerSprite;

    [HideInInspector]
    public MapNode[,] nodes;
    public MapNodeComp[,] nodeComps;
    [HideInInspector]
    public MapRoom[] rooms;
    [HideInInspector]
    public Corridor[] corridors;

    /// <summary>
    /// The game object that will hold all the map and corridor tiles
    /// </summary>
    GameObject mapHolder;

    void Start()
    {

        GenerateRoomsAndCorridorsInfo();
        GenerateNodeComps();
        AssignMapNav();
    }

    /// <summary>
    /// 
    /// </summary>
    void GenerateRoomsAndCorridorsInfo()
    {
        nodes = new MapNode[mapData.xBounds, mapData.yBounds];
        mapHolder = new GameObject("Map Holder");
        rooms = new MapRoom[mapData.numRooms];
        corridors = new Corridor[mapData.numRooms - 1];

        MapRoom roomTemp = null;
        int issues = 0;
        int issuesMax = 50;

        for (int i = 0; i < mapData.numRooms; i++)
        {
            if (issues >= issuesMax)
            {
                Destroy(mapHolder);

                GenerateRoomsAndCorridorsInfo();
                Debug.Log("Remaking Map...");
                return;
            }

            bool roomGood = true;
            bool corridorGood = true;

            //generate the first room and corridor

            if (i == 0)
            {
                rooms[i] = new MapRoom();
                corridors[i] = new Corridor();

                int x = mapData.xBounds / 2;
                int y = mapData.yBounds / 2;

                int w = Random.Range(1, mapData.maxRoomWidth + 1);
                int h = Random.Range(1, mapData.maxRoomHeight + 1);

                rooms[i].SetupRoom(x, y, w, h);
                int corridorLen = Random.Range(1, mapData.maxCorridorLen + 1);

                corridors[i].setupCorridor(rooms[i], corridorLen);

                foreach (MapNode node in corridors[i].corridorNodes)
                {
                    node.nodeSprite = corridorSprite;
                    nodes[node.posX, node.posY] = node;
                }
            }
            else
            {
                if (roomTemp != null)
                {
                    rooms[i] = roomTemp;
                    roomTemp = null;
                }
                else
                {
                    rooms[i] = new MapRoom();


                    //Base X and Y on the last corridor
                    int x = 0;
                    int y = 0;

                    int w = Random.Range(1, mapData.maxRoomWidth + 1);
                    int h = Random.Range(1, mapData.maxRoomHeight + 1);

                    int corX = corridors[i - 1].endPosX;
                    int corY = corridors[i - 1].endPosY;

                    switch (corridors[i - 1].direction)
                    {
                        case Corridor.Direction.North:
                            x = Random.Range(corX - (w / 2), corX + (w / 2) - 1);
                            y = corY + 1;
                            break;
                        case Corridor.Direction.East:
                            x = corX + 1;
                            y = Random.Range(corY - (h / 2), corY + (h / 2) - 1);
                            break;
                        case Corridor.Direction.South:
                            x = Random.Range(corX - (w / 2), corX + (w / 2) - 1);
                            y = corY - 1;
                            break;
                        case Corridor.Direction.West:
                            x = corX - 1;
                            y = Random.Range(corY - (h / 2), corY + (h / 2) - 1);
                            break;
                    }

                    rooms[i].SetupRoom(x, y, w, h, corridors[i - 1]);

                    foreach (MapNode node in rooms[i].roomnodes)
                    {
                        if (nodes[node.posX, node.posY] != null)
                        {
                            roomGood = false;
                            Debug.Log("Bad Room");
                            break;
                        }
                    }
                }

                if (roomGood == true)
                {
                    //We still have to build more corridors for the new rooms
                    if (i < corridors.Length)
                    {
                        corridors[i] = new Corridor();
                        int corridorLen = Random.Range(1, mapData.maxCorridorLen + 1);
                        corridors[i].setupCorridor(rooms[i], corridorLen);


                        foreach (MapNode node in corridors[i].corridorNodes)
                        {
                            if (nodes[node.posX, node.posY] != null)
                            {
                                corridorGood = false;
                                roomTemp = rooms[i];
                                Debug.Log("Bad Corridor");
                                break;
                            }
                        }

                        if (corridorGood)
                        {
                            foreach (MapNode node in corridors[i].corridorNodes)
                            {
                                node.nodeSprite = corridorSprite;
                                nodes[node.posX, node.posY] = node;
                            }
                        }

                    }
                }
            }
            if (roomGood == true && corridorGood == true)
            {
                foreach (MapNode node in rooms[i].roomnodes)
                {
                    node.nodeSprite = roomSprite;
                    nodes[node.posX, node.posY] = node;
                }
            }
            else
            {
                i--;
                issues++;
            }
        }
    }

    /// <summary>
    /// Creates the Actual game Objects and info for the nodes on screen
    /// </summary>
    void GenerateNodeComps()
    {
        nodeComps = new MapNodeComp[mapData.xBounds, mapData.yBounds];

        foreach (MapRoom room in rooms)
        {
            GameObject newRoomObj = new GameObject("Room");
            newRoomObj.transform.SetParent(mapHolder.transform);
            foreach (MapNode node in room.roomnodes)
            {
                node.nodeSprite = roomSprite;
                nodes[node.posX, node.posY] = node;

                GameObject newNode = new GameObject("Node");
                MapNodeComp comp = newNode.AddComponent<MapNodeComp>();
                comp.nodeData = node;
                nodeComps[node.posX, node.posY] = comp;
                newNode.transform.SetParent(newRoomObj.transform);
            }
        }

        foreach (Corridor corridor in corridors)
        {
            GameObject newRoomObj = new GameObject("Corridor");
            newRoomObj.transform.SetParent(mapHolder.transform);
            foreach (MapNode node in corridor.corridorNodes)
            {
                node.nodeSprite = corridorSprite;
                nodes[node.posX, node.posY] = node;

                GameObject newNode = new GameObject("Node");
                MapNodeComp comp = newNode.AddComponent<MapNodeComp>();
                comp.nodeData = node;
                nodeComps[node.posX, node.posY] = comp;
                newNode.transform.SetParent(newRoomObj.transform);
            }
        }


    }

    /// <summary>
    /// Creates the Map Nav Component
    /// </summary>
    void AssignMapNav()
    {
        RoomNavigation nav = gameObject.AddComponent<RoomNavigation>();

        nav.mapData = this;
        nav.currentNode = nodes[50, 50];
        nav.currNodeComp = nodeComps[50, 50];
        nav.cam = cam;
    }
}
