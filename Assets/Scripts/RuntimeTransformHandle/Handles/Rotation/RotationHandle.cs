using System.Collections.Generic;
using Drone.Builder;
using UnityEngine;

namespace Drone.RuntimeHandle.Handles.Rotation
{
    public class RotationHandle : MonoBehaviour
    {
        protected RuntimeTransformHandle _parentTransformHandle;
        protected List<RotationAxis> _axes;

        public RotationHandle Initialize(RuntimeTransformHandle p_parentTransformHandle, EditObject editObject)
        {
            _parentTransformHandle = p_parentTransformHandle;
            transform.SetParent(_parentTransformHandle.transform, false);

            _axes = new List<RotationAxis>();
            
            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.right, Color.red, editObject));
            
            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.up, Color.green, editObject));

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.forward, Color.blue, editObject));

            return this;
        }

        public void Destroy()
        {
            foreach (RotationAxis axis in _axes)
                Destroy(axis.gameObject);
            
            Destroy(this);
        }
    }
}