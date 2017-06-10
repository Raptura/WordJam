using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeEvent
{
    public enum EventStatus
    {
        Incomplete,
        Success,
        Failure
    }

    [HideInInspector]
    public EventStatus status;

    public delegate void OnSuccess();
    public delegate void OnFailure();

    [TextArea]
    public string description;

}
