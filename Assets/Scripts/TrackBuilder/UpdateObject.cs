using UnityEngine;
using UnityEngine.EventSystems;

namespace Drone.Builder
{
    public sealed class UpdateObject : MonoBehaviour, IPointerClickHandler
    {
        public ObjectEditMode objectEditMode = ObjectEditMode.Position;
        public float positionStep = 0.5f;
        public float rotationStep = 10f;
        public EditObject editObject;
        public PositionType positionType;
        public RotationType rotationType;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            switch (objectEditMode)
            {
                case ObjectEditMode.Position:
                    UpdatePosition();
                    break;
                case ObjectEditMode.Rotation:
                    UpdateRotation();
                    break;
            }
        }

        private void UpdateRotation()
        {
            switch (rotationType)
            {
                case RotationType.AroundX:
                    break;
                case RotationType.AroundY:
                    editObject.currentObject.gameObject.transform.Rotate(editObject.currentObject.transform.up * rotationStep, Space.World);
                    break;
                case RotationType.AroundZ:
                    break;
            }
        }

        private void UpdatePosition()
        {
            switch (positionType)
            {
                case PositionType.X:
                    editObject.currentObject.Position = new Vector3(editObject.currentObject.Position.x + positionStep,
                        editObject.currentObject.Position.y, editObject.currentObject.Position.z);
                    break;
                case PositionType.MX:
                    editObject.currentObject.Position = new Vector3(editObject.currentObject.Position.x - positionStep,
                        editObject.currentObject.Position.y, editObject.currentObject.Position.z);
                    break;
                case PositionType.Y:
                    editObject.currentObject.Position = new Vector3(editObject.currentObject.Position.x,
                        editObject.currentObject.Position.y + positionStep, editObject.currentObject.Position.z);
                    break;
                case PositionType.MY:
                    editObject.currentObject.Position = new Vector3(editObject.currentObject.Position.x,
                        editObject.currentObject.Position.y - positionStep, editObject.currentObject.Position.z);
                    break;
                case PositionType.Z:
                    editObject.currentObject.Position = new Vector3(editObject.currentObject.Position.x,
                        editObject.currentObject.Position.y, editObject.currentObject.Position.z + positionStep);
                    break;
                case PositionType.MZ:
                    editObject.currentObject.Position = new Vector3(editObject.currentObject.Position.x,
                        editObject.currentObject.Position.y, editObject.currentObject.Position.z - positionStep);
                    break;
            }
        }
    }
}