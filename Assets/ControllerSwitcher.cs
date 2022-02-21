using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllerSwitcher : MonoBehaviour
{
    private GameObject m_gameobject;
    public bool m_Isfps = true;
    public GameObject Eyes;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        m_gameobject = this.gameObject;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            if (m_Isfps)
                turninMOBA();
            else
                turninFPS();
        }
    }

    private void turninFPS()
    {
        m_gameobject.GetComponent<NavMeshAgent>().enabled = false;
        m_gameobject.GetComponent<MobaController>().enabled = false;
        m_gameobject.GetComponent<CapsuleCollider>().enabled = false;
        m_gameobject.GetComponent<FirstPersonController>().enabled = true;
        m_gameobject.GetComponent<CharacterController>().enabled = true;
        cam.transform.position = Eyes.transform.position; 
        cam.transform.rotation = Eyes.transform.rotation; 
        m_Isfps = true;
    }

    private void turninMOBA()
    {
        m_gameobject.GetComponent<FirstPersonController>().enabled = false;
        m_gameobject.GetComponent<CharacterController>().enabled = false;
        m_gameobject.GetComponent<NavMeshAgent>().enabled = true;
        m_gameobject.GetComponent<MobaController>().enabled = true;
        m_gameobject.GetComponent<CapsuleCollider>().enabled = true;
        cam.transform.localPosition = new Vector3(0.0f, 25.0f, -5.0f);
        cam.transform.localRotation = Quaternion.Euler(80f, 0, 0);
        m_Isfps = false;
    }
}
