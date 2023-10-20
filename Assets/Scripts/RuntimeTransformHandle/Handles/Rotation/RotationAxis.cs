using Drone.Builder;
using Drone.RuntimeHandle.Utils;
using UnityEngine;

namespace Drone.RuntimeHandle.Handles.Rotation
{
    public class RotationAxis : HandleBase
    {
        private EditObject _editObject;
        private Mesh _arcMesh;
        private Material _arcMaterial;
        private Vector3 _axis;
        private Vector3 _rotatedAxis;
        private Plane _axisPlane;
        private Vector3 _tangent;
        private Vector3 _biTangent;

        private Quaternion _startRotation;

        public RotationAxis Initialize(RuntimeTransformHandle p_runtimeHandle, Vector3 p_axis, Color p_color, EditObject editObject)
        {
            _parentTransformHandle = p_runtimeHandle;
            _axis = p_axis;
            _defaultColor = p_color;
            _editObject = editObject;
            
            InitializeMaterial();

            transform.SetParent(p_runtimeHandle.transform, false);

            var o = new GameObject();
            o.transform.SetParent(transform, false);
            var mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            var mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateTorus(2f, .02f, 32, 6);
            var mc = o.AddComponent<MeshCollider>();
            mc.sharedMesh = MeshUtils.CreateTorus(2f, .1f, 32, 6);
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, _axis);
            TrackBuilderUtils.ChangeLayerRecursively(o.transform, LayerMask.NameToLayer(Idents.Layers.TrackGround));

            return this;
        }

        protected override void InitializeMaterial()
        {
            _material = new Material(Resources.Load("Shaders/AdvancedHandleShader") as Shader);
            _material.color = _defaultColor;
        }

        public void Update()
        {
            _material.SetVector("_CameraPosition", _parentTransformHandle.handleCamera.transform.position);
            _material.SetFloat("_CameraDistance",
                (_parentTransformHandle.handleCamera.transform.position - _parentTransformHandle.transform.position)
                .magnitude);
        }

        public override void Interact(Vector3 p_previousPosition)
        {
            var cameraRay = Camera.main.ScreenPointToRay(RuntimeTransformHandle.GetMousePosition());

            if (!_axisPlane.Raycast(cameraRay, out var hitT))
            {
                base.Interact(p_previousPosition);
                return;
            }

            var hitPoint = cameraRay.GetPoint(hitT);
            var hitDirection = (hitPoint - _parentTransformHandle.target.position).normalized;
            var x = Vector3.Dot(hitDirection, _tangent);
            var y = Vector3.Dot(hitDirection, _biTangent);
            var angleRadians = Mathf.Atan2(y, x);
            var angleDegrees = angleRadians * Mathf.Rad2Deg;

            if (_parentTransformHandle.rotationSnap != 0)
            {
                angleDegrees = Mathf.Round(angleDegrees / _parentTransformHandle.rotationSnap) *
                               _parentTransformHandle.rotationSnap;
                angleRadians = angleDegrees * Mathf.Deg2Rad;
            }

            if (_parentTransformHandle.space == HandleSpace.LOCAL)
            {
                _parentTransformHandle.target.localRotation =
                    _startRotation * Quaternion.AngleAxis(angleDegrees, _axis);
            }
            else
            {
                var invertedRotatedAxis = Quaternion.Inverse(_startRotation) * _axis;
                _parentTransformHandle.target.rotation =
                    _startRotation * Quaternion.AngleAxis(angleDegrees, invertedRotatedAxis);
            }

            _arcMesh = MeshUtils.CreateArc(transform.position, _hitPoint, _rotatedAxis, 2, angleRadians,
                Mathf.Abs(Mathf.CeilToInt(angleDegrees)) + 1);
            DrawArc();
            
            _editObject.editMenu.UpdateRotationsView(_editObject.currentObject);

            base.Interact(p_previousPosition);
        }

        public override bool CanInteract(Vector3 p_hitPoint)
        {
            var cameraDistance = (_parentTransformHandle.transform.position -
                                  _parentTransformHandle.handleCamera.transform.position).magnitude;
            var pointDistance = (p_hitPoint - _parentTransformHandle.handleCamera.transform.position).magnitude;
            return pointDistance <= cameraDistance;
        }

        public override void StartInteraction(Vector3 p_hitPoint)
        {
            if (!CanInteract(p_hitPoint))
                return;


            base.StartInteraction(p_hitPoint);

            _startRotation = _parentTransformHandle.space == HandleSpace.LOCAL
                ? _parentTransformHandle.target.localRotation
                : _parentTransformHandle.target.rotation;

            _arcMaterial = new Material(Shader.Find("RuntimeTransformGizmos/HandleShader"));
            _arcMaterial.color = new Color(1, 1, 0, .4f);
            _arcMaterial.renderQueue = 5000;
            //_arcMesh.gameObject.SetActive(true);

            if (_parentTransformHandle.space == HandleSpace.LOCAL)
                _rotatedAxis = _startRotation * _axis;
            else
                _rotatedAxis = _axis;

            _axisPlane = new Plane(_rotatedAxis, _parentTransformHandle.target.position);

            Vector3 startHitPoint;
            var cameraRay = Camera.main.ScreenPointToRay(RuntimeTransformHandle.GetMousePosition());
            if (_axisPlane.Raycast(cameraRay, out var hitT))
                startHitPoint = cameraRay.GetPoint(hitT);
            else
                startHitPoint = _axisPlane.ClosestPointOnPlane(p_hitPoint);

            _tangent = (startHitPoint - _parentTransformHandle.target.position).normalized;
            _biTangent = Vector3.Cross(_rotatedAxis, _tangent);
        }

        public override void EndInteraction()
        {
            base.EndInteraction();
            //Destroy(_arcMesh.gameObject);
            delta = 0;
        }

        private void DrawArc()
        {
            // _arcMaterial.SetPass(0);
            // Graphics.DrawMeshNow(_arcMesh, Matrix4x4.identity);
            Graphics.DrawMesh(_arcMesh, Matrix4x4.identity, _arcMaterial, 0);

            // GameObject arc = new GameObject();
            // MeshRenderer mr = arc.AddComponent<MeshRenderer>();
            // mr.material = new Material(Shader.Find("sHTiF/HandleShader"));
            // mr.material.color = new Color(1,1,0,.5f);
            // _arcMesh = arc.AddComponent<MeshFilter>();
        }
    }
}