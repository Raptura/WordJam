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
}
