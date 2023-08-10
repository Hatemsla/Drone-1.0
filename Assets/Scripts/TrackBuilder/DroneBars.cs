using System.Collections.Generic;
using UnityEngine;

namespace Drone.Builder
{
    public class DroneBars : MonoBehaviour
    {
        public List<GameObject> bars;

        public void TurnBars(int barIndex)
        {
            for (var i = 0; i < bars.Count; i++)
            {
                bars[i].SetActive(i == barIndex);
            }
        }
    }
}