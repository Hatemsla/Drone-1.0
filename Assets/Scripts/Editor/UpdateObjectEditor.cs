using Drone.Builder;
using UnityEditor;
using UnityEngine;

namespace Drone.Editor
{
    [CustomEditor(typeof(UpdateObject))]
    public class UpdateObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var updateObject = target as UpdateObject;
            
            updateObject.objectEditMode = (ObjectEditMode)EditorGUILayout.EnumPopup("Object Edit Mode", updateObject.objectEditMode);
            updateObject.editObject = EditorGUILayout.ObjectField("Edit Object", updateObject.editObject, typeof(EditObject), true) as EditObject;
            
            if (updateObject.objectEditMode == ObjectEditMode.Position)
            {
                updateObject.positionStep = EditorGUILayout.FloatField("Position Step", updateObject.positionStep);
                updateObject.positionType = (PositionType)EditorGUILayout.EnumPopup("Position Type", updateObject.positionType);
            }
            else if (updateObject.objectEditMode == ObjectEditMode.Rotation)
            {
                updateObject.rotationStep = EditorGUILayout.FloatField("Rotation Step", updateObject.rotationStep);
                updateObject.rotationType = (RotationType)EditorGUILayout.EnumPopup("Rotation Type", updateObject.rotationType);
            }
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(updateObject);
            }
        }
    }
}