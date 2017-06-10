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
    /// <summary>
    /// The size of the map
    /// </summary>
    public int mapSize;

    [HideInInspector]
    public MapNode[] rooms;

    void GenerateRooms()
    {

    
    }


}
