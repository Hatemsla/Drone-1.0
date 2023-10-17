using Drone.RuntimeHandle;
using Drone.RuntimeHandle.Handles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Drone
{
    public class SelectTransformGizmo : MonoBehaviour
    {
        public Material highlightMaterial;
        public Material selectionMaterial;

        private Material originalMaterialHighlight;
        private Material originalMaterialSelection;
        private Transform highlight;
        private Transform selection;
        private RaycastHit raycastHit;
        private RaycastHit raycastHitHandle;
        private GameObject runtimeTransformGameObj;
        private RuntimeTransformHandle runtimeTransformHandle;
        private readonly int runtimeTransformLayer = 6;
        private int runtimeTransformLayerMask;

        private void Start()
        {
            runtimeTransformGameObj = new GameObject();
            runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
            runtimeTransformGameObj.layer = runtimeTransformLayer;
            runtimeTransformLayerMask =
                1 << runtimeTransformLayer; //Layer number represented by a single bit in the 32-bit integer using bit shift
            runtimeTransformHandle.type = HandleType.POSITION;
            runtimeTransformHandle.autoScale = true;
            runtimeTransformHandle.autoScaleFactor = 1.0f;
            runtimeTransformGameObj.SetActive(false);
        }

        private void Update()
        {
            // Highlight
            if (highlight != null)
            {
                highlight.GetComponent<MeshRenderer>().sharedMaterial = originalMaterialHighlight;
                highlight = null;
            }

            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!EventSystem.current.IsPointerOverGameObject() &&
                Physics.Raycast(ray,
                    out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
            {
                highlight = raycastHit.transform;
                if (highlight.CompareTag("Selectable") && highlight != selection)
                {
                    if (highlight.GetComponent<MeshRenderer>().material != highlightMaterial)
                    {
                        originalMaterialHighlight = highlight.GetComponent<MeshRenderer>().material;
                        highlight.GetComponent<MeshRenderer>().material = highlightMaterial;
                    }
                }
                else
                {
                    highlight = null;
                }
            }

            // Selection
            if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
            {
                ApplyLayerToChildren(runtimeTransformGameObj);
                if (Physics.Raycast(ray, out raycastHit))
                {
                    if (Physics.Raycast(ray, out raycastHitHandle, Mathf.Infinity,
                            runtimeTransformLayerMask)) //Raycast towards runtime transform handle only
                    {
                    }
                    else if (highlight)
                    {
                        if (selection != null)
                            selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                        selection = raycastHit.transform;
                        if (selection.GetComponent<MeshRenderer>().material != selectionMaterial)
                        {
                            originalMaterialSelection = originalMaterialHighlight;
                            selection.GetComponent<MeshRenderer>().material = selectionMaterial;
                            runtimeTransformHandle.target = selection;
                            runtimeTransformGameObj.SetActive(true);
                        }

                        highlight = null;
                    }
                    else
                    {
                        if (selection)
                        {
                            selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                            selection = null;

                            runtimeTransformGameObj.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (selection)
                    {
                        selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                        selection = null;

                        runtimeTransformGameObj.SetActive(false);
                    }
                }
            }
            
            
            //Hot Keys for move, rotate, scale, local and Global/World transform
            if (runtimeTransformGameObj.activeSelf)
            {
                if (Keyboard.current.wKey.wasPressedThisFrame)
                {
                    runtimeTransformHandle.type = HandleType.POSITION;
                }
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    runtimeTransformHandle.type = HandleType.ROTATION;
                }
                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    runtimeTransformHandle.type = HandleType.SCALE;
                }
                if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)
                {
                    if (Keyboard.current.gKey.wasPressedThisFrame)
                    {
                        runtimeTransformHandle.space = HandleSpace.WORLD;
                    }
                    if (Keyboard.current.lKey.wasPressedThisFrame)
                    {
                        runtimeTransformHandle.space = HandleSpace.LOCAL;
                    }
                }
            }
        }

        private void ApplyLayerToChildren(GameObject parentGameObj)
        {
            foreach (Transform transform1 in parentGameObj.transform)
            {
                var layer = parentGameObj.layer;
                transform1.gameObject.layer = layer;
                foreach (Transform transform2 in transform1)
                {
                    transform2.gameObject.layer = layer;
                    foreach (Transform transform3 in transform2)
                    {
                        transform3.gameObject.layer = layer;
                        foreach (Transform transform4 in transform3)
                        {
                            transform4.gameObject.layer = layer;
                            foreach (Transform transform5 in transform4) transform5.gameObject.layer = layer;
                        }
                    }
                }
            }
        }
    }
}