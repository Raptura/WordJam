using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WordJam/Map")]
public class Map : ScriptableObject{

    /// <summary>
    /// The name of the room
    /// Has no purpose other than organization
    /// </summary>
    public string mapName;

    [TextArea]
    public string map;

    public RoomEvent[] events;
    public Room[] rooms;

}
