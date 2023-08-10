using Drone.DroneFootball;
using UnityEngine;

namespace Drone.CustomTimeRewind
{
    public class ScoreRewind : RewindAbstract
    {
        public CheckNode checkNode;
        private CircularBuffer<int> _trackedCurrentNodes;

        private void Start()
        {
            _trackedCurrentNodes = new CircularBuffer<int>();
        }

        public override void Track()
        {
            TrackTargetTransform();
        }

        public override void Rewind(float seconds)
        {
            RestoreTargetTransform(seconds);
        }

        private void RestoreTargetTransform(float seconds)
        {
            checkNode.currentNode = _trackedCurrentNodes.ReadFromBuffer(seconds);
        }

        private void TrackTargetTransform()
        {
            _trackedCurrentNodes.WriteLastValue(checkNode.currentNode);
        }
    }
}