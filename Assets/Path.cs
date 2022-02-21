using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color LineColor;
    private List<Transform> nodes = new List<Transform>();

    void OnDrawGizmos()
    {
        Gizmos.color = LineColor;

        Transform [] pathtransform = GetComponentsInChildren<Transform>();
        nodes = getnodes(pathtransform);    
        

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currtNode = nodes[i].position;
            Vector3 prevNode = Vector3.zero;
            if (i > 0)
            { prevNode = nodes[i - 1].position; }
            else if (i == 0 && nodes.Count > 1)
            { prevNode = nodes[nodes.Count - 1].position; }

            Gizmos.DrawLine(prevNode,currtNode);

        }

    }

    private List<Transform> getnodes(Transform[] pathtransform)
    {
        List<Transform> nodes = new List<Transform>();
        for (int i = 0; i < pathtransform.Length; i++)
            if (pathtransform[i] != transform)
                nodes.Add(pathtransform[i]);

        return nodes;
    }
}
