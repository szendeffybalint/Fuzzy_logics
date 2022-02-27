using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobaController : MonoBehaviour
{

    //camera controller
    public Transform moba_player;
    private Vector3 cameraoffset;

    [Range(0.01f, 1.0f)]
    public float smoothness = 0.5f;
    public float rotateSpeedMovement = 0.1f;
    // movement
    public Animator Animator;
    NavMeshAgent agent;
    float rotateVelocity;

    void Start()
    {
        invoke();
    }

    private void invoke()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        moba_player = this.gameObject.transform;
        cameraoffset = transform.position - moba_player.transform.position;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        //fps controller volt
        if (!Cursor.visible)
            invoke();

        camerafollow();
        move_player();
        if (Vector3.Distance(transform.position, agent.destination) < 0.49f)
            agent.isStopped = true;
        float speed = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;
        handleAnimation(speed);
    }

    private void handleAnimation(float speed)
    {
        //redundant with HumanAIcontroller because it s easier to set different animations liek this

        //Debug.Log(speed);
        if (speed > 0.01f)
            Animator.SetBool("IsWalking", true);
        else
            Animator.SetBool("IsWalking", false);
    }

    private void move_player()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit rayhit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out rayhit,Mathf.Infinity))
            {
                //Move
                agent.SetDestination(rayhit.point);
                agent.isStopped = false;
                //rotate
                Quaternion rotationToLookAt = Quaternion.LookRotation(rayhit.point - transform.position);

                float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                    rotationToLookAt.eulerAngles.y, 
                    ref rotateVelocity, 
                    rotateSpeedMovement * (Time.deltaTime * 5));

                transform.eulerAngles = new Vector3(0,rotationY, 0);
            }
        }
    }

    private void camerafollow()
    {
        Vector3 newpos = moba_player.position + cameraoffset;
        transform.position = Vector3.Slerp(transform.position,newpos,smoothness);
    }
}
