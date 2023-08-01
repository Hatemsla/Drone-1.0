using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Builder;
using UnityEngine;

public class DroneBuilderCheckNode : CheckNode
{
    private void Update()
    {
        CalculateWayDistance();
    }

    private void CalculateWayDistance()
    {
        if(currentNode >= nodes.Count)
            return;
    }

    public void CheckWaypoint()
    {
        currentNode++;
    }

    public void AddNode(Transform t)
    {
        nodes.Add(t);
        SetCheckpointsId();
    }

    public void RemoveNode(Transform t)
    {
        nodes.Remove(t);
        SetCheckpointsId();
    }

    private void SetCheckpointsId()
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            nodes[i].GetComponent<BuilderCheckpointTrigger>().checkpointId = i;
        }
    }
}
