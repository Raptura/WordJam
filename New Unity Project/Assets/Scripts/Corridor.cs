using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor
{

    public MapNode[] corridorNodes;

    public string exitDescription;


    public enum Direction
    {
        North,
        East,
        South,
        West,
        None //This will not be set intentionally, only reference
    }
    public Direction direction;

    public MapRoom[] linkedRooms = new MapRoom[2];
    public int length;
    public int startPosX, startPosY;

    public int endPosX
    {
        get
        {
            switch (direction)
            {
                case Direction.North:
                case Direction.South:
                    return startPosX;
                case Direction.East:
                    return startPosX + length - 1;
                case Direction.West:
                    return startPosX - length + 1;
                default:
                    return 0;
            }
        }
    }

    public int endPosY
    {
        get
        {
            switch (direction)
            {
                case Direction.East:
                case Direction.West:
                    return startPosY;
                case Direction.North:
                    return startPosY + length + 1;
                case Direction.South:
                    return startPosY - length - 1;
                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// Sets up a new corridor next to a node
    /// </summary>
    /// <param name="room"></param>
    public void setupCorridor(MapRoom room, int length)
    {
        this.length = length;
        //Choose a random direction to go
        direction = (Direction)Random.Range(0, 4);

        if (room.entranceCorridor != null)
        {
            Direction otherDir = room.entranceCorridor.direction;

            //If the directions are the same, change directions...
            if (direction == otherDir)
            {
                int directionInt = (int)direction;
                directionInt++;
                directionInt = directionInt % 4;
                direction = (Direction)directionInt;
            }
        }

        switch (direction)
        {
            case Direction.North:
                startPosX = room.posX; //Random.Range(room.posX, room.posX + room.width - 1);
                startPosY = room.posY + room.height;
                break;
            case Direction.East:
                startPosX = room.posX + room.width;
                startPosY = room.posY; //Random.Range(room.posY, room.posY + room.height - 1);
                break;
            case Direction.South:
                startPosX = room.posX; //Random.Range(room.posX, room.posX + room.width - 1);
                startPosY = room.posY - 1;
                break;
            case Direction.West:
                startPosX = room.posX - 1;
                startPosY = room.posY; //Random.Range(room.posY, room.posY + room.height - 1);
                break;
        }

        corridorNodes = new MapNode[length];
        for (int i = 0; i < length; i++)
        {
            int x = 0;
            int y = 0;

            switch (direction)
            {
                case Direction.North:
                    x = startPosX;
                    y = startPosY + i;
                    break;

                case Direction.East:
                    x = startPosX + i;
                    y = startPosY;
                    break;
                case Direction.South:
                    x = startPosX;
                    y = startPosY - i;
                    break;
                case Direction.West:
                    x = startPosX - i;
                    y = startPosY;
                    break;
            }
            corridorNodes[i] = new MapNode(x, y);
        }
    }
}
