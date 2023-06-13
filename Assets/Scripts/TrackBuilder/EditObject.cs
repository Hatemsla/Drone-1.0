using System;
using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public class EditObject : MonoBehaviour
    {
        public TrackObject currentObject;
        [SerializeField] private EditMenu editMenu;

        private void Start()
        {
            HideEditMenu();
        }

        private Dictionary<int, float> _sliderValues = new Dictionary<int, float>()
        {
            { 0, 0.5f },
            { 1, 1f },
            { 2, 1.5f },
            { 3, 2f },
            { 4, 2.5f },
            { 5, 3f },
            { 6, 3.5f },
            { 7, 4f },
            { 8, 4.5f },
            { 9, 5f },
            { 10, 5.5f },
            { 11, 6f },
            { 12, 6.5f },
            { 13, 7f },
            { 14, 7.5f },
            { 15, 8f },
        };

        public void ShowEditMenu()
        {
            editMenu.gameObject.SetActive(true);
        }

        public void HideEditMenu()
        {
            editMenu.gameObject.SetActive(false);
        }
        
        public void OnSelectObject(TrackObject obj)
        {
            currentObject = obj;
            var angleX = currentObject.Rotation.eulerAngles.x;
            var angleY = currentObject.Rotation.eulerAngles.y;
            var angleZ = currentObject.Rotation.eulerAngles.z;

            editMenu.SetEditPanelParams(currentObject.objectName, currentObject.objectDescription,
                currentObject.Position.x, currentObject.Position.y, currentObject.Position.z, 
                angleX, angleY, angleZ,
                currentObject.Scale.x, currentObject);
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
            if(currentObject.objectType is ObjectsType.Drone or ObjectsType.Gate)
                return;

            var newScale = _sliderValues[(int)value];
            var oldScale = currentObject.Scale.x;
            currentObject.Scale = new Vector3(newScale, newScale, newScale);

            if (newScale < oldScale)
            {
                var currentObjectYOffset = (oldScale - newScale);
                currentObject.Position = new Vector3(currentObject.Position.x,
                    currentObject.Position.y - currentObjectYOffset, currentObject.Position.z);
                currentObject.yOffset -= currentObjectYOffset;
                currentObject.maxMouseDistance -= currentObjectYOffset;
            }
            else if (newScale > oldScale)
            {
                var currentObjectYOffset = (newScale - oldScale);
                currentObject.Position = new Vector3(currentObject.Position.x,
                    currentObject.Position.y + currentObjectYOffset, currentObject.Position.z);
                currentObject.yOffset += currentObjectYOffset;
                currentObject.maxMouseDistance += currentObjectYOffset;
            }
        }
    }
}