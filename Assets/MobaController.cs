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
    // movement
    NavMeshAgent agent;

    public float rotateSpeedMovement = 0.1f;
    float rotateVelocity;
    // Start is called before the first frame update
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
        {
            invoke();
        }
        camerafollow();
        move_player();
        
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
