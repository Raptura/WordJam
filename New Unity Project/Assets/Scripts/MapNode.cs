using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{

    public int posX, posY;

    public MapNode(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Sprite nodeSprite;

    public enum Direction
    {
        North,
        East,
        South,
        West,
        None //This will not be set intentionally, only reference
    }
    public List<Direction> exits = new List<Direction>();

    public void setNodeSprites(Sprite[] roomSprites)
    {
        bool north = exits.Contains(Direction.North);
        bool south = exits.Contains(Direction.South);
        bool east = exits.Contains(Direction.East);
        bool west = exits.Contains(Direction.West);

        if (north)
        {
            nodeSprite = roomSprites[0];
            if (east)
            {
                nodeSprite = roomSprites[7];
                if (south)
                {
                    nodeSprite = roomSprites[9];
                    if (west)
                    {
                        nodeSprite = roomSprites[14];
                    }
                }
                else if (west)
                {
                    nodeSprite = roomSprites[8];
                }
            }
            else if (south)
            {
                nodeSprite = roomSprites[2];
                if (west)
                {
                    nodeSprite = roomSprites[13];
                }
            }
            else if (west)
            {
                nodeSprite = roomSprites[6];
            }
        }
        else if (east)
        {
            nodeSprite = roomSprites[1];
            if (south)
            {
                nodeSprite = roomSprites[11];
                if (west)
                {
                    nodeSprite = roomSprites[12];
                }
            }
            else if (west)
            {
                nodeSprite = roomSprites[3];
            }
        }
        else if (south)
        {
            nodeSprite = roomSprites[5];
            if (west)
            {
                //sw
                nodeSprite = roomSprites[10];
            }
        }
        else if (west)
        {
            nodeSprite = roomSprites[4];
        }
        else
        {
            //Should never get here
            nodeSprite = roomSprites[15];
        }

    }

}
