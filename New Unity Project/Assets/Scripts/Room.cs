using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WordJam/Room")]
public class Room : ScriptableObject{

    /// <summary>
    /// The description of the room
    /// </summary>
    [TextArea]
    public string description;
    public Exit[] exits;
}
