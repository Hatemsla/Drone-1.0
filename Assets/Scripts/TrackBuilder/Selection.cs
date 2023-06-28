using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using UnityEngine;
using UnityEngine.EventSystems;
using Outline = cakeslice.Outline;

namespace Builder
{
    public class Selection : MonoBehaviour
    {
        public TrackObject selectedTrackObject;
        public GameObject selectedObject;
        public List<GameObject> selectedObjects = new();
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private EditObject editObject;
        private Connection _selectedConnection;

        private void Update()
        {
            if(BuilderManager.Instance.isInputText)
                return;
            
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 10000, layerMask))
                {
                    if (selectedTrackObject && selectedObjects.Count == 1)
                    {
                        if (hit.collider.GetComponent<Connection>() && _selectedConnection)
                        {
                            var otherConnection = hit.collider.GetComponent<Connection>();
                            
                            switch (otherConnection.connectionType)
                            {
                                case ConnectionType.Floor:
                                {
                                    var sizeMultiplier = (selectedTrackObject.Scale.x - 1f) * 2.5f;
                                    var offset = _selectedConnection.GetObjectOffset(otherConnection, sizeMultiplier);
                                    selectedObject.transform.position = otherConnection.transform.position + offset;
                                    selectedObject.transform.rotation = otherConnection.transform.rotation;
                                    break;
                                }
                                case ConnectionType.Wall:
                                case ConnectionType.Slant:
                                    selectedObject.transform.position = new Vector3(otherConnection.transform.position.x,
                                        otherConnection.transform.position.y + selectedTrackObject.yOffset,
                                        otherConnection.transform.position.z);
                                    break;
                            }
                        }
                    }

                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (hit.collider.transform.root.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
                            AddSelection(hit.collider.transform.root.gameObject);
                    }
                    else
                    {
                        if (hit.collider.transform.root.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
                            Select(hit.collider.transform.root.gameObject);
                    }
                }
            }

