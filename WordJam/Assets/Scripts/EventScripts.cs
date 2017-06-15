using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScripts
{

    public static NodeEvent testEvent()
    {
        NodeEvent e = new NodeEvent();
        e.status = NodeEvent.EventStatus.Incomplete;

        string success = "testSuccess";
        string fail = "testFail";

        GameController.StartListening("testSuccess", e.successListener);
        GameController.StartListening("testFail", e.failureListener);

        //What happens when you succeed
        e.successDelegate += () =>
        {
            GameController.instance.message("You succeeded!");
            GameController.StopListening(success, e.successListener);
        };

        //What happens when you fail
        e.failureDelegate += () =>
        {
            GameController.instance.message("You Failed");
            GameController.StopListening(fail, e.failureListener);
        };


        e.addAction("take test", delegate ()
        {
            GameController.instance.message("This is a test...");
        });

        return e;
    }

    public static NodeEvent exitFloor(MapNode node)
    {
        NodeEvent e = new NodeEvent();
        e.status = NodeEvent.EventStatus.Incomplete;
        e.node = node;

        e.setupEnterAction(delegate
        {
            GameController.instance.message("You see the stairs to the next floor");
        });

        e.addAction("exit", delegate
        {
            if (GameController.instance.roomNavigation.currentNode == node)
            {
                GameController.instance.message("You climb the stairs to the next floor");
                GameController.instance.roomNavigation.ExitFloor();
                e.removeAction("exit");
            }

        });

        return e;
    }

    public static NodeEvent firstFall(MapNode node)
    {
        NodeEvent e = new NodeEvent();
        e.status = NodeEvent.EventStatus.Incomplete;

        string success = "fallSuccess";
        string fail = "fallFail";

        GameController.StartListening(success, e.successListener);
        GameController.StartListening(fail, e.failureListener);

        //What happens when you succeed
        e.successDelegate += () =>
        {
            //GameController.instance.message("You succeeded!");
            GameController.StopListening(success, e.successListener);

            node.blockedExits.Clear();
        };

        //What happens when you fail
        e.failureDelegate += () =>
        {
            //GameController.instance.message("You Failed");
            GameController.StopListening(fail, e.failureListener);
        };

        GameController.instance.message("You wake up in a dark cavern. You can't see much, but ahead of you, you see a torch that has just an ember left.");
        GameController.instance.message("You should probably take that torch.");

        GameController.instance.changeObjective("take the torch");
        node.blockedExits.Add(MapNode.Direction.North);
        node.blockedExits.Add(MapNode.Direction.South);
        node.blockedExits.Add(MapNode.Direction.East);
        node.blockedExits.Add(MapNode.Direction.West);
        node.room.description = "The room is extremely dark...";

        e.addAction("take torch", delegate ()
        {
            GameController.instance.playerInfo.addTorch();
            GameController.instance.message("You take the torch");
            GameController.instance.message("Even if the torch is about to die out, you should probably look around the room");
            GameController.instance.changeObjective("Look Around the room");
            e.removeAction("take torch");

            e.addAction("look around", delegate ()
            {
                GameController.instance.message("With what little light the torch gives, you see a small bottle on the ground.");
                GameController.instance.changeObjective("Grab the bottle off the ground");

                e.removeAction("look around");
                e.addAction("take bottle", delegate ()
                {
                    GameController.instance.playerInfo.addInventory("fuel bottle");
                    GameController.instance.message("You take the bottle");
                    GameController.instance.message("Well, go on, try examining the bottle.");
                    GameController.instance.changeObjective("Examine the bottle");
                    e.removeAction("take bottle");

                    e.addAction("look bottle", delegate ()
                    {
                        GameController.instance.message("The bottle contains fuel. ");
                        GameController.instance.message("Maybe try using the bottle on the torch ?");
                        GameController.instance.changeObjective("use the bottle on the torch");
                        e.removeAction("look bottle");

                        e.addAction("use bottle on torch", delegate ()
                        {
                            GameController.instance.playerInfo.removeInventory("fuel bottle");
                            GameController.instance.playerInfo.addInventory("empty bottle");

                            GameController.instance.message("You pour some of the fuel onto the torch, and the embers grow into a healthy flame.");
                            GameController.instance.message("You can see a bit better now.");
                            GameController.instance.changeObjective("Use your newly gained vision to look around the room");
                            node.room.description = "The ground beneath you is very moist. A little too moist for your liking.";
                            e.removeAction("use bottle on torch");

                            e.addAction("look around", delegate ()
                            {
                                string dir = "";
                                MapNode.Direction dirS = node.exits[0];
                                dir = dirS.ToString().ToLower();

                                GameController.instance.message("To the " + dir + " you see an opening that you can go through.");
                                GameController.instance.message("That seems like a good direction to go in.");
                                GameController.instance.changeObjective("Exit the floor: Use the 'move/go' command");
                                GameController.TriggerEvent(success);
                                e.removeAction("look around");
                            });

                        });

                    });

                });

            });

        });

        return e;
    }

    public static NodeEvent skeletonPuzzzle1(MapNode node)
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent();
        e.status = NodeEvent.EventStatus.Incomplete;
        e.node = node;


        e.setupEnterAction(delegate
        {
            node.room.lockRoom();
            node.room.description = "You see a stone table in the room. On top of this table is a skeleton. It's missing its head";

            if (cont.roomNavigation.traversedNodes.Contains(node) == false)
            {
                GameController.instance.message("You hear a door lock in the distance...");
            }
            GameController.instance.message("You notice that there's a human skull on the ground.");

            e.addAction("take skull", delegate
            {
                if (cont.roomNavigation.currentNode == node)
                {
                    string examine = "The skull is weathered and bare. It fits neatly in your hand. You'd do a hamlet reenaction but you wonder where its other boney companions are.";
                    cont.playerInfo.addInventory("skull", examineText: examine);
                    cont.message("You take the skull");

                    e.removeAction("take skull");
                    e.removeEnterAction();
                }
            });

        });

        return e;
    }

    public static NodeEvent skeletonPuzzzle2(MapNode node)
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent();
        e.status = NodeEvent.EventStatus.Incomplete;
        e.node = node;


        e.setupEnterAction(delegate
        {
            GameController.instance.message("You rub against a stone table with a skeleton on it. It's missing its head");

            e.addAction("place skull on table", delegate
            {
                if (cont.roomNavigation.currentNode == node)
                {
                    if (cont.playerInfo.hasItem("skull"))
                    {
                        cont.playerInfo.removeInventory("skull");
                        cont.message("You place the skull on the table and complete the skeleton.");
                        cont.message("The ground rumbles beneath you. You see a stone slab in the distance move, revealing a stairway.");
                        e.removeAction("place skull on table");
                        node.room.unlockRoom();
                        e.removeEnterAction();
                    }
                }
            });


        });

        return e;
    }

}