using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Corridor
{

    public string exitDescription;


    public enum Direction
    {
        North,
        East,
        South,
        West
    }
    public Direction direction;

    public MapNode[] linkedRooms = new MapNode[2];
    public int length;
    public int startPosX, startPosY;

    public bool isBlocked;
    public bool isLinked { get { return linkedRooms[1] != null && linkedRooms[2] != null; } }

    public void linkRoom(MapNode node)
    {

        if (isLinked == false)
        {
            if (linkedRooms[1] == null)
            {
                linkedRooms[1] = node;
            }
            if (linkedRooms[2] == null)
            {
                linkedRooms[2] = node;
            }
        }

    }


    /// <summary>
    /// Sets up a new corridor next to a node
    /// </summary>
    /// <param name="node"></param>
    public void setupCorridor(MapNode node, int length)
    {

        this.length = length;
        //Choose a random direction to go
        direction = (Direction)Random.Range(0, 4);
        Direction otherDir = node.corridor.direction;

        //If the directions are the same, change directions...
        if (direction == otherDir)
        {
            int directionInt = (int)direction;
            directionInt++;
            directionInt = directionInt % 4;
            direction = (Direction)directionInt;
        }
        switch (direction)
        {
            case Direction.North:
                startPosX = Random.Range(node.posX, node.posX + node.width - 1);
                startPosY = node.posY + node.height; 
                break;
            case Direction.East:
                startPosX = node.posX;
                startPosY = node.posY;
                break;
            case Direction.South:
                startPosX = Random.Range(node.posX, node.posX + node.width);
                startPosY = node.posY;
                break;
            case Direction.West:
                startPosX = node.posX;
                startPosY = node.posY;
                break;
        }



    }
}
