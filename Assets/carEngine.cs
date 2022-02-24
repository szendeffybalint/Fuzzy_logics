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

    [SerializeField] private float PeopleRangeCheck = 10f;
    [SerializeField] private float CarsRangeCheck = 10f;

    private float speedOfCar = 0f; 
    private bool isGoingToTurn = false;
    private List<Transform> nodes = new List<Transform>();
    private int currentNode = 0;
    private float VehicleMaxSpeedHolder;
    private GameObject[] People;
    private GameObject[] Cars;
    private bool isStop = false;
    private bool turned = false;
    private enum GameObjectsToCheck
    {
        People,
        Cars
    }
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
        speedOfCar = this.GetComponent<Rigidbody>().velocity.sqrMagnitude;
        //wheels of car 
        ApplySteer();
        
        //if needed
        //set the cars speed 
        CheckAndGoNextWaypoint();

        //if needed
        CheckCarsAndPeople();

        //if ppl / car 
        if (isStop)
            Brake();

        //drive with the speed
        if(!isStop)
            Drive();
        //
    }

    private void CheckCarsAndPeople()
    {
        if (checkobjects(GameObjectsToCheck.Cars) || checkobjects(GameObjectsToCheck.People))
            isStop = true;
        else
            isStop = false;
    }

    private bool checkobjects(GameObjectsToCheck objecttype)
    {
        GameObject[] Objectstocheck;
        float rangecheck = 0f;
        switch (objecttype)
        {
            case GameObjectsToCheck.People:
                Objectstocheck = People;
                rangecheck = PeopleRangeCheck;
                break;
            case GameObjectsToCheck.Cars:
                Objectstocheck = Cars;
                rangecheck = CarsRangeCheck;
                break;
            default:
                Objectstocheck = new GameObject[0];
                break;
        }
        foreach (var Gameobj in Objectstocheck)
        {
            
            if (Gameobj == this.gameObject)
                continue;
            //Debug.Log(Vector3.Distance(transform.position, Gameobj.transform.position) + " at: " + Gameobj.name);
            if (Vector3.Distance(transform.position, Gameobj.transform.position) <= rangecheck)
            {
                Vector3 directionToGo = (nodes[currentNode].transform.position - transform.position);
                Vector3 directionToTarget = (Gameobj.transform.position - transform.position);
                float angle = Vector3.Angle(directionToGo, directionToTarget);
                float distance = directionToTarget.magnitude;
                //Debug.Log(angle + " at: " + Gameobj.name);
                if (Mathf.Abs(angle) < getangle(distance, objecttype))
                    return true;
            }
        }
        return false;
    }
    private float getangle(float distance, GameObjectsToCheck objecttype)
    {
        if (isGoingToTurn && objecttype == GameObjectsToCheck.Cars)
            return 1200/distance;
        return 140 / distance;
    }
    private void Brake()
    {
        //Debug.Log("Brake");
        if (LeftWheel.motorTorque > 0)
        {
            slowDown(1);
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
       // Debug.Log("Brake");

        if (speedOfCar < m_VehicleMaxSpeed)
        {
            speedUP();
        }
        else if (speedOfCar > m_VehicleMaxSpeed && isGoingToTurn)
        {
            slowDown(2);
        }
        //Debug.Log(speed);
    }

    private void slowDown(int FramesToStop)
    {   
        //the 20 is hardcoded, it s a num which help car stay in the path
        if(isGoingToTurn)
            this.transform.GetComponent<Rigidbody>().drag = speedOfCar / 30;
        LeftWheel.motorTorque -= LeftWheel.motorTorque  > 0 ? Acceleration/ FramesToStop : 0;
        RightWheel.motorTorque -= RightWheel.motorTorque > 0 ? Acceleration/ FramesToStop : 0;
    }

    private void speedUP()
    {
       // Debug.Log(isGoingToTurn);
        if(!isGoingToTurn)
            this.transform.GetComponent<Rigidbody>().drag = 0.0f;
        
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
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 1.99f)
        {
            //isGoingToTurn = true;
            if (currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
            //Debug.Log("Turned");
            m_VehicleMaxSpeed = VehicleMaxSpeedHolder;
            turned = true;
        }
        else if (Vector3.Distance(transform.position, nodes[currentNode].position) < speedOfCar/10)
        {
            isGoingToTurn = true;
            m_VehicleMaxSpeed = VehicleMaxSpeedHolder / 2;

            //Debug.Log(Vector3.Distance(transform.position, nodes[currentNode].position));
        }
        else if (turned)
            isGoingToTurn = false;

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
