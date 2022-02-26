using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAIController : MonoBehaviour
{
    public Transform path;
    public Animator Animator;
    NavMeshAgent agent;

    private List<Transform> nodes = new List<Transform>();
    private int currentNode = 0;
    float rotateVelocity;

    void Start()
    {

        Transform[] pathtransform = path.GetComponentsInChildren<Transform>();
        nodes = getnodes(pathtransform);
        agent = this.gameObject.GetComponent<NavMeshAgent>();  

        //go to first waypoint
        MoveCharacter(); 
    }
    void Update()
    {
        //if needed
        CheckAndGoNextWaypoint();
        handleAnimation();
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
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 1.1f)
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
        //redundant with moba controller because it s easier to set different animations liek this
        float speed = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;
        //Debug.Log(speed);
        if (speed > 0.01f)
            Animator.SetBool("IsWalking", true);
        else
            Animator.SetBool("IsWalking", false);
    }
}
