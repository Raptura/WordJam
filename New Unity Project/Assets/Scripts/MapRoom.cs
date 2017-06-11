using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom : MonoBehaviour
{

    public MapNode[,] roomnodes;
    public NodeEvent[] events;
    //Node information for Map

    public int posX, posY;
    public int width, height; //How large is the room
    public Corridor entranceCorridor;

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
    }

    public void SetupRoom(int x, int y, int width, int height, Corridor corridor)
    {
        this.width = width;
        this.height = height;
        roomnodes = new MapNode[width, height];

        this.entranceCorridor = corridor;
        posX = x;
        posY = y;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int nodeX = 0;
                int nodeY = 0;

                switch (corridor.direction)
                {
                    case Corridor.Direction.North:
                        nodeX = posX + i; //random element from start
                        nodeY = posY - j; //must expand up
                        break;
                    case Corridor.Direction.East:
                        nodeX = posX + i; //must expand right
                        nodeY = posY + j; //random element from start
                        break;
                    case Corridor.Direction.South:
                        nodeX = posX + i; //random element from start
                        nodeY = posY + j; //must expand down
                        break;
                    case Corridor.Direction.West:
                        nodeX = posX - i; //must expand left
                        nodeY = posY + j; //random element from start
                        break;
                }

                roomnodes[i, j] = new MapNode(nodeX, nodeY);
            }
        }
    }
}
