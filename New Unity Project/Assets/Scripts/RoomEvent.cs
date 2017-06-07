using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomEvent {


    public enum EventStatus {
        Incomplete,
        Success,
        Failure
    }

    public EventStatus status;

    public delegate void OnSuccess();
    public delegate void OnFailure();

    [TextArea]
    public string description;

}
