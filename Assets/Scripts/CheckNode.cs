using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNode : MonoBehaviour
{
    public int currentNode = 0;
    public int passedNode = 0;
    public float wayDistance;
    public List<Transform> nodes;

    private void Update()
    {
        wayDistance = Vector3.Distance(transform.position, nodes[currentNode].position);
    }

    public void CheckWaypoint()
    {
        if (currentNode == nodes.Count - 1)
        {
            currentNode = 0;
        }
        else
        {
            currentNode++;
        }

        // if (passedNode <= nodes.Count - 1)
        passedNode++;
    }
}
