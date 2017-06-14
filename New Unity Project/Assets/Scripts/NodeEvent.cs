using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public delegate void Action();

    public OnSuccess successDelegate;
    public OnFailure failureDelegate;

    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    private Dictionary<string, UnityAction> uActions = new Dictionary<string, UnityAction>();

    public UnityAction successListener;
    public UnityAction failureListener;

    public string description;

    public void addAction(string key, Action ac)
    {
        actions.Add(key, ac);
        UnityAction newActionListener = new UnityAction(invokeAction);
        uActions.Add(key, newActionListener);
        GameController.StartListening(key, newActionListener);
    }

    public void removeAction(string key)
    {
        GameController.StopListening(key, uActions[key]);
        actions.Remove(key);
        uActions.Remove(key);
    }

    void invokeAction()
    {
        if (actions.ContainsKey(GameController.lastEvent))
        {
            actions[GameController.lastEvent].Invoke();
        }
    }

    void Succeed()
    {
        if (status == EventStatus.Incomplete)
        {
            status = EventStatus.Success;
            successDelegate.Invoke();
        }
    }

    public void Fail()
    {

        if (status == EventStatus.Incomplete)
        {
            status = EventStatus.Failure;
            failureDelegate.Invoke();
        }
    }

    public NodeEvent()
    {
        successListener = new UnityAction(Succeed);
        failureListener = new UnityAction(Fail);
    }

}
