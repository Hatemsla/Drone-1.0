using System;
using UnityEngine;

namespace Drone.Builder
{
    public class Connection : MonoBehaviour
    {
        public ConnectionType connectionType;
        public ConnectionDirection connectionDirection;
        private TrackObject _trackObject;
        private Selection _selection;
        private Vector3 _connectPosition;

        private void Awake()
        {
            _trackObject = GetComponentInParent<TrackObject>();
        }

        private void Start()
        {
            _selection = FindObjectOfType<Selection>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(BuilderManager.Instance.objectActionType is not ActionType.FreeMove) return;
            if (!_trackObject.isActive || BuilderManager.Instance.pendingObjects.Count != 1) return;
            if (BuilderManager.Instance.pendingObject == null)
                return;

            var otherConnection = other.GetComponent<Connection>();

            if (otherConnection == null)
                return;

            SetObjectPosition(other, otherConnection);
        }

        private void SetObjectPosition(Collider other, Connection otherConnection)
        {
            float sizeMultiplier;
            Vector3 offset;
            switch (_trackObject.objectType)
            {
                case ObjectsType.Floor when otherConnection.connectionType == ConnectionType.Floor:
                    BuilderManager.Instance.PutObject();

                    sizeMultiplier = (_trackObject.Scale.x - 1f) * 2.5f;
                    offset = GetObjectOffset(otherConnection, sizeMultiplier);

                    _trackObject.transform.position = other.transform.position + offset;
                    _trackObject.transform.rotation = other.transform.rotation;
                    break;
                case ObjectsType.Wall when otherConnection.connectionType == ConnectionType.Wall:
                    BuilderManager.Instance.PutObject();
                    _connectPosition = new Vector3(other.transform.position.x,
                        other.transform.position.y + _trackObject.yOffset, other.transform.position.z);
                    _trackObject.transform.position = _connectPosition;
                    break;
                case ObjectsType.Slant when otherConnection.connectionType == ConnectionType.Slant:
                    BuilderManager.Instance.PutObject();
                    _trackObject.transform.position = other.transform.position;
                    break;
                case ObjectsType.Pipe when otherConnection.connectionType == ConnectionType.Pipe:
                    // BuilderManager.Instance.PutObject();
                    //
                    // sizeMultiplier = (_trackObject.Scale.x - 1f) * 2.5f;
                    // offset = GetObjectOffset(otherConnection, sizeMultiplier);
                    //
                    // _trackObject.transform.position = other.transform.position + offset;
                    // _trackObject.transform.rotation = other.transform.rotation;
                    break;
            }
        }

        public Vector3 GetObjectOffset(Connection otherConnection, float sizeMultiplier)
        {
            Vector3 offset = default;
            switch (otherConnection.connectionType)
            {
                case ConnectionType.Floor:
                    switch (otherConnection.connectionDirection)
                    {
                        case ConnectionDirection.Z:
                            offset = new Vector3(0f, 0f, 2.5f + sizeMultiplier);
                            break;
                        case ConnectionDirection.X:
                            offset = new Vector3(2.5f + sizeMultiplier, 0f, 0f);
                            break;
                        case ConnectionDirection.MZ:
                            offset = new Vector3(0f, 0f, -2.5f - sizeMultiplier);
                            break;
                        case ConnectionDirection.MX:
                            offset = new Vector3(-2.5f - sizeMultiplier, 0f, 0f);
                            break;
                    }
                    break;
            }

            offset = otherConnection.transform.TransformDirection(offset);
            
            return offset;
        }

        private void OnTriggerStay(Collider other)
        {
            if(BuilderManager.Instance.objectActionType is not ActionType.FreeMove) return;
            if (!_trackObject.isActive || BuilderManager.Instance.pendingObjects.Count != 1) return;
            
            var otherConnection = other.GetComponent<Connection>();
            
            if(otherConnection == null)
                return;
            
            switch (_trackObject.objectType)
            {
                case ObjectsType.Floor when otherConnection.connectionType == ConnectionType.Floor:
                {
                    if (Vector3.Distance(other.transform.position, BuilderManager.Instance.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }

                    break;
                }
                case ObjectsType.Wall when otherConnection.connectionType == ConnectionType.Wall:
                {
                    if (Vector3.Distance(other.transform.position, BuilderManager.Instance.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }

                    break;
                }
                case ObjectsType.Slant when otherConnection.connectionType == ConnectionType.Slant:
                {
                    if (Vector3.Distance(other.transform.position, BuilderManager.Instance.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }

                    break;
                }
            }
        }
    }
}