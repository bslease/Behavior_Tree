using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arriver : Kinematic
{
    Arrive myMoveType;

    // Start is called before the first frame update
    void Start()
    {
        myMoveType = new Arrive();
        myMoveType.character = this;
        myMoveType.target = myTarget;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //steeringUpdate = new SteeringOutput();
        steeringUpdate = myMoveType.getSteering();
        base.Update();
    }
}
