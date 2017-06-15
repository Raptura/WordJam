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

        e.onInit += () =>
        {
            e.node.room.description = "You see the stairs to the next floor";
        };

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
            }

        });

        return e;
    }



    public static NodeEvent exitWell()
    {
        NodeEvent e = new NodeEvent("exitwell");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.onInit += () =>
        {
            e.node.room.description = "You see the light to the top of the well";
        };

        e.setupEnterAction(delegate
        {
            e.node.room.description = "You see the light to the top of the well";
            GameController.instance.message("You see the bright light beaming to the outside world");
            GameController.instance.changeObjective("Exit the well.");
        });

        e.addAction("exit", delegate
        {
            if (GameController.instance.roomNavigation.currentNode == e.node)
            {
                GameController.instance.message("You finally exit the well. It was an experience you can tell your parents about!");
                GameController.instance.message("Say 'goodbye' to exit the well");
                GameController.instance.changeObjective("Say 'goodbye' at the well exit.");
                e.addAction("goodbye", delegate
                {
                    if (GameController.instance.roomNavigation.currentNode == e.node)
                    {
                        GameController.ExitWell();
                    }

                });
            }

        });

        return e;
    }

    //First Fall
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

    //Skeleton Puzzle

    public static NodeEvent skeletonPuzzzle()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("skeleton1");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.onInit += () =>
        {
            e.node.room.description = "You see a stone table in the room. On top of this table is a skeleton. It's missing its head";
            e.node.room.addRoomEvent(skeletonPuzzzleTable());
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

    private static NodeEvent skeletonPuzzzleTable()
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

    //Crowbar Puzzle

    public static NodeEvent crowbarPuzzle()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("crowbar1");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.onInit += () =>
        {
            e.node.room.description = "You notice a large lever in the the room.";
            e.node.room.addRoomEvent(crowBarPuzzle1());
            e.node.room.addRoomEvent(crowbarPuzzle2(e));
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

                    int counter = 0;
                    e.addAction("any", delegate
                    {
                        counter++;
                        switch (counter)
                        {
                            case 1:
                                cont.message("The water level is currently at your shoes.");
                                break;
                            case 2:
                                cont.message("The water level is currently at your knees.");
                                break;
                            case 3:
                                cont.message("The water level is currently at your hips. You begin to panic.");
                                break;
                            case 4:
                                cont.message("The water level is currently at your torso. This is not looking too good...");
                                break;
                            case 5:
                                cont.message("The water level is currently at your shoulders. It seems the end is nigh...");
                                break;
                            case 6:
                                cont.message("The water level is currently above your head. You struggle to breathe. You see a faint light...");
                                GameController.TriggerEvent(e.failureTrigger);
                                break;
                            default:
                                Debug.Log("nothing is happening...");
                                break;
                        }
                        Debug.Log("Counter" + counter);
                    });

                    e.removeAction("pull lever");


                    e.addAction("pull lever", delegate
                    {
                        if (e.node == cont.roomNavigation.currentNode)
                        {
                            cont.message("The lever handle is broken...");
                        }
                    });

                }
            });

        });

        return e;
    }

    private static NodeEvent crowBarPuzzle1()
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

    private static NodeEvent crowbarPuzzle2(NodeEvent e1)
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


    //Stone Puzzle

    public static NodeEvent stonePuzzle()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("stonePuzzleStart");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.onInit += () =>
        {
            e.node.room.description = "There's a large circle painted on a smooth wall. Inside the circle are three shallow holes, aligned horizontally.";

            e.node.room.addRoomEvent(stonePuzzleNote());
            e.node.room.addRoomEvent(stonePuzzleBlack());
            e.node.room.addRoomEvent(stonePuzzleBlue());
            e.node.room.addRoomEvent(stonePuzzleGreen());
            e.node.room.addRoomEvent(stonePuzzleRed());
        };

        e.setupEnterAction(delegate
        {
            cont.message("There's a large circle painted on a smooth wall. Inside the circle are three shallow holes, aligned horizontally.");
        });

        e.addAction("look circle", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                cont.message("Just a painted circle. You're more interested in the holes, though.");

                e.removeEnterAction();
            }
        });

        e.addAction("look holes", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                cont.message("Theres are holes on the left, middle, and right of the circle.");
                cont.message("Hmm. The left hole seems more moist than the others. There's actually water dripping from it.");
            }
        });

        e.addAction("place green stone in right hole", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                if (cont.playerInfo.hasItem("green stone"))
                {
                    cont.playerInfo.removeInventory("green stone");
                    cont.message("You put the green stone in the right hole.");

                    e.removeAction("place green stone in right hole");
                }
            }
        });

        e.addAction("place red stone in middle hole", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                if (cont.playerInfo.hasItem("red stone"))
                {
                    cont.playerInfo.removeInventory("red stone");
                    cont.message("You put the red stone in the middle hole.");

                    e.removeAction("place red stone in middle hole");
                }
            }
        });

        e.addAction("place blue stone in left hole", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                if (cont.playerInfo.hasItem("blue stone"))
                {
                    cont.playerInfo.removeInventory("blue stone");
                    cont.message("You put the blue stone in the left hole.");
                    cont.message("Water starts running from the hole, slowly filling the room.");

                    e.removeAction("place blue stone in left hole");
                }
            }
        });

        e.addAction("use black stone on left hole", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                if (cont.playerInfo.hasItem("black stone"))
                {
                    cont.message("You use the black stone to dig open the running hole. Water starts to gush out even more. Are you sure about this?");

                    e.removeAction("use black stone in left hole");
                }
            }
        });

        return e;
    }

    private static NodeEvent stonePuzzleGreen()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("stoneGreen");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.setupEnterAction(delegate
        {
            GameController.instance.message("You notice a peculiar green stone.");
        });

        e.addAction("take green stone", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                string examine = "Very mossy.";
                cont.playerInfo.addInventory("green stone", examineText: examine);
                cont.message("You take the green stone");

                e.removeAction("take green stone");
                e.removeEnterAction();
            }
        });

        return e;
    }

    private static NodeEvent stonePuzzleRed()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("stoneRed");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.setupEnterAction(delegate
        {
            GameController.instance.message("You notice a shiny red stone.");
        });

        e.addAction("take red stone", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                string examine = "Warm to the touch.";
                cont.playerInfo.addInventory("red stone", examineText: examine);
                cont.message("You take the red stone");

                e.removeAction("take red stone");
                e.removeEnterAction();
            }
        });

        return e;
    }

    private static NodeEvent stonePuzzleBlue()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("stoneBlue");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.setupEnterAction(delegate
        {
            GameController.instance.message("You notice a sparkling blue stone.");
        });

        e.addAction("take blue stone", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                string examine = "Moist.";
                cont.playerInfo.addInventory("blue stone", examineText: examine);
                cont.message("You take the blue stone");

                e.removeAction("take blue stone");
                e.removeEnterAction();
            }
        });

        return e;
    }

    private static NodeEvent stonePuzzleBlack()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("stoneBlack");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.setupEnterAction(delegate
        {
            cont.message("You notice a sharp black stone.");
        });

        e.addAction("take black stone", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                string examine = "Sharp and pointy. You can probably dig with it.";
                cont.playerInfo.addInventory("black stone", examineText: examine);
                cont.message("You take the black stone");

                e.removeAction("take black stone");
                e.removeEnterAction();
            }
        });

        return e;
    }

    private static NodeEvent stonePuzzleNote()
    {
        GameController cont = GameController.instance;
        NodeEvent e = new NodeEvent("stoneNote");
        e.status = NodeEvent.EventStatus.Incomplete;

        e.setupEnterAction(delegate
        {
            cont.message("In this moist, moist room you see a skeleton resting against a wall. Its boney hands clutch on to a flimsy page.");

        });

        e.addAction("take page", delegate
        {
            if (cont.roomNavigation.currentNode == e.node)
            {
                string examine = "The scribbles on the page read\n\n" +
                "Dear Diary,\n" + "I've finally solved the wretched puzzle in the far-side room. " +
                "The green stone goes int he right slot, and blue stone to the left, and then the red stone in the middle. " +
                "I ain't figured out what the black stone is for. " +
                "However, I've inspected the holes for myself and I felt drips of water behind the holes. " +
                "Especially behind the left slot. " +
                "And aye, you know how my greatest fears include drowning in a cave. " +
                "You'd more quickly find me dead dry than drowned.";

                cont.playerInfo.addInventory("page", examineText: examine);
                cont.message("You take the page");

                e.removeAction("take page");
                e.removeEnterAction();

                e.node.room.description = "You see a skeleton resting against a wall.";

                e.addAction("look skeleton", delegate
                {
                    if (cont.roomNavigation.currentNode == e.node)
                    {
                        cont.message("The skeleton is indeed dry.");
                    }
                });
            }
        });

        return e;
    }

}