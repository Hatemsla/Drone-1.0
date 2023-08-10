using System;
using Drone.Builder;
using Drone.DroneFootball;

namespace Drone.CustomTimeRewind
{
    public class YawRewind : RewindAbstract
    {
        public DroneController droneController;
        private CircularBuffer<float> _trackedDroneYaws;

        private void Start()
        {
            _trackedDroneYaws = new CircularBuffer<float>();
        }

        public override void Track()
        {
            TrackDroneYaw();
        }

        public override void Rewind(float seconds)
        {
            RestoreDroneYaw(seconds);
        }

        public void TrackDroneYaw()
        {
            _trackedDroneYaws.WriteLastValue(droneController.yaw);
        }

        public void RestoreDroneYaw(float seconds)
        {
            droneController.yaw = _trackedDroneYaws.ReadFromBuffer(seconds);
        }
    }
}