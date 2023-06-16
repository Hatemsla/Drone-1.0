using Cinemachine;
using UnityEngine;

namespace Builder
{
    public class BuilderCameraController : MonoBehaviour
    {
        public bool isSwitch;
        public bool isFirstView;
        [SerializeField] private CinemachineVirtualCamera thirdView;
        [SerializeField] private CinemachineVirtualCamera firstView;

        private int _prevThirdView = 10;
        private int _prevFirstView = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V) && isSwitch)
            {
                (_prevThirdView, _prevFirstView) = (_prevFirstView, _prevThirdView);
                thirdView.Priority = _prevThirdView;
                firstView.Priority = _prevFirstView;
                isFirstView = firstView.Priority > thirdView.Priority;
            }
        }

        public void SetUpCamerasDefaultPriority()
        {
            thirdView.Priority = 10;
            firstView.Priority = 0;
        }

        public void SetUpCamerasZeroPriority()
        {
            thirdView.Priority = 0;
            firstView.Priority = 0;
        }
    }
}