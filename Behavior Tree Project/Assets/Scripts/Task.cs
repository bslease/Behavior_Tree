using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task
{
    // Return on success (true) or failure (false)
    public abstract bool run();
}

public class Sequence : Task
{
    List<Task> children;

    public Sequence(List<Task> taskList)
    {
        children = taskList;
    }

    // Sequence wants all tasks to succeed
    // try all tasks in order
    // stop and return false on the first task that fails
    // return true if all tasks succeed
    public override bool run()
    {

        foreach (Task c in children)
        {
            if (!c.run())
            {
                return false;
            }
        }
        return true;
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

public class MoveTo : Task
{
    Vector3 targetPosition;

    public MoveTo(Vector3 somePosition)
    {
        targetPosition = somePosition;
    }

    public override bool run()
    {
        Debug.Log("Moving to target position: " + targetPosition);
        return true;
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
        //mDoor.AddExplosionForce(10f, mDoor.transform.position, 5f);
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
        mEntity.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        return true;
    }
}