using Drone.Builder;
using Drone.RuntimeHandle.Utils;
using UnityEngine;

namespace Drone.RuntimeHandle.Handles.Scale
{
    public class ScaleAxis : HandleBase
    {
        private const float SIZE = 2;

        private Vector3 _axis;
        private Vector3 _startScale;

        private float _interactionDistance;
        private Ray _raxisRay;

        public ScaleAxis Initialize(RuntimeTransformHandle p_parentTransformHandle, Vector3 p_axis, Color p_color)
        {
            _parentTransformHandle = p_parentTransformHandle;
            _axis = p_axis;
            _defaultColor = p_color;

            InitializeMaterial();

            transform.SetParent(p_parentTransformHandle.transform, false);

            var o = new GameObject();
            o.transform.SetParent(transform, false);
            var mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            var mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateCone(p_axis.magnitude * SIZE, .02f, .02f, 8, 1);
            var mc = o.AddComponent<MeshCollider>();
            mc.sharedMesh = MeshUtils.CreateCone(p_axis.magnitude * SIZE, .1f, .02f, 8, 1);
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p_axis);
            TrackBuilderUtils.ChangeLayerRecursively(o.transform, LayerMask.NameToLayer(Idents.Layers.TrackGround));

            o = new GameObject();
            o.transform.SetParent(transform, false);
            mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateBox(.25f, .25f, .25f);
            mc = o.AddComponent<MeshCollider>();
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p_axis);
            o.transform.localPosition = p_axis * SIZE;
            TrackBuilderUtils.ChangeLayerRecursively(o.transform, LayerMask.NameToLayer(Idents.Layers.TrackGround));

            return this;
        }

        protected void Update()
        {
            transform.GetChild(0).localScale = new Vector3(1, 1 + delta, 1);
            transform.GetChild(1).localPosition = _axis * (SIZE * (1 + delta));
        }

        public override void Interact(Vector3 p_previousPosition)
        {
            var cameraRay = Camera.main.ScreenPointToRay(RuntimeTransformHandle.GetMousePosition());

            var closestT = HandleMathUtils.ClosestPointOnRay(_raxisRay, cameraRay);
            var hitPoint = _raxisRay.GetPoint(closestT);

            var distance = Vector3.Distance(_parentTransformHandle.target.position, hitPoint);
            var axisScaleDelta = distance / _interactionDistance - 1f;

            var snapping = _parentTransformHandle.scaleSnap;
            var snap = Mathf.Abs(Vector3.Dot(snapping, _axis));
            if (snap != 0)
            {
                if (_parentTransformHandle.snappingType == HandleSnappingType.RELATIVE)
                {
                    axisScaleDelta = Mathf.Round(axisScaleDelta / snap) * snap;
                }
                else
                {
                    var axisStartScale = Mathf.Abs(Vector3.Dot(_startScale, Vector3.one));
                    axisScaleDelta = Mathf.Round((axisScaleDelta + axisStartScale) / snap) * snap - axisStartScale;
                }
            }

            delta = axisScaleDelta;
            var scale = Vector3.Scale(_startScale, Vector3.one * axisScaleDelta);

            _parentTransformHandle.target.localScale = new Vector3(Mathf.Clamp(scale.x, 0.5f, 8f),
                Mathf.Clamp(scale.y, 0.5f, 8f), Mathf.Clamp(scale.z, 0.5f, 8f));
            
            base.Interact(p_previousPosition);
        }

        public override void StartInteraction(Vector3 p_hitPoint)
        {
            base.StartInteraction(p_hitPoint);
            _startScale = _parentTransformHandle.target.localScale;

            var raxis = _parentTransformHandle.space == HandleSpace.LOCAL
                ? _parentTransformHandle.target.rotation * _axis
                : _axis;

            _raxisRay = new Ray(_parentTransformHandle.target.position, raxis);

            var cameraRay = Camera.main.ScreenPointToRay(RuntimeTransformHandle.GetMousePosition());

            var closestT = HandleMathUtils.ClosestPointOnRay(_raxisRay, cameraRay);
            var hitPoint = _raxisRay.GetPoint(closestT);

            _interactionDistance = Vector3.Distance(_parentTransformHandle.target.position, hitPoint);
        }
    }
}