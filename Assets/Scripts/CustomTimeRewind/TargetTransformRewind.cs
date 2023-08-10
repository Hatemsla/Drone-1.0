using Drone.DroneFootball;
using UnityEngine;

namespace Drone.CustomTimeRewind
{
    public class TargetTransformRewind : RewindAbstract
    {
        public DroneAI droneAi;
        private CircularBuffer<Transform> _trackedTargetTransforms;

        private void Start()
        {
            _trackedTargetTransforms = new CircularBuffer<Transform>();
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
            droneAi.targetTransform = _trackedTargetTransforms.ReadFromBuffer(seconds);
        }

        private void TrackTargetTransform()
        {
            _trackedTargetTransforms.WriteLastValue(droneAi.targetTransform);
        }
    }
}