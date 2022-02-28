using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAIController : MonoBehaviour
{
    public Transform path;
    public Transform CrossWalk;
    public Animator Animator;
    public bool isStop;
    public bool isAtCrossWalk;

    NavMeshAgent agent;
    float rotateVelocity;
    float speed;
    private GameObject[] Cars;
    private List<Transform> nodes = new List<Transform>();
    private Transform[] CrossWalks; 
    private int currentNode = 0;
    private float maxWaitTime = 15f;
    private float timeOfWait = 0f;

    void Start()
    {

        Transform[] pathtransform = path.GetComponentsInChildren<Transform>();
        CrossWalks = CrossWalk.GetComponentsInChildren<Transform>();
        nodes = getnodes(pathtransform);
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        Cars = GameObject.FindGameObjectsWithTag("Car");
        //go to first waypoint
        MoveCharacter(); 
    }
    void Update()
    {
        isAtCrossWalk= IsAtCrossWalk();
        speed = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;
        CheckAndGoNextWaypoint();
        isStop = CheckCars();
        if (isStop)
            agent.velocity = Vector3.zero;
        else if (speed < 0.01f)
            MoveCharacter();

        handleAnimation();
    }

    private bool CheckCars()
    {
        
        if (isAtCrossWalk)
            foreach (var car in Cars)
            {
                if (car == this.gameObject)
                    continue;
                //Debug.Log(Vector3.Distance(transform.position, Gameobj.transform.position) + " at: " + Gameobj.name);
                if (Vector3.Distance(transform.position, car.transform.position) <= 6f)
                {
                    if (isStop)
                        timeOfWait += Time.deltaTime;
                    else 
                        timeOfWait = 0f;

                    Vector3 directionToGo = agent.velocity;
                    Vector3 directionToTarget = (car.transform.position - transform.position);
                    float angle = Vector3.Angle(directionToGo, directionToTarget);
                    //Debug.Log(angle + " at: " + Gameobj.name);
                    if (Mathf.Abs(angle) < 45 && !car.GetComponent<carEngine>().isStop  )
                        return true;
                    if (timeOfWait > maxWaitTime)
                        return false;
                    if (car.GetComponent<carEngine>().isStop && angle < 1 )
                        return true;
                }
            }
        return false;
    }

    private bool IsAtCrossWalk()
    {
        foreach (var crosswalk in CrossWalks)
        {
            //hardcoded because all of the crosswalks are 2f 'big'
            if (Vector3.Distance(transform.position, crosswalk.transform.position) <= 2.5f )
                return true;
        }
        return false;
    }

    private void MoveCharacter()
    {
        //move
        agent.SetDestination(nodes[currentNode].position);

        //rotate
        Quaternion rotationToLookAt = Quaternion.LookRotation(nodes[currentNode].position - transform.position);

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookAt.eulerAngles.y,
            ref rotateVelocity,
            0.2f * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationY, 0);
    }

    private void CheckAndGoNextWaypoint()
    {
        //Debug.Log(Vector3.Distance(transform.position, nodes[currentNode].position));
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 1.5f)
        {
            //isGoingToTurn = true;
            if (currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;

            MoveCharacter();
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
    private void handleAnimation()
    {
        //redundant with other controllers because it s easier to set different animations if needed

        if (speed > 0.01f && !isStop)
            Animator.SetBool("IsWalking", true);
        else
            Animator.SetBool("IsWalking", false);
    }
}
