using System;
using Cinemachine;
using CodeMonkey.Utils;
using UnityEngine;

namespace Builder
{
    public class DrawLine : InteractiveObject
    {
        public Transform meshParent;
        public CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float lineThickness = 1f;
        [SerializeField] private Color lineColor = Color.green;
        [SerializeField] private Material drawMeshMaterial;

        private CinemachineBrain _mainCamera;
        private GameObject _lastGameObject;
        private int _lastSortingOrder;
        private Mesh _mesh;
        private Vector3 _lastMouseWorldPosition;
        private Vector3 _mouseWorldPosition;
        private bool _isDraw;

        private void Start()
        {
            _mainCamera = FindObjectOfType<CinemachineBrain>();
        }

        private void Update()
        {
            if (!UtilsClass.IsPointerOverUI() && _isDraw)
            {
                _mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                if (Input.GetMouseButtonDown(0))
                {
                    CreateMeshObject();
                    _mesh = MeshUtils.CreateMesh(_mouseWorldPosition, _mouseWorldPosition, _mouseWorldPosition,
                        _mouseWorldPosition);
                    _mesh.MarkDynamic();
                    _lastGameObject.GetComponent<MeshFilter>().mesh = _mesh;
                    var material = new Material(drawMeshMaterial);
                    material.color = lineColor;
                    _lastGameObject.GetComponent<MeshRenderer>().material = material;
                    _lastGameObject.transform.localPosition = new Vector3(_lastGameObject.transform.localPosition.x,
                        _lastGameObject.transform.localPosition.y, 0f);
                }

                if (Input.GetMouseButton(0))
                {
                    const float minDistance = .1f;
                    if (Vector2.Distance(_lastMouseWorldPosition, _mouseWorldPosition) > minDistance)
                    {
                        Vector2 forwardVector = (_mouseWorldPosition - _lastMouseWorldPosition).normalized;

                        _lastMouseWorldPosition = _mouseWorldPosition;

                        MeshUtils.AddLinePoint(_mesh, _mouseWorldPosition, lineThickness);
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    MeshUtils.AddLinePoint(_mesh, _mouseWorldPosition, 0f);
                }
            }
        }

        private void CreateMeshObject()
        {
            _lastGameObject = new GameObject("DrawMeshSingle", typeof(MeshFilter), typeof(MeshRenderer));
            _lastGameObject.transform.SetParent(meshParent);
            _lastSortingOrder++;
            _lastGameObject.GetComponent<MeshRenderer>().sortingOrder = _lastSortingOrder;
        }

        public void StartDraw()
        {
            _isDraw = true;
            virtualCamera.Priority = 11;
            _mainCamera.OutputCamera.orthographic = true;
        }

        public void StopDraw()
        {
            _isDraw = false;
            virtualCamera.Priority = 0;
            _mainCamera.OutputCamera.orthographic = false;
        }

        public void SetThickness(float value)
        {
            lineThickness = value;
        }

        public void SetColor(Color color)
        {
            lineColor = color;
        }

        public override void SetActive(bool active)
        {
            
        }
    }
}