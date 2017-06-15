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

    public delegate void Action();

    public UnityAction successDelegate;
    public UnityAction failureDelegate;
    public UnityAction onInit;

    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    public UnityAction successListener;
    public UnityAction failureListener;
    public UnityAction enterListener;

    public string successTrigger, failureTrigger;

    public string description;

    private bool initialized = false;

    public void addAction(string key, Action ac)
    {
        if (actions.ContainsKey(key) == false)
        {
            actions.Add(key, ac);
            if (initialized)
                GameController.StartListening(key, invokeAction);
        }
    }

    public void removeAction(string key)
    {
        if (actions.ContainsKey(key))
        {
            GameController.StopListening(key, invokeAction);
            actions.Remove(key);
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

    void Fail()
    {

        if (status == EventStatus.Incomplete)
        {
            status = EventStatus.Failure;
            failureDelegate.Invoke();
        }
    }

    public NodeEvent(string name)
    {
        successTrigger = name + "success";
        failureTrigger = name + "fail";
        successListener = new UnityAction(Succeed);
        failureListener = new UnityAction(Fail);

        successDelegate = delegate { };
        failureDelegate = delegate { };
        onInit = delegate { };

        node = new MapNode(-1, -1);
        node.room = new MapRoom();
        initialized = false;
    }

    public void setupEnterAction(Action ac)
    {
        enterListener = new UnityAction(ac);
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
        GameController.StopListening(successTrigger, successListener);
        GameController.StopListening(failureTrigger, failureListener);

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

    public void Init()
    {
        GameController.StartListening(successTrigger, successListener);
        GameController.StartListening(failureTrigger, failureListener);

        GameController.StartListening("enter node " + "(" + node.posX + "," + node.posY + ")", enterListener);

        foreach (string key in actions.Keys)
        {
            GameController.StartListening(key, invokeAction);
        }
        onInit.Invoke();

        initialized = true;
    }
}