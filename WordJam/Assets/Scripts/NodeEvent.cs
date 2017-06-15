using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NodeEvent
{

    public MapNode node;

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
    public UnityAction enterListener;

    public string description;

    public void addAction(string key, Action ac)
    {
        if (actions.ContainsKey(key) == false)
        {
            actions.Add(key, ac);
            UnityAction newActionListener = new UnityAction(invokeAction);
            uActions.Add(key, newActionListener);
            GameController.StartListening(key, newActionListener);
        }
    }

    public void removeAction(string key)
    {
        if (actions.ContainsKey(key))
        {
            GameController.StopListening(key, uActions[key]);
            actions.Remove(key);
            uActions.Remove(key);
        }
        else
        {
            Debug.Log("The key" + key + "does not exist");
        }
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

    public void setupEnterAction(Action ac)
    {
        enterListener = new UnityAction(ac);
        GameController.StartListening("enter node " + "(" + node.posX + "," + node.posY + ")", enterListener);
    }

    public void removeEnterAction()
    {
        if (enterListener != null)
        {
            GameController.StopListening("enter node " + "(" + node.posX + "," + node.posY + ")", enterListener);
        }
    }

    public void Flush()
    {
        List<string> acKeys = new List<string>();
        foreach (string key in actions.Keys)
        {
            acKeys.Add(key);
        }
        foreach (string key in acKeys)
        {
            removeAction(key);
        }
        removeEnterAction();
    }
}