            if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1)) && selectedObjects.Count > 0 && selectedObject != null)
            {
                BuilderManager.Instance.PlaceObjects();
                Deselect();
            }

            if (Input.GetKeyDown(KeyCode.T) && selectedObjects.Count > 0 && selectedObject != null)
            {
                BuilderManager.Instance.PlaceObjects();
                BuilderManager.Instance.SelectObject(BuilderManager.Instance.currentSelectObjectIndex);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }

            if (selectedObject)
            {
                switch (selectedTrackObject.interactiveType)
                {
                    case InteractiveType.Windmill:
                        selectedTrackObject.windmill = selectedTrackObject.GetComponentInChildren<Windmill>();
                        break;
                    case InteractiveType.Magnet:
                        selectedTrackObject.magnet = selectedTrackObject.GetComponentInChildren<RigidbodyMagnet>();
                        break;
                    case InteractiveType.Pendulum:
                        selectedTrackObject.pendulum = selectedTrackObject.GetComponentInChildren<Pendulum>();
                        break;
                    case InteractiveType.Battery:
                        selectedTrackObject.battery = selectedTrackObject.GetComponentInChildren<Battery>();
                        break;
                    case InteractiveType.Freezing:
                        selectedTrackObject.freezingBall = selectedTrackObject.GetComponentInChildren<FreezingBall>();
                        break;
                    case InteractiveType.Wind:
                        selectedTrackObject.windZone = selectedTrackObject.GetComponentInChildren<WindZoneScript>();
                        break;
                    case InteractiveType.Boost:
                        selectedTrackObject.boost = selectedTrackObject.GetComponentInChildren<BoostTrigger>();
                        break;
                    case InteractiveType.Lamp:
                        selectedTrackObject.lamp = selectedTrackObject.GetComponentInChildren<Lamp>();
                        break;
                    case InteractiveType.Hint:
                        selectedTrackObject.hint = selectedTrackObject.GetComponentInChildren<Hint>();
                        break;
                }

                editObject.OnSelectObject(selectedTrackObject);
            }
        }

        public void Move()
        {
            if (selectedObject == null) return;
            TrackBuilderUtils.ChangeLayerRecursively(selectedObject.transform.root.transform, LayerMask.NameToLayer("Track"));
            TrackBuilderUtils.TurnTrackObjects(selectedObjects, true);
            BuilderManager.Instance.pendingObject = selectedObject.gameObject;
            BuilderManager.Instance.pendingObjects = new List<GameObject>(selectedObjects);
            BuilderManager.Instance.currentObjectType = selectedObject.GetComponentInParent<TrackObject>();
            BuilderManager.Instance.currentObjectType.isActive = true;
        }

        public void Delete()
        {
            if (selectedObject == null) return;
            if (selectedTrackObject.objectType == ObjectsType.Drone) return;
            
            foreach (var selectedObj in selectedObjects)
            {
                BuilderManager.Instance.objectsPool.Remove(selectedObj);
                BuilderManager.Instance.droneBuilderCheckNode.RemoveNode(selectedObj.transform);
                Destroy(selectedObj);
            }

            BuilderManager.Instance.pendingObjects.Clear();
            selectedObjects.Clear();
            BuilderManager.Instance.pendingObject = null;
        }

        public void AddSelection(GameObject obj)
        {
            if (selectedObjects.Contains(obj))
            {
                RemoveSelection(obj);
                return;
            }

            var outlines = obj.GetComponentsInChildren<Outline>();
            TrackBuilderUtils.TurnAllOutlineEffects(outlines, true);
            selectedObjects.Add(obj);
            selectedObject = obj;
            selectedTrackObject = obj.GetComponent<TrackObject>();
        }

        public void Select(GameObject obj)
        {
            if (obj == selectedObject)
            {
                foreach (var selectedObj in selectedObjects)
                {
                    if (selectedObj != obj)
                    {
                        var objOutlines = selectedObj.GetComponentsInChildren<Outline>();
                        TrackBuilderUtils.TurnAllOutlineEffects(objOutlines, false);
                    }
                }
                
                selectedObjects = selectedObjects.Where(selectedObj => selectedObj == obj).ToList();
                
                return;
            }

            if(selectedObject != null)
                Deselect();

            var outlines = obj.GetComponentsInChildren<Outline>();
            TrackBuilderUtils.TurnAllOutlineEffects(outlines, true);
            selectedObject = obj;
            selectedTrackObject = obj.GetComponent<TrackObject>();
            switch (selectedTrackObject.objectType)
            {
                case ObjectsType.Floor:
                    layerMask = TrackBuilderUtils.SetLayerMask("FloorConnection");
                    break;
                case ObjectsType.Wall:
                    layerMask = TrackBuilderUtils.SetLayerMask("WallConnection");
                    break;
                case ObjectsType.Slant:
                    layerMask = TrackBuilderUtils.SetLayerMask("SlantConnection");
                    break;
            }
            _selectedConnection = selectedObject.GetComponentInChildren<Connection>();
            selectedObjects.Add(selectedObject);
            editObject.ShowEditMenu();
        }

        public void Deselect()
        {
            if (selectedObject == null)
                return;
            
            foreach (var selectedObj in selectedObjects)
            {
                var outlines = selectedObj.GetComponentsInChildren<Outline>();
                TrackBuilderUtils.TurnAllOutlineEffects(outlines, false);
            }
            
            selectedObject.GetComponent<TrackObject>().isActive = false;
            selectedObject = null;
            selectedTrackObject = null;
            selectedObjects.Clear();
            editObject.HideEditMenu();
        }
        
        public void RemoveSelection(GameObject obj)
        {
            if (selectedObjects.Count == 0)
                return;

            var outlines = obj!.GetComponentsInChildren<Outline>();
            TrackBuilderUtils.TurnAllOutlineEffects(outlines, false);

            obj.GetComponent<TrackObject>().isActive = false;
            selectedObjects.Remove(obj);
            selectedObject = selectedObjects.Count == 0 ? null : selectedObject;
        }
    }
}