using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using Drone.Builder.ControllerElements;
using Drone.TrackBuilder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Outline = cakeslice.Outline;

namespace Drone.Builder
{
    public class Selection : MonoBehaviour
    {
        public TrackObject selectedTrackObject;
        public GameObject selectedObject;
        public List<GameObject> selectedObjects = new();
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private EditObject editObject;
        private Connection _selectedConnection;

        private void Start()
        {
            InputManager.Instance.SelectObjectEvent += SelectObject;
            InputManager.Instance.SelectObjectsEvent += SelectObjects;
            InputManager.Instance.DeleteObjectEvent += Delete;
            BuilderManager.Instance.StartGame += Deselect;
        }

        private void OnDestroy()
        {
            InputManager.Instance.SelectObjectEvent -= SelectObject;
            InputManager.Instance.SelectObjectsEvent -= SelectObjects;
            InputManager.Instance.DeleteObjectEvent -= Delete;
            BuilderManager.Instance.StartGame -= Deselect;
        }
        
        private void Update()
        {
            if (selectedObject)
            {
                editObject.OnSelectObject(selectedTrackObject);
            }
        }
        
        private void SelectObjects()
        {
            if (EventSystem.current.IsPointerOverGameObject() || BuilderManager.Instance.isMove) return;
            
            var ray = Camera.main!.ScreenPointToRay(Mouse.current.position.ReadValue());
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
                                return;
                            }
                            case ConnectionType.Wall:
                            case ConnectionType.Slant:
                                selectedObject.transform.position = new Vector3(otherConnection.transform.position.x,
                                    otherConnection.transform.position.y + selectedTrackObject.yOffset,
                                    otherConnection.transform.position.z);
                                return;
                        }
                    }
                }
                    
                if (hit.collider.transform.root.gameObject.layer == LayerMask.NameToLayer(Idents.Layers.TrackGround))
                    AddSelection(hit.collider.transform.root.gameObject);
            }
        }

        private void SelectObject()
        {
            if (EventSystem.current.IsPointerOverGameObject() || BuilderManager.Instance.isMove) return;
            
            var ray = Camera.main!.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out var hit, 10000, layerMask))
            {
                if (selectedTrackObject && selectedObjects.Count == 1)
                {
                    var otherConnection = hit.collider.GetComponent<Connection>();
                    if (otherConnection && _selectedConnection)
                    {
                        float sizeMultiplier;
                        Vector3 offset;
                        Transform otherConnectionTransform;
                        switch (otherConnection.connectionType)
                        {
                            case ConnectionType.Floor:
                            {
                                sizeMultiplier = (selectedTrackObject.Scale.x - 1f) * 2.5f;
                                offset = _selectedConnection.GetObjectOffset(otherConnection, sizeMultiplier);
                                otherConnectionTransform = otherConnection.transform;
                                selectedObject.transform.position = otherConnectionTransform.position + offset;
                                selectedObject.transform.rotation = otherConnectionTransform.rotation;
                                return;
                            }
                            case ConnectionType.Wall:
                            case ConnectionType.Slant:
                                var otherConnectionPosition = otherConnection.transform.position; 
                                selectedObject.transform.position = new Vector3(otherConnectionPosition.x,
                                    otherConnectionPosition.y + selectedTrackObject.yOffset,
                                    otherConnectionPosition.z);
                                return;
                            // case ConnectionType.Pipe:
                            //     var currentPipe = selectedObject.GetComponentInParent<Pipe>();
                            //     var otherPipe = otherConnection.GetComponentInParent<Pipe>();
                            //     var currentPipeDictionary = selectedObject.GetComponent<PipeDictionary>();
                            //     var matchingOffset = currentPipeDictionary.dictionary
                            //         .Where(kv => kv.Key == currentPipe)
                            //         .Select(kv => kv.Value.connectionPosition
                            //                 .Where(cp => cp.thisConnectionDirection == cp.otherConnectionDirection)
                            //                 .Select(cp => cp.offset)
                            //         )
                            //         .FirstOrDefault()!.FirstOrDefault();
                            //     var matchingRotation = currentPipeDictionary.dictionary
                            //         .Where(kv => kv.Key == currentPipe)
                            //         .Select(kv => kv.Value.connectionPosition
                            //             .Where(cp => cp.thisConnectionDirection == cp.otherConnectionDirection)
                            //             .Select(cp => cp.offset)
                            //         )
                            //         .FirstOrDefault()!.FirstOrDefault();
                            //     otherConnectionTransform = otherConnection.transform;
                            //     selectedObject.transform.position = otherConnectionTransform.position + matchingOffset;
                            //     selectedObject.transform.rotation = Quaternion.Euler(
                            //         new Vector3(otherPipe.transform.eulerAngles.x + matchingRotation.x,
                            //             otherPipe.transform.eulerAngles.y + matchingRotation.y,
                            //             otherPipe.transform.eulerAngles.z + matchingRotation.z));
                            //     return;
                        }
                    }
                }
                    
                if (hit.collider.transform.root.gameObject.layer == LayerMask.NameToLayer(Idents.Layers.TrackGround))
                    Select(hit.collider.transform.root.gameObject);
            }
        }

        public void Move()
        {
            if (selectedObject == null) return;
            TrackBuilderUtils.ChangeLayerRecursively(selectedObject.transform.root.transform, LayerMask.NameToLayer(Idents.Layers.Track));
            TrackBuilderUtils.TurnTrackObjects(selectedObjects, true);
            BuilderManager.Instance.MoveObject(selectedObject, selectedObjects);
        }

        private void Delete()
        {
            if (selectedObject == null) return;
            if (selectedTrackObject.objectType == ObjectsType.Drone) return;
            
            foreach (var selectedObj in selectedObjects)
            {
                BuilderManager.Instance.objectsPool.Remove(selectedObj);
                BuilderManager.Instance.droneBuilderCheckNode.RemoveNode(selectedObj.transform);
                Destroy(selectedObj);
            }

            BuilderManager.Instance.DeleteObject();
            selectedObjects.Clear();
            editObject.HideEditMenu();
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
                    layerMask = TrackBuilderUtils.SetLayerMask(Idents.Layers.FloorConnection);
                    break;
                case ObjectsType.Wall:
                    layerMask = TrackBuilderUtils.SetLayerMask(Idents.Layers.WallConnection);
                    break;
                case ObjectsType.Slant:
                    layerMask = TrackBuilderUtils.SetLayerMask(Idents.Layers.SlantConnection);
                    break;
                case ObjectsType.Pipe:
                    layerMask = TrackBuilderUtils.SetLayerMask(Idents.Layers.PipeConnection);
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