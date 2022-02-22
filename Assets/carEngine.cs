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
    private float VehicleMaxSpeedHolder;
    private GameObject[] People;
    private GameObject[] Cars;
    private bool isStop = false;
    [SerializeField]float maxPeopleRange = 100f;
    [SerializeField]float maxCarsRange = 100f;
    void Start()
    {
        Transform[] pathtransform = path.GetComponentsInChildren<Transform>();
        nodes = getnodes(pathtransform);
        Acceleration = Acceleration * 100;
        VehicleMaxSpeedHolder = m_VehicleMaxSpeed;
        People = GameObject.FindGameObjectsWithTag("Person");
        Cars = GameObject.FindGameObjectsWithTag("Car");
    }


    void FixedUpdate()
    {
        ApplySteer();
        
        //if needed
        CheckAndGoNextWaypoint();

        //if needed
        CheckCarsAndPeople();

        if (isStop)
            Brake();

        if(!isStop)
            Drive();
        //
    }

    private void CheckCarsAndPeople()
    {
        if (checkcars() || checkpeople())
            isStop = true;

        else
            isStop = false;
    }

    private bool checkpeople()
    {
        foreach (var person in People)
        { 
            if (Vector3.Distance(transform.position, person.transform.position) < maxPeopleRange)
            {
                Vector3 direction = (nodes[currentNode].transform.position - transform.position);
                Vector3 direction2 = (person.transform.position - transform.position);
                Ray rayToNode = new Ray(transform.position, direction);
                Debug.DrawRay(transform.position, nodes[currentNode].transform.position-transform.position,Color.red);
                Debug.DrawRay(transform.position, direction, Color.red);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(rayToNode, out hit))
                    if (hit.collider.gameObject.tag == "Person")
                        return true;
            }
        }
        return false;
    }

    private bool checkcars()
    {
        foreach (var car in Cars)
        {
            if (Vector3.Distance(transform.position, car.transform.position) < maxCarsRange)
            {
                Vector3 direction = (nodes[currentNode].transform.position - transform.position);
                Vector3 direction2 = (car.transform.position - transform.position);
                Ray rayToNode = new Ray(transform.position, direction);
                Debug.DrawRay(transform.position, nodes[currentNode].transform.position - transform.position, Color.red);
                Debug.DrawRay(transform.position, direction, Color.red);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(rayToNode, out hit))
                    if (hit.collider.gameObject.tag == "Car")
                        return true;
            }
        }
        return false;
    }

    private void Brake()
    {
        if (LeftWheel.motorTorque > 0)
        {
            slowDown(10);
        }
        else
        {
            //Debug.Log("stopped");
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
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
            slowDown(5);
        }
       // Debug.Log(speed);
    }

    private void slowDown(int FramesToStop)
    {
        LeftWheel.motorTorque -= LeftWheel.motorTorque  > 0 ? Acceleration/ FramesToStop : 0;
        RightWheel.motorTorque -= RightWheel.motorTorque > 0 ? Acceleration/ FramesToStop : 0;
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
    private void CheckAndGoNextWaypoint()
    {
        if (Vector3.Distance(transform.position,nodes[currentNode].position) < 1.99f)
        {
            if (currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
            //Debug.Log("Turned");
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
