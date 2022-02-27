using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 60f;
    public WheelCollider LeftWheel;
    public WheelCollider RightWheel;
    public float Acceleration = 5f;
    public float m_VehicleMaxSpeed = 40f;
    public bool isGoingToTurn = false;
    public bool isStop = false;
    public bool isPedestrianWannaCross = false;

    [SerializeField] private float PeopleRangeCheck = 10f;
    [SerializeField] private float CarsRangeCheck = 7f;
    private float speedOfCar = 0f; 
    private List<Transform> nodes = new List<Transform>();
    private int currentNode = 0;
    private float VehicleMaxSpeedHolder;
    private GameObject[] People;
    private GameObject[] Cars;
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
        //set wheels angle  
        ApplySteer();

        //if needed
        CheckAndSetNextWaypoint();

        //if needed
        CheckCarsAndPeople();

        //if ppl / car 
        if (isStop)
            Brake();

        //drive with the speed
        if(!isStop)
            Drive();
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
        //angle = divideWithMinMax(30, 120, 1200, distance);
        if (isGoingToTurn && objecttype == GameObjectsToCheck.Cars)
            return divideWithMinMax(30, 90, 1200, distance);

        return divideWithMinMax(15, 45, 120, distance);
    }

    private float divideWithMinMax(int min, int max, int num, float divider)
    {

        float divided = num / divider;
        if (divided > 90)
            return 0;
        divided = divided > max ? max : divided;
        divided = divided < min ? min : divided;
        return divided;
    }

    private void Brake()
    {
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //Debug.Log("Brake");
    }

    private void Drive()
    {
       // Debug.Log("Brake");

        if (speedOfCar < m_VehicleMaxSpeed)
            speedUP();

        else if (speedOfCar > m_VehicleMaxSpeed)
            slowDown();

        else
        {
            LeftWheel.motorTorque -= 0f;
            RightWheel.motorTorque -= 0f;
        }
        //Debug.Log(speed);
    }

    private void slowDown()
    {   if(isGoingToTurn)
            this.transform.GetComponent<Rigidbody>().drag = speedOfCar / 25.0f;
        LeftWheel.motorTorque = 0;
        RightWheel.motorTorque = 0;
    }

    private void speedUP()
    {
       // Debug.Log(isGoingToTurn);
        if(!isGoingToTurn)
            this.transform.GetComponent<Rigidbody>().drag = 0.0f;
        else
            this.transform.GetComponent<Rigidbody>().drag = speedOfCar / 25.0f;
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
    private void CheckAndSetNextWaypoint()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 2.0f)
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
        else if (Vector3.Distance(transform.position, nodes[currentNode].position) < speedOfCar / 25)
        {
            isGoingToTurn = true;
            m_VehicleMaxSpeed = VehicleMaxSpeedHolder / 2;

            //Debug.Log(Vector3.Distance(transform.position, nodes[currentNode].position));
        }
        else if (turned)
        {
            isGoingToTurn = false;
            turned = false;
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
