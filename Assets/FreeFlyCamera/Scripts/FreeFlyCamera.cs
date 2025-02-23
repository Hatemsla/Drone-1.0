﻿using System;
using Cinemachine;
using Drone;
using Drone.Builder;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeFlyCamera : MonoBehaviour
{
    private float _currentIncrease = 1;
    private float _currentIncreaseMem;
    private float _currentSpeed;
    private Vector3 _deltaPosition;

    private Vector3 _initPosition;
    private Vector3 _initRotation;

    private CursorLockMode _wantedMode;

    private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private void Start()
    {
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        BuilderManager.Instance.StartGame += DeactivateFreeFlyCamera;
        BuilderManager.Instance.StopGame += ActivateFreeFlyCamera;
    }

    private void OnEnable()
    {
        InputManager.Instance.SetCursorEvent += CursorState;
        InputManager.Instance.CameraBoostEvent += CameraBoost;
        InputManager.Instance.CameraMoveEvent += CameraMove;
        InputManager.Instance.CameraZoomEvent += CameraZoom;
        InputManager.Instance.CameraChangeHeightEvent += CameraChangeHeight;
        InputManager.Instance.LockCursorEvent += LockCursor;
    }

    private void OnDestroy()
    {
        InputManager.Instance.SetCursorEvent -= CursorState;
        InputManager.Instance.CameraBoostEvent -= CameraBoost;
        InputManager.Instance.CameraMoveEvent -= CameraMove;
        InputManager.Instance.CameraZoomEvent -= CameraZoom;
        InputManager.Instance.CameraChangeHeightEvent -= CameraChangeHeight;
        InputManager.Instance.LockCursorEvent -= LockCursor;
        BuilderManager.Instance.StartGame -= DeactivateFreeFlyCamera;
        BuilderManager.Instance.StopGame -= ActivateFreeFlyCamera;
    }

    private void DeactivateFreeFlyCamera()
    {
        _cinemachineVirtualCamera.Priority = 0;
    }

    private void ActivateFreeFlyCamera()
    {
        _cinemachineVirtualCamera.Priority = 10;
    }
    
    private void CameraMove(Vector2 value)
    {
        SetDefaultParameters();
        
        var moveDirection = new Vector3(value.x, 0f, value.y);

        _deltaPosition += moveDirection;
    }

    private void LockCursor()
    {
        _wantedMode = _wantedMode == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        InputManager.Instance.TurnCustomActionMap(_wantedMode == CursorLockMode.Locked
            ? Idents.ActionMaps.Camera
            : Idents.ActionMaps.Builder);
    }

    private void CameraChangeHeight(float value)
    {
        SetDefaultParameters();
        
        var heightDirection = new Vector3(0f, value, 0f);

        _deltaPosition += heightDirection;
    }

    private void CameraZoom(float value)
    {
        SetDefaultParameters();

        var zoomDirection = new Vector3(0f, 0f, value);

        _deltaPosition += zoomDirection * 10 * Time.deltaTime;
    }

    private void SetDefaultParameters()
    {
        _deltaPosition = Vector3.zero;
        _currentSpeed = movementSpeed;
    }

    private void CameraMove()
    {
        if (enableMovement)
        {
            CalculateCurrentIncrease(_deltaPosition != Vector3.zero);

            transform.position += transform.rotation * _deltaPosition * _currentSpeed * _currentIncrease;
        }
    }

    private void CameraBoost()
    {
        _currentSpeed = boostedSpeed;
    }

    private void CursorState()
    {
        Cursor.lockState = _wantedMode = CursorLockMode.None;
        InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Builder);
    }

    private void Update()
    {
        if (!active)
            return;

        SetCursorState();

        if (Cursor.visible)
            return;

        CameraMove();
            
        if (enableRotation)
        {
            transform.rotation *= Quaternion.AngleAxis(
                -Mouse.current.delta.y.ReadValue() * mouseSense * Time.smoothDeltaTime,
                Vector3.right
            );
            
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + Mouse.current.delta.x.ReadValue() * mouseSense  * Time.smoothDeltaTime,
                transform.eulerAngles.z
            );
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (boostedSpeed < movementSpeed)
            boostedSpeed = movementSpeed;
    }
#endif


    // Apply requested cursor state
    private void SetCursorState()
    {
        // Apply cursor state
        Cursor.lockState = _wantedMode;
        // Hide cursor when locking
        Cursor.visible = CursorLockMode.Locked != _wantedMode;
    }

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!enableSpeedAcceleration || (enableSpeedAcceleration && !moving))
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    #region UI

    [Space] [SerializeField] [Tooltip("The script is currently active")]
    private bool active = true;

    [Space] [SerializeField] [Tooltip("Camera rotation by mouse movement is active")]
    private bool enableRotation = true;

    [SerializeField] [Tooltip("Sensitivity of mouse rotation")]
    private float mouseSense = 1.8f;

    [Space] [SerializeField] [Tooltip("Camera zooming in/out by 'Mouse Scroll Wheel' is active")]
    private bool enableTranslation = true;

    [SerializeField] [Tooltip("Velocity of camera zooming in/out")]
    private float translationSpeed = 55f;

    [Space] [SerializeField] [Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
    private bool enableMovement = true;

    [SerializeField] [Tooltip("Camera movement speed")]
    private float movementSpeed = 10f;

    [SerializeField] [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
    private float boostedSpeed = 50f;

    [Space] [SerializeField] [Tooltip("Acceleration at camera movement is active")]
    private bool enableSpeedAcceleration = true;

    [SerializeField] [Tooltip("Rate which is applied during camera movement")]
    private float speedAccelerationFactor = 1.5f;

    #endregion UI
}