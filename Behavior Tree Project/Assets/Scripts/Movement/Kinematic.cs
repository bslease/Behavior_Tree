using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    public Vector3 linearVelocity;
    public float angularVelocity;  // Millington calls this rotation
    // because I'm attached to a gameobject, we also have:
    // rotation <<< Millington calls this orientation
    // position
    public float maxSpeed = 10.0f;
    public float maxAngularVelocity = 45.0f; // degrees

    public GameObject myTarget;

    // child classes will get new steering data for use in our update function
    protected SteeringOutput steeringUpdate;

    // Start is called before the first frame update
    void Start()
    {
        steeringUpdate = new SteeringOutput(); // default to nothing. should be overriden by children
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // something is breaking my angular velocity
        // check here and reset it if it broke
        if (float.IsNaN(angularVelocity))
        {
            angularVelocity = 0.0f;
        }

        // update my position and rotation
        this.transform.position += linearVelocity * Time.deltaTime;
        if (Mathf.Abs(angularVelocity) > 0.01f)
        {
            Vector3 v = new Vector3(0, angularVelocity, 0);
            this.transform.eulerAngles += v * Time.deltaTime;
        }

        // update linear and angular velocities
        if (steeringUpdate != null)
        {
            linearVelocity += steeringUpdate.linear * Time.deltaTime;
            angularVelocity += steeringUpdate.angular * Time.deltaTime;
        }

        // check for speeding and clip
        if (linearVelocity.magnitude > maxSpeed)
        {
            linearVelocity.Normalize();
            linearVelocity *= maxSpeed;
        }
        if (Mathf.Abs(angularVelocity) > maxAngularVelocity)
        {
            angularVelocity = maxAngularVelocity * (angularVelocity / angularVelocity);
        }
    }

}
