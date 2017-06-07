using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Exit
{
    public enum ExitDir
    {
        North,
        South,
        East,
        West,
        None
    }

    public ExitDir exitDirection;
    public Room roomToEnter;
    public string exitDescription;
    public bool isBlocked;
}
