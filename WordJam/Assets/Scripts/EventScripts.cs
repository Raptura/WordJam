using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScripts
{

    public static NodeEvent testEvent()
    {

        NodeEvent e = new NodeEvent("test");
        e.status = NodeEvent.EventStatus.Incomplete;

        //What happens when you succeed
        e.successDelegate += () =>
        {
            GameController.instance.message("You succeeded!");
            GameController.StopListening(e.successTrigger, e.successListener);
        };

        //What happens when you fail
        e.failureDelegate += () =>
        {
            GameController.instance.message("You Failed");
            GameController.StopListening(e.failureTrigger, e.failureListener);
        };


        e.addAction("take test", delegate ()
        {
            GameController.instance.message("This is a test...");
        });

        return e;
    }

    public static NodeEvent exitFloor()
    {
        NodeEvent e = new NodeEvent("exitFloor");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.setupEnterAction(delegate
        {
            e.node.room.description = "You see the stairs to the next floor";
            GameController.instance.message("You see the stairs to the next floor");
            GameController.instance.changeObjective("Exit the floor.");
        });

        e.addAction("exit", delegate
        {
            if (GameController.instance.roomNavigation.currentNode == e.node)
            {
                GameController.instance.message("You climb the stairs to the next floor");
                GameController.instance.roomNavigation.ExitFloor();
                e.removeAction("exit");
            }

        });

        return e;
    }

    public static NodeEvent firstFall()
    {
        NodeEvent e = new NodeEvent("firstFall");
        e.status = NodeEvent.EventStatus.Incomplete;


        //What happens when you succeed
        e.successDelegate += () =>
        {
            e.node.room.unlockRoom();
            GameController.StopListening(e.successTrigger, e.successListener);
        };


        e.setupEnterAction(delegate
        {
            GameController.instance.message("You wake up in a dark cavern. You can't see much, but ahead of you, you see a torch that has just an ember left.");
            GameController.instance.message("You should probably take that torch.");

            GameController.instance.changeObjective("take the torch");
            e.node.room.lockRoom();
            e.node.room.description = "The room is extremely dark...";
            e.removeEnterAction();
        });

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
                            e.node.room.description = "The ground beneath you is very moist. A little too moist for your liking.";
                            e.removeAction("use bottle on torch");

                            e.addAction("look around", delegate ()
                            {
                                string dir = "";
                                MapNode.Direction dirS = e.node.exits[0];
                                dir = dirS.ToString().ToLower();

                                GameController.instance.message("To the " + dir + " you see an opening that you can go through.");
                                GameController.instance.message("That seems like a good direction to go in.");
                                GameController.instance.changeObjective("Exit the floor: Use the 'move/go' command");
                                GameController.TriggerEvent(e.successTrigger);
                                e.removeAction("look around");
                            });

                        });

                    });

                });

            });

        });

        return e;
    }

    public static NodeEvent skeletonPuzzzle1()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("skeleton1");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.onInit += () =>
        {
            e.node.room.description = "You see a stone table in the room. On top of this table is a skeleton. It's missing its head";
        };

        e.setupEnterAction(delegate
        {

            e.node.room.description = "You see a stone table in the room. On top of this table is a skeleton. It's missing its head";

            if (cont.roomNavigation.traversedNodes.Contains(e.node) == false)
            {
                e.node.room.lockRoom();
                GameController.instance.message("You hear a door lock in the distance...");
                cont.changeObjective("Escape the room.");
            }
            GameController.instance.message("You notice that there's a human skull on the ground.");

            e.addAction("take skull", delegate
            {
                if (cont.roomNavigation.currentNode == e.node)
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

    public static NodeEvent skeletonPuzzzle2()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("skeleton2");
        e.status = NodeEvent.EventStatus.Incomplete;


        e.setupEnterAction(delegate
        {
            GameController.instance.message("You rub against a stone table with a skeleton on it. It's missing its head");

            e.addAction("place skull on table", delegate
            {
                if (cont.roomNavigation.currentNode == e.node)
                {
                    if (cont.playerInfo.hasItem("skull"))
                    {
                        cont.playerInfo.removeInventory("skull");
                        cont.message("You place the skull on the table and complete the skeleton.");
                        cont.message("The ground rumbles beneath you. You see a stone slab in the distance move, revealing a stairway.");

                        e.node.room.description = "You see a stone table in the room. On top of this table is a fully built skeleton.";
                        e.removeAction("place skull on table");
                        e.node.room.unlockRoom();
                        e.removeEnterAction();
                    }
                }
            });


        });

        return e;
    }

    public static NodeEvent crowbarPuzzle1()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("crowbar1");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.onInit += () =>
        {
            e.node.room.description = "You notice a large lever in the the room.";
        };

        //What happens when you succeed
        e.successDelegate += () =>
        {
            e.node.room.unlockRoom();
            cont.message("The gates surrounding the room open once more!");
            cont.changeObjective("Exit to the next floor.");
            GameController.StopListening(e.successTrigger, e.successListener);
        };

        //What happens when you fail
        e.failureDelegate += () =>
        {
            GameController.instance.message("You drown in the room...");
            GameController.StopListening(e.failureTrigger, e.failureListener);
        };

        e.setupEnterAction(delegate
        {
            GameController.instance.message("You notice a lever in the side of the room.");

            e.addAction("pull lever", delegate
            {
                if (cont.roomNavigation.currentNode == e.node)
                {
                    cont.message("You pull the lever");
                    cont.message("The lever handle breaks as you pull it all the way down.");
                    e.node.room.description = "You notice a large broken lever in the the room.";
                    e.node.room.lockRoom();
                    cont.message("You notice water seeping out from the sides, and the gates surrounding the room lock.");
                    cont.changeObjective("Escape the room.");

                    e.removeAction("pull lever");

                    int counter = 0;
                    e.addAction("any", delegate
                    {
                        counter++;
                        switch (counter)
                        {
                            case 0:
                                cont.message("The water level is currently at your shoes.");
                                break;
                            case 1:
                                cont.message("The water level is currently at your knees.");
                                break;
                            case 2:
                                cont.message("The water level is currently at your hips. You begin to panic.");
                                break;
                            case 3:
                                cont.message("The water level is currently at your torso. This is not looking too good...");
                                break;
                            case 4:
                                cont.message("The water level is currently at your shoulders. It seems the end is nigh...");
                                break;
                            case 5:
                                cont.message("The water level is currently above your head. You struggle to breathe. You see a faint light...");
                                GameController.TriggerEvent(e.failureTrigger);
                                break;
                            default:
                                break;
                        }
                    });

                    e.addAction("pull lever", delegate
                    {
                        if (e.node == cont.roomNavigation.currentNode)
                        {
                            cont.message("The lever handle is broken...");
                        }
                    });

                    e.node.room.addRoomEvent(crowbarPuzzle3(e));

                }
            });

        });

        return e;
    }

    public static NodeEvent crowbarPuzzle2()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("crowbar2");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.setupEnterAction(delegate
        {
            GameController.instance.message("You feel a crowbar at your feet");

            e.addAction("take crowbar", delegate
            {
                if (e.node == cont.roomNavigation.currentNode)
                {
                    cont.message("You take the crowbar");
                    cont.playerInfo.addInventory("crowbar");
                    e.removeAction("take crowbar");
                    GameController.TriggerEvent(e.successTrigger);
                }
            });

        });

        return e;
    }

    public static NodeEvent crowbarPuzzle3(NodeEvent e1)
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("crowbar3");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.setupEnterAction(delegate
        {
            GameController.instance.message("You see a drainpipe blocked by a large steel beam");

            e.addAction("examine drainpipe", delegate
            {
                if (e.node == cont.roomNavigation.currentNode)
                {
                    cont.message("You see a drainpipe blocked by a large steel beam");

                    if (cont.playerInfo.hasItem("crowbar") == false)
                    {
                        cont.message("You dont seem to be able to move the steel beam without some assistance.");
                    }
                    else
                    {
                        cont.message("Maybe you can move the steel beam somehow.");
                    }
                }
            });

            e.addAction("use crowbar", delegate
            {
                if (e.node == cont.roomNavigation.currentNode)
                {
                    if (cont.playerInfo.hasItem("crowbar"))
                    {
                        cont.message("You attempt to move the large steel beam away from the drainpipe with the crowbar");
                        cont.message("With your strength,the beam is now open for water to flow.");
                        cont.message("All of the water in the room flows into the pipe, and the crowbar along with it.");
                        cont.playerInfo.removeInventory("crowbar");
                        GameController.TriggerEvent(e1.successTrigger);
                        GameController.TriggerEvent(e.successTrigger);
                    }
                }
            });

        });

        return e;
    }

}