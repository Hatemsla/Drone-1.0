//===========================================================================//
//                       FreeFlyCamera (Version 1.2)                         //
//                        (c) 2019 Sergey Stafeyev                           //
//===========================================================================//

using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{
    private float _currentIncrease = 1;
    private float _currentIncreaseMem;

    private Vector3 _initPosition;
    private Vector3 _initRotation;

    private CursorLockMode _wantedMode;


    private void Start()
    {
        _initPosition = transform.position;
        _initRotation = transform.eulerAngles;
    }

    private void Update()
    {
        if (!active)
            return;

        SetCursorState();

        if (Cursor.visible)
            return;

        // Translation
        if (enableTranslation)
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * translationSpeed);

        // Movement
        if (enableMovement)
        {
            var deltaPosition = Vector3.zero;
            var currentSpeed = movementSpeed;

            if (Input.GetKey(boostSpeed))
                currentSpeed = boostedSpeed;

            if (Input.GetKey(KeyCode.W))
                deltaPosition += transform.forward;

            if (Input.GetKey(KeyCode.S))
                deltaPosition -= transform.forward;

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                deltaPosition += transform.forward * 10;

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                deltaPosition -= transform.forward * 10;

            if (Input.GetKey(KeyCode.A))
                deltaPosition -= transform.right;

            if (Input.GetKey(KeyCode.D))
                deltaPosition += transform.right;

            if (Input.GetKey(moveUp))
                deltaPosition += transform.up;

            if (Input.GetKey(moveDown))
                deltaPosition -= transform.up;

            // Calc acceleration
            CalculateCurrentIncrease(deltaPosition != Vector3.zero);

            transform.position += deltaPosition * currentSpeed * _currentIncrease;
        }

        // Rotation
        if (enableRotation)
        {
            // Pitch
            transform.rotation *= Quaternion.AngleAxis(
                -Input.GetAxis("Mouse Y") * mouseSense,
                Vector3.right
            );

            // Paw
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + Input.GetAxis("Mouse X") * mouseSense,
                transform.eulerAngles.z
            );
        }

        // Return to init position
        if (Input.GetKeyDown(initPositonButton))
        {
            transform.position = _initPosition;
            transform.eulerAngles = _initRotation;
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
        if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = _wantedMode = CursorLockMode.None;

        if (Input.GetMouseButtonDown(1))
        {
            if (_wantedMode == CursorLockMode.Locked)
                _wantedMode = CursorLockMode.None;
            else
                _wantedMode = CursorLockMode.Locked;
        }

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

    [SerializeField] [Tooltip("Boost speed")]
    private KeyCode boostSpeed = KeyCode.LeftShift;

    [SerializeField] [Tooltip("Move up")] private KeyCode moveUp = KeyCode.E;

    [SerializeField] [Tooltip("Move down")]
    private KeyCode moveDown = KeyCode.Q;

    [Space] [SerializeField] [Tooltip("Acceleration at camera movement is active")]
    private bool enableSpeedAcceleration = true;

    [SerializeField] [Tooltip("Rate which is applied during camera movement")]
    private float speedAccelerationFactor = 1.5f;

    [Space] [SerializeField] [Tooltip("This keypress will move the camera to initialization position")]
    private KeyCode initPositonButton = KeyCode.R;

    #endregion UI
}