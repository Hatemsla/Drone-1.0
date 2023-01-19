using System.Collections.Generic;
using DroneRace;
using UnityEngine;

namespace DroneFootball
{
    public class DroneFootballCheckNode : MonoBehaviour
    {
        public int currentNode;
        
        public void CheckWaypoint()
        {
            currentNode++;
        }
    }
}