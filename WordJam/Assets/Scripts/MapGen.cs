using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{

    public Map[] floorData;
    private Map mapData;

    public SnapCam2D cam;
    public Sprite playerSprite;

    [HideInInspector]
    public MapNode[,] nodes;
    public MapNodeComp[,] nodeComps;
    [HideInInspector]
    public MapRoom[] rooms;
    [HideInInspector]
    public Corridor[] corridors;
    [HideInInspector]
    public int floorNum = 1;

    public int playerStartX = 0, playerStartY = 0;

    /// <summary>
    /// The game object that will hold all the map and corridor tiles
    /// </summary>
    GameObject mapHolder;
    RoomNavigation nav;

    void Start()
    {
        AssignMapNav();
        mapData = floorData[0];
        floorNum = 1;
        GenerateNewFloor();
    }

    void GenerateNewFloor()
    {
        GenerateRoomsAndCorridorsInfo();
        GenerateNodeComps();
        AssignRoomEvents();
        GameController.instance.DisplayLoggedText();
    }

    /// <summary>
    /// 
    /// </summary>
    void GenerateRoomsAndCorridorsInfo()
    {

        if (mapHolder != null)
        {
            Destroy(mapHolder);
        }
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

                playerStartX = x;
                playerStartY = y;

                int w = Random.Range(1, mapData.maxRoomWidth + 1);
                int h = Random.Range(1, mapData.maxRoomHeight + 1);

                rooms[i].SetupRoom(x, y, w, h);
                int corridorLen = Random.Range(1, mapData.maxCorridorLen + 1);

                corridors[i].setupCorridor(rooms[i], corridorLen);

                foreach (MapNode node in corridors[i].corridorNodes)
                {
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
                GameObject newNode = SetupRoomNode(room, node);
                newNode.transform.SetParent(newRoomObj.transform);
            }
        }

        foreach (Corridor corridor in corridors)
        {
            GameObject newRoomObj = new GameObject("Corridor");
            newRoomObj.transform.SetParent(mapHolder.transform);
            foreach (MapNode node in corridor.corridorNodes)
            {
                GameObject newNode = SetupCorridorNode(corridor, node);
                newNode.transform.SetParent(newRoomObj.transform);
            }
        }

        ReLinkNodes();
    }

    GameObject SetupRoomNode(MapRoom room, MapNode node)
    {
        room.gen = this;
        nodes[node.posX, node.posY] = node;

        foreach (MapNode r_node in room.roomnodes)
        {
            if (r_node != node)
            {
                if (nodes[node.posX, node.posY + 1] == r_node)
                    node.exits.Add(MapNode.Direction.North);

                if (nodes[node.posX + 1, node.posY] == r_node)
                    node.exits.Add(MapNode.Direction.East);

                if (nodes[node.posX, node.posY - 1] == r_node)
                    node.exits.Add(MapNode.Direction.South);

                if (nodes[node.posX - 1, node.posY] == r_node)
                    node.exits.Add(MapNode.Direction.West);
            }
        }
        node.setNodeSprites();
        GameObject newNode = new GameObject("Node");
        MapNodeComp comp = newNode.AddComponent<MapNodeComp>();
        comp.nodeData = node;
        nodeComps[node.posX, node.posY] = comp;

        return newNode;
    }

    GameObject SetupCorridorNode(Corridor corridor, MapNode node)
    {
        nodes[node.posX, node.posY] = node;

        if (corridor.direction == Corridor.Direction.North || corridor.direction == Corridor.Direction.South)
        {
            node.exits.Add(MapNode.Direction.North);
            node.exits.Add(MapNode.Direction.South);


            nodes[node.posX, node.posY - 1].exits.Add(MapNode.Direction.North);
            nodes[node.posX, node.posY - 1].setNodeSprites();

            nodes[node.posX, node.posY + 1].exits.Add(MapNode.Direction.South);
            nodes[node.posX, node.posY + 1].setNodeSprites();


        }
        else
        {
            node.exits.Add(MapNode.Direction.East);
            node.exits.Add(MapNode.Direction.West);


            nodes[node.posX - 1, node.posY].exits.Add(MapNode.Direction.East);
            nodes[node.posX - 1, node.posY].setNodeSprites();

            nodes[node.posX + 1, node.posY].exits.Add(MapNode.Direction.West);
            nodes[node.posX + 1, node.posY].setNodeSprites();


        }

        node.setNodeSprites();

        GameObject newNode = new GameObject("Node");
        MapNodeComp comp = newNode.AddComponent<MapNodeComp>();
        comp.nodeData = node;
        nodeComps[node.posX, node.posY] = comp;

        return newNode;

    }

    /// <summary>
    /// Re links all the nodes in the map to the correct game objects
    /// </summary>
    void ReLinkNodes()
    {
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                if (nodes[i, j] != null)
                {
                    nodeComps[i, j].nodeData = nodes[i, j];
                }
            }

        }

        nav.currentNode = nodes[playerStartX, playerStartY];
        nav.currNodeComp = nodeComps[playerStartX, playerStartY];
        nav.cam = cam;
    }

    /// <summary>
    /// Creates the Map Nav Component
    /// </summary>
    void AssignMapNav()
    {
        nav = gameObject.AddComponent<RoomNavigation>();

        nav.mapGen = this;

    }

    void AssignRoomEvents()
    {

        if (floorNum == 1)
        {
            rooms[0].events.Add(EventScripts.firstFall());
            rooms[1].events.Add(EventScripts.exitFloor());
        }
        else if (floorNum == 2)
        {
            rooms[0].events.Add(EventScripts.skeletonPuzzzle1());
            rooms[0].events.Add(EventScripts.skeletonPuzzzle2());
        }
        else
        {

        }

        rooms[rooms.Length - 1].events.Add(EventScripts.exitFloor());
        foreach (MapRoom room in rooms)
        {
            room.assignRandomEvents();
        }
    }

    public void SwitchFloors(int floor)
    {
        this.floorNum = floor;
        if (floor < floorData.Length)
            mapData = floorData[floor - 1];
        else
            mapData = floorData[floorData.Length - 1];

        foreach (MapRoom room in rooms)
        {
            foreach (NodeEvent e in room.events)
            {
                e.Flush();
            }
        }
        GenerateNewFloor();
    }
}
