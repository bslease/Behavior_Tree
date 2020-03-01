using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehavior
{
    public Kinematic character;
    public GameObject target;

    float maxAcceleration = 100f;
    float maxSpeed = 10f;

    // the radius for arriving at the target
    float targetRadius = 1.5f;

    // the radius for beginning to slow down
    float slowRadius = 3f;

    // the time over which to achieve target speed
    float timeToTarget = 0.1f;

    public override SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        // get the direction to the target
        Vector3 direction = target.transform.position - character.transform.position;
        float distance = direction.magnitude;

        //if (distance < targetRadius)
        //{
        //    return null;
        //}

        // if we are outside the slow radius, then move at max speed
        float targetSpeed = 0f;
        if (distance > slowRadius)
        {
            targetSpeed = maxSpeed;
        }
        else // otherwise calculate a scaled speed
        {
            //targetSpeed = -(maxSpeed * distance / slowRadius); // should slowRadius here instead be targetRadius?
            targetSpeed = maxSpeed * (distance - targetRadius) / targetRadius;
        }

        // the target velocity combines speed and direction
        Vector3 targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        // acceleration tries to get to the target velocity
        result.linear = targetVelocity - character.linearVelocity;
        result.linear /= timeToTarget;

        // check if the acceleration is too fast
        if (result.linear.magnitude > maxAcceleration)
        {
            result.linear.Normalize();
            result.linear *= maxAcceleration;
        }

        return result;
    }
}
