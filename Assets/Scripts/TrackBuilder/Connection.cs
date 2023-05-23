using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Builder
{
    public class Connection : MonoBehaviour
    {
        private TrackObject _trackObject;
        private BuilderManager _builderManager;
        private Selection _selection;
        private Vector3 _connectPosition;

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
            if (_trackObject.isActive && _builderManager.pendingObjects.Count == 1)
            {
                if(_builderManager.pendingObject == null)
                    return;
                
                if (_trackObject.objectType == ObjectsType.Floor &&
                    other.gameObject.layer == LayerMask.NameToLayer("FloorConnection"))
                {
                    _builderManager.PutObject();
                    _trackObject.transform.position = other.transform.position;
                    _trackObject.transform.rotation = other.transform.rotation;
                }
                else if (_trackObject.objectType == ObjectsType.Wall &&
                         other.gameObject.layer == LayerMask.NameToLayer("WallConnection"))
                {
                    _builderManager.PutObject();
                    _connectPosition = new Vector3(other.transform.position.x,
                        other.transform.position.y + _trackObject.yOffset, other.transform.position.z);
                    _trackObject.transform.position = _connectPosition;
                }
                else if (_trackObject.objectType == ObjectsType.Slant && 
                         other.gameObject.layer == LayerMask.NameToLayer("SlantConnection"))
                {
                    _builderManager.PutObject();
                    _trackObject.transform.position = other.transform.position;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_trackObject.isActive && _builderManager.pendingObjects.Count == 1)
            {
                if (_trackObject.objectType == ObjectsType.Floor &&
                    other.gameObject.layer == LayerMask.NameToLayer("FloorConnection"))
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }
                }
                else if (_trackObject.objectType == ObjectsType.Wall &&
                         other.gameObject.layer == LayerMask.NameToLayer("WallConnection"))
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }
                }
                else if (_trackObject.objectType == ObjectsType.Slant &&
                         other.gameObject.layer == LayerMask.NameToLayer("SlantConnection"))
                {
                    if (Vector3.Distance(other.transform.position, _builderManager.mousePos) > _trackObject.maxMouseDistance)
                    {
                        _selection.Move();
                    }
                }
            }
        }
    }
}