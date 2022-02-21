using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45f;
    public WheelCollider LeftWheel;
    public WheelCollider RightWheel;
    public float Acceleration = 5f;
    public float m_VehicleMaxSpeed = 40f;

    private bool isGoingToTurn = false;
    private List<Transform> nodes = new List<Transform>();
    private int currentNode = 0;
    private float GoodAngle;
    private float VehicleMaxSpeedHolder;
    void Start()
    {
        Transform[] pathtransform = path.GetComponentsInChildren<Transform>();
        nodes = getnodes(pathtransform);
        Acceleration = Acceleration * 100;
        VehicleMaxSpeedHolder = m_VehicleMaxSpeed;
    }


    void FixedUpdate()
    {
        ApplySteer();
        Drive();
        GoNextWaypoint();
    }

    private void Drive()
    {
        float speed = this.GetComponent<Rigidbody>().velocity.sqrMagnitude;
        if (speed < m_VehicleMaxSpeed)
        {
            speedUP();
        }
        else if (speed > VehicleMaxSpeedHolder && isGoingToTurn)
        {
            slowDown();
        }
        Debug.Log(speed);
    }

    private void slowDown()
    {
        LeftWheel.motorTorque -= Acceleration;
        RightWheel.motorTorque -= Acceleration;
    }

    private void speedUP()
    {
        LeftWheel.motorTorque = Acceleration;
        RightWheel.motorTorque = Acceleration;
    }

    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude)*maxSteerAngle;
        LeftWheel.steerAngle = newSteer;
        RightWheel.steerAngle = newSteer;
        
    }
    private void GoNextWaypoint()
    {
        if (Vector3.Distance(transform.position,nodes[currentNode].position) < 1.99f)
        {
            if (currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
            Debug.Log("Turned");
            VehicleMaxSpeedHolder = m_VehicleMaxSpeed;
        }
        else if (Vector3.Distance(transform.position, nodes[currentNode].position) < 4.99f)
        {
            isGoingToTurn = true;
            VehicleMaxSpeedHolder = m_VehicleMaxSpeed/2;
            //Debug.Log(Vector3.Distance(transform.position, nodes[currentNode].position));
        }

    }
    private List<Transform> getnodes(Transform[] pathtransform)
    {
        List<Transform> nodes = new List<Transform>();
        for (int i = 0; i < pathtransform.Length; i++)
            if (pathtransform[i] != path.transform)
                nodes.Add(pathtransform[i]);

        return nodes;
    }

}
