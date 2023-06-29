using System;
using CodeMonkey.Utils;
using UnityEngine;

namespace Builder
{
    public class DrawMesh : MonoBehaviour
    {
        public static DrawMesh Instance;
        
        [SerializeField] private Material drawMeshMaterial;

        private GameObject _lastGameObject;
        private int _lastSortingOrder;
        private Mesh _mesh;
        private Vector3 _lastMouseWorldPosition;
        public float lineThickness = 0.1f;
        public Color lineColor = Color.green;
        public bool eraseMode;
        public Transform meshParent;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!UtilsClass.IsPointerOverUI())
            {
                // Only run logic if not over UI
                var mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                
                if(Input.GetMouseButtonDown(1))
                    ToggleEraseMode();

                if (!eraseMode)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Mouse Down
                        CreateMeshObject();
                        _mesh = MeshUtils.CreateMesh(mouseWorldPosition, mouseWorldPosition, mouseWorldPosition,
                            mouseWorldPosition);
                        _mesh.MarkDynamic();
                        _lastGameObject.GetComponent<MeshFilter>().mesh = _mesh;
                        var material = new Material(drawMeshMaterial);
                        material.color = lineColor;
                        _lastGameObject.GetComponent<MeshRenderer>().material = material;
                    }

                    if (Input.GetMouseButton(0))
                    {
                        // Mouse Held Down
                        var minDistance = .1f;
                        if (Vector2.Distance(_lastMouseWorldPosition, mouseWorldPosition) > minDistance)
                        {
                            // Far enough from last point
                            Vector2 forwardVector = (mouseWorldPosition - _lastMouseWorldPosition).normalized;

                            _lastMouseWorldPosition = mouseWorldPosition;

                            MeshUtils.AddLinePoint(_mesh, mouseWorldPosition, lineThickness);
                        }
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        MeshUtils.AddLinePoint(_mesh, mouseWorldPosition, 0f);
                    }
                }
            }
        }

        private void CreateMeshObject()
        {
            _lastGameObject = new GameObject("DrawMeshSingle", typeof(MeshFilter), typeof(MeshRenderer));
            _lastGameObject.AddComponent<DrawMeshHandler>();
            _lastGameObject.transform.SetParent(meshParent, false);
            _lastSortingOrder++;
            _lastGameObject.GetComponent<MeshRenderer>().sortingOrder = _lastSortingOrder;
        }
        
        public void ToggleEraseMode()
        {
            eraseMode = !eraseMode;
        }
    }
}