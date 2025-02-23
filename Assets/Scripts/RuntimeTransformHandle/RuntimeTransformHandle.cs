using Drone.Builder;
using Drone.RuntimeHandle.Handles;
using Drone.RuntimeHandle.Handles.Position;
using Drone.RuntimeHandle.Handles.Rotation;
using Drone.RuntimeHandle.Handles.Scale;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Drone.RuntimeHandle
{
    public class RuntimeTransformHandle : MonoBehaviour
    {
        public HandleAxes axes = HandleAxes.XYZ;
        public HandleSpace space = HandleSpace.LOCAL;
        public HandleType type = HandleType.POSITION;
        public HandleSnappingType snappingType = HandleSnappingType.RELATIVE;

        public Vector3 positionSnap = Vector3.zero;
        public float rotationSnap;
        public Vector3 scaleSnap = Vector3.zero;

        public bool autoScale;
        public float autoScaleFactor = 1;
        public Camera handleCamera;
        public EditObject editObject;

        private Vector3 _previousMousePosition;
        private HandleBase _previousAxis;

        private HandleBase _draggingHandle;

        private HandleType _previousType;
        private HandleAxes _previousAxes;

        private PositionHandle _positionHandle;
        private RotationHandle _rotationHandle;
        private ScaleHandle _scaleHandle;

        public Transform target;

        public UnityEvent startedDraggingHandle = new();
        public UnityEvent isDraggingHandle = new();
        public UnityEvent endedDraggingHandle = new();

        [SerializeField] private bool disableWhenNoTarget;

        private void Start()
        {
            if (handleCamera == null)
                handleCamera = Camera.main;

            _previousType = type;

            if (target == null)
                target = transform;

            if (disableWhenNoTarget && target == transform)
                gameObject.SetActive(false);

            CreateHandles();
        }

        private void CreateHandles()
        {
            switch (type)
            {
                case HandleType.POSITION:
                    _positionHandle = gameObject.AddComponent<PositionHandle>().Initialize(this, editObject);
                    break;
                case HandleType.ROTATION:
                    _rotationHandle = gameObject.AddComponent<RotationHandle>().Initialize(this, editObject);
                    break;
                case HandleType.SCALE:
                    _scaleHandle = gameObject.AddComponent<ScaleHandle>().Initialize(this);
                    break;
            }
        }

        private void Clear()
        {
            _draggingHandle = null;

            if (_positionHandle) _positionHandle.Destroy();
            if (_rotationHandle) _rotationHandle.Destroy();
            if (_scaleHandle) _scaleHandle.Destroy();
        }

        private void Update()
        {
            if (autoScale)
                transform.localScale =
                    Vector3.one * (Vector3.Distance(handleCamera.transform.position, transform.position) *
                                   autoScaleFactor) / 15;

            if (_previousType != type || _previousAxes != axes)
            {
                Clear();
                CreateHandles();
                _previousType = type;
                _previousAxes = axes;
            }

            HandleBase handle = null;
            var hitPoint = Vector3.zero;
            GetHandle(ref handle, ref hitPoint);

            HandleOverEffect(handle, hitPoint);

            if (PointerIsDown() && _draggingHandle != null)
            {
                _draggingHandle.Interact(_previousMousePosition);
                isDraggingHandle.Invoke();
            }

            if (GetPointerDown() && handle != null)
            {
                _draggingHandle = handle;
                _draggingHandle.StartInteraction(hitPoint);
                startedDraggingHandle.Invoke();
            }

            if (GetPointerUp() && _draggingHandle != null)
            {
                _draggingHandle.EndInteraction();
                _draggingHandle = null;
                endedDraggingHandle.Invoke();
            }

            _previousMousePosition = GetMousePosition();

            transform.position = target.transform.position;
            if (space == HandleSpace.LOCAL || type == HandleType.SCALE)
                transform.rotation = target.transform.rotation;
            else
                transform.rotation = Quaternion.identity;
        }

        public static bool GetPointerDown()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.leftButton.wasPressedThisFrame;
#else
            return Mouse.current.leftButton.wasPressedThisFrame;
#endif
        }

        public static bool PointerIsDown()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.leftButton.isPressed;
#else
            return Mouse.current.leftButton.isPressed;
#endif
        }

        public static bool GetPointerUp()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.leftButton.wasReleasedThisFrame;
#else
            return Mouse.current.leftButton.wasReleasedThisFrame;
#endif
        }

        public static Vector3 GetMousePosition()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Mouse.current.position.ReadValue();
#endif
        }

        private void HandleOverEffect(HandleBase p_axis, Vector3 p_hitPoint)
        {
            if (_draggingHandle == null && _previousAxis != null &&
                (_previousAxis != p_axis || !_previousAxis.CanInteract(p_hitPoint))) _previousAxis.SetDefaultColor();

            if (p_axis != null && _draggingHandle == null && p_axis.CanInteract(p_hitPoint))
                p_axis.SetColor(Color.yellow);

            _previousAxis = p_axis;
        }

        private void GetHandle(ref HandleBase p_handle, ref Vector3 p_hitPoint)
        {
            var ray = Camera.main.ScreenPointToRay(GetMousePosition());
            var hits = Physics.RaycastAll(ray);
            if (hits.Length == 0)
                return;

            foreach (var hit in hits)
            {
                p_handle = hit.collider.gameObject.GetComponentInParent<HandleBase>();

                if (p_handle != null)
                {
                    p_hitPoint = hit.point;
                    return;
                }
            }
        }

        public static RuntimeTransformHandle Create(Transform p_target, HandleType p_handleType)
        {
            var runtimeTransformHandle = new GameObject().AddComponent<RuntimeTransformHandle>();
            runtimeTransformHandle.target = p_target;
            runtimeTransformHandle.type = p_handleType;

            return runtimeTransformHandle;
        }

        #region public methods to control handles

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetTarget(GameObject newTarget)
        {
            target = newTarget.transform;

            if (target == null)
                target = transform;

            if (disableWhenNoTarget && target == transform)
                gameObject.SetActive(false);
            else if (disableWhenNoTarget && target != transform)
                gameObject.SetActive(true);
        }

        public void SetHandleMode(int mode)
        {
            SetHandleMode((HandleType)mode);
        }

        public void SetHandleMode(HandleType mode)
        {
            type = mode;
        }

        public void EnableXAxis(bool enable)
        {
            if (enable)
                axes |= HandleAxes.X;
            else
                axes &= ~HandleAxes.X;
        }

        public void EnableYAxis(bool enable)
        {
            if (enable)
                axes |= HandleAxes.Y;
            else
                axes &= ~HandleAxes.Y;
        }

        public void EnableZAxis(bool enable)
        {
            if (enable)
                axes |= HandleAxes.Z;
            else
                axes &= ~HandleAxes.Z;
        }

        public void SetAxis(HandleAxes newAxes)
        {
            axes = newAxes;
        }

        #endregion
    }
}