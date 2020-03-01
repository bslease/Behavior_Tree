using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task
{
    // Return on success (true) or failure (false)
    public abstract bool run();
    public bool waitForCallback = false;
    //public delegate void TaskFinished();
    //public event TaskFinished OnTaskFinished;
    public event EventHandler<EventArgs> TaskFinished;
    protected virtual void OnTaskFinished(EventArgs e)
    {
        Debug.Log("task finished at " + Time.deltaTime);
        TaskFinished?.Invoke(this, e);
    }
}

public class Sequence : Task
{
    List<Task> children;
    int currentIndex = 0;

    public Sequence(List<Task> taskList)
    {
        children = taskList;
    }

    // Sequence wants all tasks to succeed
    // try all tasks in order
    // stop and return false on the first task that fails
    // return true if all tasks succeed
    //public override bool run()
    //{
    //    foreach (Task c in children)
    //    {
    //        if (!c.run())
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    public override bool run()
    {
        Debug.Log("currentIndex = " + currentIndex + " children.Count = " + children.Count);
        while (currentIndex < children.Count)
        {
            Task currentTask = children[currentIndex];
            if (!currentTask.run())
            {
                return false;
            }
            currentIndex++;
            if (currentTask.waitForCallback)
            {
                Debug.Log("setting up callback for task " + currentTask);
                currentTask.TaskFinished += HandleTaskFinished;
                break;
            }
        }

        // the problem is here... need to keep a list and only return true when all children finish succesfully
        return true;
    }

    void HandleTaskFinished(object sender, EventArgs e)
    {
        Debug.Log("sequence execution continuing at currentIndex " + currentIndex);
        this.run();
    }
}

public class Selector : Task
{
    List<Task> children;

    public Selector(List<Task> taskList)
    {
        children = taskList;
    }

    // Selector wants only the first task that succeeds
    // try all tasks in order
    // stop and return true on the first task that succeeds
    // return false if all tasks fail
    public override bool run()
    {
        foreach(Task c in children)
        {
            if (c.run())
            {
                return true;
            }
        }
        return false;
    }
}

public class EnemyNear : Task
{
    public Vector3 myPosition;
    public float nearDistance; // closer than this is considered "near"

    public override bool run()
    {
        // Task fails if there is no enemy nearby
        Vector3 enemyPosition = FindNearestEnemy();
        float distanceToEnemy = (enemyPosition - myPosition).magnitude;
        return distanceToEnemy < nearDistance;
    }

    Vector3 FindNearestEnemy()
    {
        // returns the position of the nearest enemy
        Vector3 enemyPosition = Vector3.positiveInfinity;
        return enemyPosition;
    }

}

public class IsTrue : Task
{
    bool varToTest;

    public IsTrue(bool someBool)
    {
        varToTest = someBool;
    }

    public override bool run()
    {
        return varToTest;
    }
}

public class IsFalse : Task
{
    bool varToTest;

    public IsFalse(bool someBool)
    {
        varToTest = someBool;
    }

    public override bool run()
    {
        return !varToTest;
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
        waitForCallback = true;
    }

    public override bool run()
    {
        mMover.OnArrived += MoverArrived;
        mMover.myTarget = mTarget;
        Debug.Log("Moving to target position: " + mTarget);
        return true;
    }

    public void MoverArrived()
    {
        mMover.OnArrived -= MoverArrived;
        Debug.Log("arrived at " + mTarget);
        OnTaskFinished(EventArgs.Empty);
    }

}

public class Pause : Task
{
    Timer myTimer;

    public Pause(float time)
    {
        myTimer = new Timer(time);
        myTimer.Elapsed += OnTimeElapsed;
        waitForCallback = true;
    }

    public override bool run()
    {
        myTimer.Enabled = true;
        return true;
    }

    void OnTimeElapsed(object source, ElapsedEventArgs e)
    {
        //myTimer.Enabled = false;
        myTimer.Stop();
        Debug.Log("Pause time elapsed.");
        OnTaskFinished(EventArgs.Empty);
    }
}

public class BargeDoor : Task
{
    Rigidbody mDoor;

    public BargeDoor(Rigidbody someDoor)
    {
        mDoor = someDoor;
    }

    public override bool run()
    {
        Debug.Log("barging door");
        mDoor.AddForce(-10f, 0, 0, ForceMode.VelocityChange);
        return true;
    }
}

public class OpenDoor : Task
{
    Door mDoor;

    public OpenDoor(Door someDoor)
    {
        mDoor = someDoor;
    }

    public override bool run()
    {
        return mDoor.Open();
    }
}

public class HulkOut : Task
{
    GameObject mEntity;

    public HulkOut(GameObject someEntity)
    {
        mEntity = someEntity;
    }

    public override bool run()
    {
        mEntity.transform.localScale *= 2;
        mEntity.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        return true;
    }
}
