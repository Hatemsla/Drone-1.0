using System;
using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public class EditObject : MonoBehaviour
    {
        public TrackObject currentObject;
        [SerializeField] private EditMenu editMenu;

        private Dictionary<int, float> _sliderValues = new Dictionary<int, float>()
        {
            { 0, 0.125f },
            { 1, 0.25f },
            { 2, 0.5f },
            { 3, 1f },
            { 4, 2f },
            { 5, 4f },
            { 6, 8f },
        };

        private bool _isAxisScaleChange;
        
        public void OnSelectObject(TrackObject obj)
        {
            currentObject = obj;
            var angleX = currentObject.Rotation.eulerAngles.x;
            var angleY = currentObject.Rotation.eulerAngles.y;
            var angleZ = currentObject.Rotation.eulerAngles.z;

            editMenu.SetEditPanelParams(currentObject.objectName, currentObject.objectDescription,
                currentObject.Position.x, currentObject.Position.y, currentObject.Position.z, 
                angleX, angleY, angleZ,
                editMenu.XYZValue, currentObject.Scale.x,
                currentObject.Scale.y, currentObject.Scale.z);
        }

        public void OnXPositionChanged(string value)
        {
            if(float.TryParse(value, out var x))
                currentObject.Position = new Vector3(x, currentObject.Position.y, currentObject.Position.z);
        }
        
        public void OnYPositionChanged(string value)
        {
            if(float.TryParse(value, out var y))
                currentObject.Position = new Vector3(currentObject.Position.x, y, currentObject.Position.z);
        }
        
        public void OnZPositionChanged(string value)
        {
            if(float.TryParse(value, out var z))
                currentObject.Position = new Vector3(currentObject.Position.x, currentObject.Position.y, z);
        }
        
        public void OnXRotationChanged(string value)
        {
            if(float.TryParse(value, out var x))
                currentObject.Rotation = Quaternion.Euler(x, currentObject.Rotation.eulerAngles.y, currentObject.Rotation.eulerAngles.z);
        }
        
        public void OnYRotationChanged(string value)
        {
            if(float.TryParse(value, out var y))
                currentObject.Rotation = Quaternion.Euler(currentObject.Rotation.eulerAngles.x, y, currentObject.Rotation.eulerAngles.z);
        }
        
        public void OnZRotationChanged(string value)
        {
            if(float.TryParse(value, out var z))
                currentObject.Rotation = Quaternion.Euler(currentObject.Rotation.eulerAngles.x, currentObject.Rotation.eulerAngles.y, z);
        }

        public void OnXYZScaleChanged(float value)
        {
            if(!_isAxisScaleChange)
                currentObject.Scale = new Vector3(_sliderValues[(int)value], _sliderValues[(int)value],
                _sliderValues[(int)value]);
        }
        
        public void OnXScaleChanged(float value)
        {
            _isAxisScaleChange = true;
            currentObject.Scale = new Vector3(_sliderValues[(int)value], currentObject.Scale.y,
                currentObject.Scale.z);
            _isAxisScaleChange = false;
        }
        
        public void OnYScaleChanged(float value)
        {
            _isAxisScaleChange = true;
            currentObject.Scale = new Vector3(currentObject.Scale.x, _sliderValues[(int)value],
                currentObject.Scale.z);
            _isAxisScaleChange = false;
        }
        
        public void OnZScaleChanged(float value)
        {
            _isAxisScaleChange = true;
            currentObject.Scale = new Vector3(currentObject.Scale.x, currentObject.Scale.y,
                _sliderValues[(int)value]);
            _isAxisScaleChange = false;
        }
    }
}