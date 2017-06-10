using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{

    public NodeEvent[] events;
    //Node information for Map
    public int posX, posY;
    public int width, height; //How large is the room
    public Corridor corridor;

    // This is used for the first room.  It does not have a Exit parameter since there are no Exits yet.
    public void SetupRoom(int width, int height, int x, int y)
    {
        this.width = width;
        this.height = height;
        posX = x;
        posY = y;
    }

    // This is an overload of the SetupRoom function and has a corridor parameter that represents the corridor entering the room.
    public void SetupRoom(int width, int height, int x, int y, Corridor corridor)
    {
        //Set the typical values
        this.width = width;
        this.height = height;
        posX = x;
        posY = y;

        corridor.linkRoom(this);
    }

}
