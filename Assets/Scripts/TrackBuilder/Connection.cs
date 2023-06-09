using UnityEngine;

namespace Builder
{
    public class Connection : MonoBehaviour
    {
        public ConnectionType connectionType;
        public ConnectionDirection connectionDirection;
        private TrackObject _trackObject;
        private BuilderManager _builderManager;
        private Selection _selection;
        private Vector3 _connectPosition;
        public bool isOccupied;

        private void Awake()
        {
            _trackObject = GetComponentInParent<TrackObject>();
        }

        private void Start()
        {
            _builderManager = FindObjectOfType<BuilderManager>();
            _selection = FindObjectOfType<Selection>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_trackObject.isActive || _builderManager.pendingObjects.Count != 1 || isOccupied) return;
            if(_builderManager.pendingObject == null)
                return;

            var otherConnection = other.GetComponent<Connection>();
            
            if(otherConnection == null || otherConnection.isOccupied)
                return;
            
            switch (_trackObject.objectType)
            {
                case ObjectsType.Floor when otherConnection.connectionType == ConnectionType.Floor:
                    _builderManager.PutObject();

                    var sizeMultiplier = (_trackObject.Scale.x - 1f) * 2.5f;
                    Vector3 offset = default;
                    switch (otherConnection.connectionDirection)
                    {
                        case ConnectionDirection.Up:
                            offset = new Vector3(0f, 0f, 2.5f + sizeMultiplier);
                            break;
                        case ConnectionDirection.Right:
                            offset = new Vector3(2.5f + sizeMultiplier, 0f, 0f);
                            break;
                        case ConnectionDirection.Down:
                            offset = new Vector3(0f, 0f, -2.5f - sizeMultiplier);
                            break;
                        case ConnectionDirection.Left:
                            offset = new Vector3(-2.5f - sizeMultiplier, 0f, 0f);
                            break;
                    }
                    
                    _trackObject.transform.position = other.transform.position + offset;
                    _trackObject.transform.rotation = other.transform.rotation;
                    isOccupied = otherConnection.isOccupied = true;
                    break;
                case ObjectsType.Wall when otherConnection.connectionType == ConnectionType.Wall:
                    _builderManager.PutObject();
                    _connectPosition = new Vector3(other.transform.position.x,
                        other.transform.position.y + _trackObject.yOffset, other.transform.position.z);
                    _trackObject.transform.position = _connectPosition;
                    isOccupied = otherConnection.isOccupied = true;
                    break;
                case ObjectsType.Slant when otherConnection.connectionType == ConnectionType.Slant:
                    _builderManager.PutObject();
                    _trackObject.transform.position = other.transform.position;
                    isOccupied = otherConnection.isOccupied = true; break;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_trackObject.isActive || _builderManager.pendingObjects.Count != 1) return;
            
            var otherConnection = other.GetComponent<Connection>();
            
            if(otherConnection == null)
                return;
            
            switch (_trackObject.objectType)
            {
                case ObjectsType.Floor when otherConnection.connectionType == ConnectionType.Floor:
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }

                    break;
                }
                case ObjectsType.Wall when otherConnection.connectionType == ConnectionType.Wall:
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }

                    break;
                }
                case ObjectsType.Slant when otherConnection.connectionType == ConnectionType.Slant:
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }

                    break;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var otherConnection = other.GetComponent<Connection>();
            
            if(otherConnection == null || otherConnection.isOccupied)
                return;

            
            switch (_trackObject.objectType)
            {
                case ObjectsType.Slant when _selection.selectedObject.GetComponent<TrackObject>().objectType == ObjectsType.Slant:
                    _selection.Move();
                    break;
                case ObjectsType.Wall when _selection.selectedObject.GetComponent<TrackObject>().objectType == ObjectsType.Wall:
                    _selection.Move();
                    break;
            }

            isOccupied = otherConnection.isOccupied = false;
        }
    }
}