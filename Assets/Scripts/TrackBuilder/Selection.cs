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
        public GameObject selectedObject;
        public List<GameObject> selectedObjects = new();
        public LayerMask layerMask;
        private BuilderManager _builderManager;

        private void Start()
        {
            _builderManager = FindObjectOfType<BuilderManager>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10000, layerMask))
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

            if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1)) && selectedObjects.Count > 0 && selectedObject != null)
            {
                _builderManager.PlaceObjects();
                Deselect();
            }

            if (Input.GetKeyDown(KeyCode.T) && selectedObjects.Count > 0 && selectedObject != null)
            {
                _builderManager.PlaceObjects();
                _builderManager.SelectObject(_builderManager.currentSelectObjectIndex);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Move()
        {
            if (selectedObject == null) return;
            TrackBuilderUtils.ChangeLayerRecursively(selectedObject.transform.root.transform, LayerMask.NameToLayer("Track"));
            _builderManager.pendingObject = selectedObject.gameObject;
            _builderManager.pendingObjects = new List<GameObject>(selectedObjects);
            _builderManager.currentObjectType = selectedObject.GetComponentInParent<TrackObject>();
            _builderManager.currentObjectType.isActive = true;
        }

        public void Delete()
        {
            if (selectedObject == null) return;
            if (selectedObject.CompareTag("Player")) return;
            
            foreach (var selectedObj in selectedObjects)
            {
                _builderManager.objectsPool.Remove(selectedObj);
                _builderManager.droneBuilderCheckNode.RemoveNode(selectedObj.transform);
                Destroy(selectedObj);
            }

            _builderManager.pendingObjects.Clear();
            selectedObjects.Clear();
            _builderManager.pendingObject = null;
        }

        public void AddSelection(GameObject obj)
        {
            if (selectedObjects.Contains(obj))
            {
                RemoveSelection(obj);
                return;
            }

            var outlines = obj.GetComponentsInChildren<Outline>();
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }
            selectedObjects.Add(obj);
            selectedObject = obj;
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
                        foreach (var outline in objOutlines)
                        {
                            outline.enabled = false;
                        }
                    }
                }

                selectedObjects = selectedObjects.Where(selectedObj => selectedObj == obj).ToList();
                
                return;
            }

            if(selectedObject != null)
                Deselect();

            var outlines = obj.GetComponentsInChildren<Outline>();
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }
            selectedObject = obj;
            selectedObjects.Add(selectedObject);
        }

        public void Deselect()
        {
            if (selectedObject == null)
                return;
            
            foreach (var selectedObj in selectedObjects)
            {
                var objOutlines = selectedObj.GetComponentsInChildren<Outline>();
                foreach (var outline in objOutlines)
                {
                    outline.enabled = false;
                }
            }
            
            selectedObject.GetComponent<TrackObject>().isActive = false;
            selectedObject = null;
            selectedObjects.Clear();
        }
        
        public void RemoveSelection(GameObject obj)
        {
            if (selectedObjects.Count == 0)
                return;

            var outlines = obj!.GetComponentsInChildren<Outline>();
            foreach (var outline in outlines)
            {
                outline.enabled = false;
            }

            obj.GetComponent<TrackObject>().isActive = false;
            selectedObjects.Remove(obj);
            selectedObject = selectedObjects.Count == 0 ? null : selectedObject;
        }
    }
}