using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruceBanner : MonoBehaviour
{
    public Door theDoor;
    public GameObject theTreasure;
    public GameObject test;

    // Start is called before the first frame update
    void Start()
    {
        // create our behavior tree based on Millington pg. 344
        // building from the bottom up

        // if door isn't locked, open it
        List<Task> taskList = new List<Task>();
        Task isDoorNotLocked = new IsFalse(theDoor.isLocked);
        //Task waitABeat = new Pause(0.5f);
        Task openDoor = new OpenDoor(theDoor);
        taskList.Add(isDoorNotLocked);
        //taskList.Add(waitABeat);
        taskList.Add(openDoor);
        Sequence openUnlockedDoor = new Sequence(taskList);

        // barge a closed door
        //taskList.Clear();
        taskList = new List<Task>();
        Task isDoorClosed = new IsFalse(theDoor.isOpen);
        Task hulkOut = new HulkOut(this.gameObject);
        Task bargeDoor = new BargeDoor(theDoor.transform.GetChild(0).GetComponent<Rigidbody>());
        taskList.Add(isDoorClosed);
        //taskList.Add(waitABeat);
        taskList.Add(hulkOut);
        taskList.Add(bargeDoor);
        Sequence bargeClosedDoor = new Sequence(taskList);

        // open a closed door, one way or another
        //taskList.Clear();
        taskList = new List<Task>();
        taskList.Add(openUnlockedDoor);
        taskList.Add(bargeClosedDoor);
        Selector openTheDoor = new Selector(taskList);

        // get the treasure when the door is closed
        //taskList.Clear();
        taskList = new List<Task>();
        Task moveToDoor = new MoveKinematicToObject(this.GetComponent<Kinematic>(), theDoor.gameObject);
        Task moveToTreasure = new MoveKinematicToObject(this.GetComponent<Kinematic>(), theTreasure.gameObject);
        //Task moveToTest = new MoveKinematicToObject(this.GetComponent<Kinematic>(), test.gameObject);
        taskList.Add(moveToDoor);
        //taskList.Add(moveToTest);
        //taskList.Add(waitABeat);
        taskList.Add(openTheDoor); // one way or another
        taskList.Add(moveToTreasure);
        Sequence getTreasureBehindClosedDoor = new Sequence(taskList);

        // get the treasure when the door is open 
        //taskList.Clear();
        taskList = new List<Task>();
        Task isDoorOpen = new IsTrue(theDoor.isOpen);
        taskList.Add(isDoorOpen);
        //taskList.Add(moveToTest);
        taskList.Add(moveToTreasure);
        Sequence getTreasureBehindOpenDoor = new Sequence(taskList);

        // get the treasure, one way or another
        //taskList.Clear();
        taskList = new List<Task>();
        taskList.Add(getTreasureBehindOpenDoor);
        taskList.Add(getTreasureBehindClosedDoor);
        Selector getTreasure = new Selector(taskList);

        getTreasure.run();
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    //public void RunSequenceWithDelay(List<Task> taskList)
    //{
    //    StartCoroutine(delayMe(taskList));
    //}

    //IEnumerator delayMe(List<Task> taskList)
    //{
    //    foreach (Task t in taskList)
    //    {
    //        t.run();
    //        yield return new WaitForSeconds(3);
    //    }
    //}
}
