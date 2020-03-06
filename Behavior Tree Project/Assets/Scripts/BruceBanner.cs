﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruceBanner : MonoBehaviour
{
    public Door theDoor;
    public GameObject theTreasure;
    public GameObject test;
    bool executingBehavior = false;
    Task myCurrentTask;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!executingBehavior)
            {
                executingBehavior = true;
                //myCurrentTask = BuildTask_Test();
                myCurrentTask = BuildTask_GetTreasue();

                myCurrentTask.eventId = EventBus.GetEventID();
                EventBus.StartListening("FinishedTask" + myCurrentTask.eventId, OnTaskFinished);
                myCurrentTask.run();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Help");
        }
    }

    void OnTaskFinished()
    {
        EventBus.StopListening("FinishedTask" + myCurrentTask.eventId, OnTaskFinished);
        Debug.Log("Behavior complete! Success = " + myCurrentTask.succeeded);
        executingBehavior = false;
    }

    Task BuildTask_Test()
    {
        List<Task> taskList = new List<Task>();

        // get the treasure when the door is open 
        taskList = new List<Task>();
        Task isDoorOpen = new IsTrue(theDoor.isOpen);
        Task moveToTreasure = new MoveKinematicToObject(this.GetComponent<Kinematic>(), theTreasure.gameObject);
        taskList.Add(isDoorOpen);
        taskList.Add(moveToTreasure);
        Sequence getTreasureBehindOpenDoor = new Sequence(taskList);

        //taskList = new List<Task>();
        Task moveToDoor = new MoveKinematicToObject(this.GetComponent<Kinematic>(), theDoor.gameObject);
        //Task openDoor = new OpenDoor(theDoor);
        //Task waitABeat = new Pause(0.5f);
        Task waitABeat = new Wait(1.5f);
        //taskList.Add(moveToDoor);
        //taskList.Add(openDoor);
        //taskList.Add(waitABeat);
        //taskList.Add(moveToTreasure);
        //Sequence getTreasureBehindOClosedDoor = new Sequence(taskList);
        //return getTreasureBehindOClosedDoor;

        taskList = new List<Task>();
        Task isDoorClosed = new IsFalse(theDoor.isOpen);
        Task hulkOut = new HulkOut(this.gameObject);
        Task bargeDoor = new BargeDoor(theDoor.transform.GetChild(0).GetComponent<Rigidbody>());
        taskList.Add(isDoorClosed);
        taskList.Add(moveToDoor);
        taskList.Add(hulkOut);
        taskList.Add(waitABeat);
        taskList.Add(bargeDoor);
        taskList.Add(waitABeat);
        taskList.Add(moveToTreasure);
        Sequence bargeClosedDoor = new Sequence(taskList);
        return bargeClosedDoor;

        /*
        // open a closed door, or don't
        taskList = new List<Task>();
        taskList.Add(getTreasureBehindOpenDoor);
        taskList.Add(getTreasureBehindOClosedDoor);
        taskList.Add(bargeClosedDoor);
        Selector getTreasure = new Selector(taskList);
        */
    }
    
    Task BuildTask_GetTreasue()
    {
        // create our behavior tree based on Millington pg. 344
        // building from the bottom up
        List<Task> taskList = new List<Task>();

        // if door isn't locked, open it
        Task isDoorNotLocked = new IsFalse(theDoor.isLocked);
        Task waitABeat = new Wait(0.5f);
        Task openDoor = new OpenDoor(theDoor);
        taskList.Add(isDoorNotLocked);
        taskList.Add(waitABeat);
        taskList.Add(openDoor);
        Sequence openUnlockedDoor = new Sequence(taskList);

        // barge a closed door
        taskList = new List<Task>();
        Task isDoorClosed = new IsFalse(theDoor.isOpen);
        Task hulkOut = new HulkOut(this.gameObject);
        Task bargeDoor = new BargeDoor(theDoor.transform.GetChild(0).GetComponent<Rigidbody>());
        taskList.Add(isDoorClosed);
        taskList.Add(waitABeat);
        taskList.Add(hulkOut);
        taskList.Add(waitABeat);
        taskList.Add(bargeDoor);
        Sequence bargeClosedDoor = new Sequence(taskList);

        // open a closed door, one way or another
        taskList = new List<Task>();
        taskList.Add(openUnlockedDoor);
        taskList.Add(bargeClosedDoor);
        Selector openTheDoor = new Selector(taskList);

        // get the treasure when the door is closed
        taskList = new List<Task>();
        Task moveToDoor = new MoveKinematicToObject(this.GetComponent<Kinematic>(), theDoor.gameObject);
        Task moveToTreasure = new MoveKinematicToObject(this.GetComponent<Kinematic>(), theTreasure.gameObject);
        taskList.Add(moveToDoor);
        taskList.Add(waitABeat);
        taskList.Add(openTheDoor); // one way or another
        taskList.Add(waitABeat);
        taskList.Add(moveToTreasure);
        Sequence getTreasureBehindClosedDoor = new Sequence(taskList);

        // get the treasure when the door is open 
        taskList = new List<Task>();
        Task isDoorOpen = new IsTrue(theDoor.isOpen);
        taskList.Add(isDoorOpen);
        taskList.Add(moveToTreasure);
        Sequence getTreasureBehindOpenDoor = new Sequence(taskList);

        // get the treasure, one way or another
        taskList = new List<Task>();
        taskList.Add(getTreasureBehindOpenDoor);
        taskList.Add(getTreasureBehindClosedDoor);
        Selector getTreasure = new Selector(taskList);

        return getTreasure;
    }
}
