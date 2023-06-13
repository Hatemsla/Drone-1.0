using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using cakeslice;
using Unity.VisualScripting;
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
        [SerializeField] private BuilderManager builderManager;
        private Connection _selectedConnection;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 10000, layerMask))
                {
                    if (selectedTrackObject && selectedObjects.Count == 1)
                    {
                        if (hit.collider.GetComponent<Connection>())
                        {
                            var sizeMultiplier = (selectedTrackObject.Scale.x - 1f) * 2.5f;
                            var otherConnection = hit.collider.GetComponent<Connection>();
                            var offset = _selectedConnection.GetObjectOffset(otherConnection, sizeMultiplier);
                            selectedObject.transform.position = otherConnection.transform.position + offset;
                            selectedObject.transform.rotation = otherConnection.transform.rotation;
                        }
                    }

                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
                            AddSelection(hit.collider.transform.root.gameObject);
                    }
                    else
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
                            Select(hit.collider.transform.root.gameObject);
                    }
                }
            }

            if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1)) && selectedObjects.Count > 0 && selectedObject != null)
            {
                builderManager.PlaceObjects();
                Deselect();
            }

            if (Input.GetKeyDown(KeyCode.T) && selectedObjects.Count > 0 && selectedObject != null)
            {
                builderManager.PlaceObjects();
                builderManager.SelectObject(builderManager.currentSelectObjectIndex);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }

            if (selectedObject)
            {
                editObject.OnSelectObject(selectedTrackObject);
            }
        }

        public void Move()
        {
            if (selectedObject == null) return;
            TrackBuilderUtils.ChangeLayerRecursively(selectedObject.transform.root.transform, LayerMask.NameToLayer("Track"));
            TrackBuilderUtils.TurnTrackObjects(selectedObjects, true);
            builderManager.pendingObject = selectedObject.gameObject;
            builderManager.pendingObjects = new List<GameObject>(selectedObjects);
            builderManager.currentObjectType = selectedObject.GetComponentInParent<TrackObject>();
            builderManager.currentObjectType.isActive = true;
        }

        public void Delete()
        {
            if (selectedObject == null) return;
            if (selectedTrackObject.objectType == ObjectsType.Drone) return;
            
            foreach (var selectedObj in selectedObjects)
            {
                builderManager.objectsPool.Remove(selectedObj);
                builderManager.droneBuilderCheckNode.RemoveNode(selectedObj.transform);
                Destroy(selectedObj);
            }

            builderManager.pendingObjects.Clear();
            selectedObjects.Clear();
            builderManager.pendingObject = null;
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