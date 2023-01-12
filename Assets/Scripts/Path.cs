using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Transform> nodes;

    private void Awake()
    {
        GetNodes();
    }

    public void GetNodes()
    {
        nodes = GetComponentsInChildren<Transform>().ToList();
        nodes.RemoveAt(0);
    }

    // private void OnDrawGizmos()
    // {
    //     for (int i = 0; i < nodes.Count; i++)
    //     {
    //         Gizmos.color = Color.green;
    //         if (i == nodes.Count - 1)
    //         {
    //             // Gizmos.DrawLine(nodes[i].position, nodes[0].position);
    //         }
    //         else
    //         {
    //             Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
    //         }
    //     }
    // }
}