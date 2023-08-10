namespace Drone.CustomTimeRewind
{
    public class TimerRewind : RewindAbstract
    {
        public Timer timer;
        private CircularBuffer<float> _trackedTime;
        private CircularBuffer<float> _trackedCurrentTime;

        private void Start()
        {
            _trackedTime = new CircularBuffer<float>();
            _trackedCurrentTime = new CircularBuffer<float>();
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
            timer.waitForEndGame = _trackedTime.ReadFromBuffer(seconds);
            timer.currentTime = _trackedCurrentTime.ReadFromBuffer(seconds);
        }

        private void TrackTargetTransform()
        {
            _trackedTime.WriteLastValue(timer.waitForEndGame);
            _trackedCurrentTime.WriteLastValue(timer.currentTime);
        }
    }
}