using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera thirdView;
    [SerializeField] private CinemachineVirtualCamera firstView;

    private int _tempView = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            thirdView.Priority = firstView.Priority;
            firstView.Priority = _tempView;
            _tempView = thirdView.Priority;
        }
    }
}
