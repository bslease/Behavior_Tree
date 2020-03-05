using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task
{
    // Return on success (true) or failure (false)
    public abstract void run();
    public bool succeeded;
    public int eventId;
    protected const string FINISHED_TASK = "FinishedTask";
}

public class IsTrue : Task
{
    bool varToTest;

    public IsTrue(bool someBool)
    {
        varToTest = someBool;
    }

    public override void run()
    {
        succeeded = varToTest;
        EventBus.TriggerEvent("FinishedTask" + eventId);
    }
}


public class IsFalse : Task
{
    bool varToTest;

    public IsFalse(bool someBool)
    {
        varToTest = someBool;
    }

    public override void run()
    {
        succeeded = !varToTest;
        EventBus.TriggerEvent("FinishedTask" + eventId);
    }
}

public class OpenDoor : Task
{
    Door mDoor;

    public OpenDoor(Door someDoor)
    {
        mDoor = someDoor;
    }

    public override void run()
    {
        succeeded = mDoor.Open();
        EventBus.TriggerEvent("FinishedTask" + eventId);
    }
}

public class BargeDoor : Task
{
    Rigidbody mDoor;

    public BargeDoor(Rigidbody someDoor)
    {
        mDoor = someDoor;
    }

    public override void run()
    {
        //Debug.Log("got here");
        //if (mDoor == null)
        //{
        //    Debug.Log("why?");
        //}
        //Debug.Log("barging door: " + mDoor);
        //Debug.Log("but not here??");
        mDoor.AddForce(-10f, 0, 0, ForceMode.VelocityChange);
        succeeded = true;
        EventBus.TriggerEvent("FinishedTask" + eventId);
    }
}

public class HulkOut : Task
{
    GameObject mEntity;

    public HulkOut(GameObject someEntity)
    {
        mEntity = someEntity;
    }

    public override void run()
    {
        //Debug.Log("hulking out");
        mEntity.transform.localScale *= 2;
        mEntity.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        succeeded = true;
        EventBus.TriggerEvent("FinishedTask" + eventId);
    }
}

// for some reason, this task will break if used before barge, hulk out, or open door
// it works okay before a moveto or an isfalse
public class Pause : Task
{
    Timer myTimer;

    public Pause(float time)
    {
        myTimer = new Timer(time * 1000f); // miliseconds!
        myTimer.AutoReset = false;
        myTimer.Elapsed += OnTimeElapsed;
    }

    public override void run()
    {
        //myTimer.Enabled = true;
        myTimer.Start();
    }

    void OnTimeElapsed(object source, ElapsedEventArgs e)
    {
        //myTimer.Enabled = false;
        myTimer.Stop();
        Debug.Log("Pause time elapsed.");
        succeeded = true;
        EventBus.TriggerEvent("FinishedTask" + eventId);
    }
}

public class MoveKinematicToObject : Task
{
    Arriver mMover;
    GameObject mTarget;

    public MoveKinematicToObject(Kinematic mover, GameObject target)
    {
        mMover = mover as Arriver;
        mTarget = target;
    }

    public override void run()
    {
        //Debug.Log("Moving to target position: " + mTarget);
        mMover.OnArrived += MoverArrived;
        mMover.myTarget = mTarget;
    }

    public void MoverArrived()
    {
        //Debug.Log("arrived at " + mTarget);
        mMover.OnArrived -= MoverArrived;
        succeeded = true;
        EventBus.TriggerEvent("FinishedTask" + eventId);
    }
}

public class Sequence : Task
{
    List<Task> children;
    Task currentTask;
    int currentTaskIndex = 0;

    public Sequence(List<Task> taskList)
    {
        children = taskList;
    }

    // Sequence wants all tasks to succeed
    // try all tasks in order
    // stop and return false on the first task that fails
    // return true if all tasks succeed
    public override void run()
    {
        Debug.Log("sequence running child task #" + currentTaskIndex);
        currentTask = children[currentTaskIndex];
        currentTask.eventId = EventBus.GetEventID();
        EventBus.StartListening("FinishedTask" + currentTask.eventId, OnChildTaskFinished);
        currentTask.run();
    }

    void OnChildTaskFinished()
    {
        Debug.Log("Behavior complete! Success = " + currentTask.succeeded);
        if (currentTask.succeeded)
        {
            EventBus.StopListening("FinishedTask" + currentTask.eventId, OnChildTaskFinished);
            currentTaskIndex++;
            if (currentTaskIndex < children.Count)
            {
                this.run();
            }
            else
            {
                // we've reached the end of our children and all have succeeded!
                succeeded = true;
                EventBus.TriggerEvent("FinishedTask" + eventId);
            }

        }
        else
        {
            // sequence needs all children to succeed
            // a child task failed, so we're done
            succeeded = false;
            EventBus.TriggerEvent("FinishedTask" + eventId);
        }
    }
}

public class Selector : Task
{
    List<Task> children;
    Task currentTask;
    int currentTaskIndex = 0;

    public Selector(List<Task> taskList)
    {
        children = taskList;
    }

    // Selector wants only the first task that succeeds
    // try all tasks in order
    // stop and return true on the first task that succeeds
    // return false if all tasks fail
    public override void run()
    {
        //Debug.Log("selector running child task #" + currentTaskIndex);
        currentTask = children[currentTaskIndex];
        currentTask.eventId = EventBus.GetEventID();
        EventBus.StartListening("FinishedTask" + currentTask.eventId, OnChildTaskFinished);
        currentTask.run();
    }

    void OnChildTaskFinished()
    {
        Debug.Log("Behavior complete! Success = " + currentTask.succeeded);
        if (currentTask.succeeded)
        {
            succeeded = true;
            EventBus.TriggerEvent("FinishedTask" + eventId);
        }
        else
        {
            EventBus.StopListening("FinishedTask" + currentTask.eventId, OnChildTaskFinished);
            currentTaskIndex++;
            if (currentTaskIndex < children.Count)
            {
                this.run();
            }
            else
            {
                // we've reached the end of our children and none have succeeded!
                succeeded = false;
                EventBus.TriggerEvent("FinishedTask" + eventId);
            }
        }
    }
}

