using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WordJam/Map")]
public class Map : ScriptableObject
{

    /// <summary>
    /// The name of the room
    /// Has no purpose other than organization
    /// </summary>
    public string mapName;

    public int xBounds, yBounds;
    public int maxRoomHeight, maxRoomWidth;
    public int maxCorridorLen;

    /// <summary>
    /// The number of rooms to be generated
    /// </summary>
    public int numRooms;
}
