using Cinemachine;
using Drone;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera thirdView;
    [SerializeField] private CinemachineVirtualCamera firstView;

    private int _tempView = 10;

    private void OnEnable()
    {
        InputManager.Instance.SwitchViewEvent += SwitchView;
    }
        
    private void OnDisable()
    {
        InputManager.Instance.SwitchViewEvent -= SwitchView;
    }

    private void SwitchView()
    {
        thirdView.Priority = firstView.Priority;
        firstView.Priority = _tempView;
        _tempView = thirdView.Priority;
    }
}
