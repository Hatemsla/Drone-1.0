using Drone.RuntimeHandle.Utils;
using UnityEngine;

namespace Drone.RuntimeHandle.Handles.Scale
{
    public class ScaleGlobal : HandleBase
    {
        protected Vector3 _axis;
        protected Vector3 _startScale;

        public ScaleGlobal Initialize(RuntimeTransformHandle p_parentTransformHandle, Vector3 p_axis, Color p_color)
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
            mf.mesh = MeshUtils.CreateBox(.35f, .35f, .35f);
            var mc = o.AddComponent<MeshCollider>();

            return this;
        }

        public override void Interact(Vector3 p_previousPosition)
        {
            var mouseVector = RuntimeTransformHandle.GetMousePosition() - p_previousPosition;
            var d = (mouseVector.x + mouseVector.y) * Time.deltaTime * 2;
            delta += d;
            
            delta = Mathf.Clamp(delta, 0.5f, 8f);
            
            _parentTransformHandle.target.localScale = _startScale + Vector3.Scale(_startScale, _axis) * delta - Vector3.one;

            base.Interact(p_previousPosition);
        }

        public override void StartInteraction(Vector3 p_hitPoint)
        {
            base.StartInteraction(p_hitPoint);
            _startScale = _parentTransformHandle.target.localScale;
        }
    }
}