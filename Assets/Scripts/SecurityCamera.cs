using Builder;
using Cinemachine;

namespace Drone
{
    public class SecurityCamera : InteractiveObject
    {
        private CinemachineVirtualCamera _camera;

        private void Start()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        public void SetPriority(int priority)
        {
            _camera.Priority = priority;
        }
    }
}