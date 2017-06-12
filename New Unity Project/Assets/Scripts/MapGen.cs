using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{

    public Map mapData;

    [HideInInspector]
    public MapNode[,] nodes;
    [HideInInspector]
    public MapRoom[] rooms;
    [HideInInspector]
    public Corridor[] corridors;

    /// <summary>
    /// The game object that will hold all the map and corridor tiles
    /// </summary>
    GameObject mapHolder;

    public Sprite roomSprite, corridorSprite;

    void Start()
    {
        nodes = new MapNode[mapData.xBounds, mapData.yBounds];

        mapHolder = new GameObject("Map Holder");

        GenerateRoomsAndCorridors();

       // createAllNodes();

    }

    /// <summary>
    /// 
    /// </summary>
    void GenerateRoomsAndCorridors()
    {
        rooms = new MapRoom[mapData.numRooms];
        corridors = new Corridor[mapData.numRooms - 1];

        for (int i = 0; i < mapData.numRooms; i++)
        {
            //generate the first room and corridor

            if (i == 0)
            {
                rooms[i] = new MapRoom();
                corridors[i] = new Corridor();

                //int x = Random.Range(0, mapData.xBounds);
                //int y = Random.Range(0, mapData.yBounds);
                int x = mapData.xBounds / 2;
                int y = mapData.yBounds / 2;

                int w = Random.Range(1, mapData.maxRoomWidth + 1);
                int h = Random.Range(1, mapData.maxRoomHeight + 1);

                rooms[i].SetupRoom(x, y, w, h);
                int corridorLen = mapData.maxCorridorLen; //Random.Range(1, mapData.maxCorridorLen + 1);

                corridors[i].setupCorridor(rooms[i], corridorLen);

                //Then add the nodes to the nodes list
                GameObject newCorridorObj = new GameObject("Corridor");
                newCorridorObj.transform.SetParent(mapHolder.transform);
                foreach (MapNode node in corridors[i].corridorNodes)
                {
                    node.nodeSprite = corridorSprite;
                    nodes[node.posX, node.posY] = node;

                    GameObject newNode = new GameObject("Node");
                    newNode.AddComponent<MapNodeComp>().nodeData = node;
                    newNode.transform.SetParent(newCorridorObj.transform);
                }
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
                        x = Random.Range(corX - (w / 2), corX + (w / 2) + 1);
                        y = corY - 1;
                        break;
                    case Corridor.Direction.East:
                        x = corX + 1;
                        y = Random.Range(corY - (h / 2), corY + (h / 2) + 1);
                        break;
                    case Corridor.Direction.South:
                        x = Random.Range(corX - (w / 2), corX + (w / 2) + 1);
                        y = corY + 1;
                        break;
                    case Corridor.Direction.West:
                        x = corX - 1;
                        y = Random.Range(corY - (h / 2), corY + (h / 2) + 1);
                        break;
                }



                rooms[i].SetupRoom(x, y, w, h, corridors[i - 1]);

                //We still have to build more corridors for the new rooms
                if (i < corridors.Length)
                {
                    corridors[i] = new Corridor();
                    int corridorLen = mapData.maxCorridorLen; //Random.Range(1, mapData.maxCorridorLen + 1);
                    corridors[i].setupCorridor(rooms[i], corridorLen);

                    //Then add the nodes to the nodes list
                    GameObject newCorridorObj = new GameObject("Corridor");
                    newCorridorObj.transform.SetParent(mapHolder.transform);
                    foreach (MapNode node in corridors[i].corridorNodes)
                    {
                        node.nodeSprite = corridorSprite;
                        nodes[node.posX, node.posY] = node;

                        GameObject newNode = new GameObject("Node");
                        newNode.AddComponent<MapNodeComp>().nodeData = node;
                        newNode.transform.SetParent(newCorridorObj.transform);
                    }
                }
            }

            //Then add the nodes to the nodes list
            //Also make the gameobjects
            GameObject newRoomObj = new GameObject("Room");
            newRoomObj.transform.SetParent(mapHolder.transform);

            foreach (MapNode node in rooms[i].roomnodes)
            {
                node.nodeSprite = roomSprite;
                nodes[node.posX, node.posY] = node;

                GameObject newNode = new GameObject("Node");
                newNode.AddComponent<MapNodeComp>().nodeData = node;
                newNode.transform.SetParent(newRoomObj.transform);
            }
        }
    }
}
