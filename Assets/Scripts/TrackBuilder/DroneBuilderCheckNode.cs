using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Builder;
using UnityEngine;

public class DroneBuilderCheckNode : CheckNode
{
    public List<BuilderCheckpointTrigger> nodes;
    
    private void Update()
    {
        CalculateWayDistance();
    }

    public void CalculateWayDistance()
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
        nodes.Add(t.GetComponent<BuilderCheckpointTrigger>());
        SetCheckpointsId();
    }

    public void RemoveNode(Transform t)
    {
        nodes.Remove(t.GetComponent<BuilderCheckpointTrigger>());
        SetCheckpointsId();
    }

    private void SetCheckpointsId()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].checkpointId = i;
        }
    }
}
