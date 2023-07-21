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

        public bool SetPriority(int priority)
        {
            if(!isActive)
                return false;
            
            _camera.Priority = priority;

            return true;
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }
    }
}