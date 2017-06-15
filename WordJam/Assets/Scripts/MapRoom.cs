using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom
{
    public MapGen gen;
    public MapNode[,] roomnodes;
    public List<NodeEvent> events = new List<NodeEvent>();
    //Node information for Map

    public int posX, posY;
    public int width, height; //How large is the room
    public Corridor entranceCorridor = null;
    public string description; //The description used for "look around" action

    //Use this one only for the first room
    public void SetupRoom(int x, int y, int width, int height)
    {
        this.width = width;
        this.height = height;
        roomnodes = new MapNode[width, height];
        posX = x;
        posY = y;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                roomnodes[i, j] = new MapNode(posX + i, posY + j);
                roomnodes[i, j].room = this;
            }
        }

        Debug.Log("Room Created at (" + posX + "," + posY + ")");
    }

    public void SetupRoom(int x, int y, int width, int height, Corridor corridor)
    {
        this.width = width;
        this.height = height;
        roomnodes = new MapNode[width, height];

        entranceCorridor = corridor;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int nodeX = 0;
                int nodeY = 0;

                switch (corridor.direction)
                {
                    case Corridor.Direction.North:
                        nodeX = x + i; //random element from start
                        nodeY = y + j; //must expand up
                        break;
                    case Corridor.Direction.East:
                        nodeX = x + i; //must expand right
                        nodeY = y + j; //random element from start
                        break;
                    case Corridor.Direction.South:
                        nodeX = x + i; //random element from start
                        nodeY = y - j; //must expand down
                        break;
                    case Corridor.Direction.West:
                        nodeX = x - i; //must expand left
                        nodeY = y + j; //random element from start
                        break;
                }

                roomnodes[i, j] = new MapNode(nodeX, nodeY);
                roomnodes[i, j].room = this;
            }
        }

        posX = roomnodes[0, 0].posX;
        posY = roomnodes[0, 0].posY;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (posX >= roomnodes[i, j].posX)
                    posX = roomnodes[i, j].posX;

                if (posY >= roomnodes[i, j].posY)
                    posY = roomnodes[i, j].posY;
            }
        }

        Debug.Log("Room Created at (" + posX + "," + posY + ")");
    }

    public void lockRoom()
    {
        List<MapNode> roomsToList = new List<MapNode>();
        foreach (MapNode node in roomnodes)
        {
            roomsToList.Add(node);
        }

        foreach (MapNode node in roomnodes)
        {

            if (node.exits.Contains(MapNode.Direction.North))
                if (roomsToList.Contains(gen.nodes[node.posX, posY + 1]) == false)
                    node.blockedExits.Add(MapNode.Direction.North);

            if (node.exits.Contains(MapNode.Direction.East))
                if (roomsToList.Contains(gen.nodes[node.posX + 1, posY]) == false)
                    node.blockedExits.Add(MapNode.Direction.East);

            if (node.exits.Contains(MapNode.Direction.South))
                if (roomsToList.Contains(gen.nodes[node.posX, posY - 1]) == false)
                    node.blockedExits.Add(MapNode.Direction.South);

            if (node.exits.Contains(MapNode.Direction.West))
                if (roomsToList.Contains(gen.nodes[node.posX - 1, posY]) == false)
                    node.blockedExits.Add(MapNode.Direction.West);
        }
    }

    public void unlockRoom()
    {
        foreach (MapNode node in roomnodes)
        {
            node.blockedExits.Clear();
        }
    }
}