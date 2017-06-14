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

    public static NodeEvent firstFall()
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

        e.addAction("take torch", delegate ()
        {
            GameController.instance.message("You take the torch");
            GameController.instance.message("Even if the torch is about to die out, you should probably look around the room");
            GameController.instance.changeObjective("Look Around the room");
            e.removeAction("take torch");


            e.addAction("look around", delegate ()
            {
                GameController.instance.message("You look around the cavernous room.");
                GameController.instance.message("With what little light the torch gives, you see a small bottle on the ground.");
                GameController.instance.changeObjective("Grab the bottle off the ground");

                e.removeAction("look around");
                e.addAction("take bottle", delegate ()
                {
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
                            GameController.instance.message("You pour some of the fuel onto the torch, and the embers grow into a healthy flame.");
                            GameController.instance.message("You can see a bit better now.");
                            GameController.instance.changeObjective("Use your newly gained vision to look around the room");
                            e.removeAction("use bottle on torch");

                            e.addAction("look around", delegate ()
                            {
                                GameController.instance.message("You look around the cavernous room. The ground beneath you is very moist. A little too moist for your liking.");
                                GameController.instance.message("To the [direction] you see an opening that you can go through.");
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

}
