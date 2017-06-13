using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom
{

    public MapNode[,] roomnodes;
    public NodeEvent[] events;
    //Node information for Map

    public int posX, posY;
    public int width, height; //How large is the room
    public Corridor entranceCorridor = null;

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
}