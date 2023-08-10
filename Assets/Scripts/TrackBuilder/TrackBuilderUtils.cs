using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cakeslice;
using UnityEngine;

namespace Drone.Builder
{
    public static class TrackBuilderUtils
    {
        public static bool GetActive(IEnumerable<InteractiveObject> controlObjs)
        {
            return controlObjs.Any(x => x.isActive);
        }
        
        public static void OffOutlineRecursively(Transform obj)
        {
            if(obj.gameObject.GetComponent<Outline>())
                obj.gameObject.GetComponent<Outline>().enabled = false;

            foreach (Transform child in obj)
            {
                OffOutlineRecursively(child);
            }
        }
        
        public static Vector3 ParseVector3(string str)
        {
            var values = str.Split(' ');
            var x = float.Parse(values[0]);
            var y = float.Parse(values[1]);
            var z = float.Parse(values[2]);
            return new Vector3(x, y, z);
        }
        
        public static void ChangeLayerRecursively(Transform obj, int layer)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) != "FloorConnection" && LayerMask.LayerToName(obj.gameObject.layer) != "WallConnection" 
                && LayerMask.LayerToName(obj.gameObject.layer) != "SlantConnection" && LayerMask.LayerToName(obj.gameObject.layer) != "Ignore Raycast"
                && LayerMask.LayerToName(obj.gameObject.layer) != "Hint" && LayerMask.LayerToName(obj.gameObject.layer) != "Draw"
                && LayerMask.LayerToName(obj.gameObject.layer) != "Intangible" && LayerMask.LayerToName(obj.gameObject.layer) != "Text3D")
            {
                obj.gameObject.layer = layer;
            }

            foreach (Transform child in obj)
            {
                ChangeLayerRecursively(child, layer);
            }
        }
        
        public static void TurnAllOutlineEffects(Outline[] outlines, bool turn)
        {
            foreach (var outline in outlines)
            {
                outline.enabled = turn;
            }
        }
        
        public static void TurnAllConnections(Connection[] connections, bool turn)
        {
            foreach (var connection in connections)
            {
                connection.gameObject.SetActive(turn);
            }
        }
        
        public static void TurnTrackObjects(List<GameObject> pendingObjects, bool turn)
        {
            foreach (var pendingObject in pendingObjects)
            {
                pendingObject.GetComponent<TrackObject>().isActive = turn;
            }
        }
        
        public static LayerMask SetLayerMask(string activeLayer)
        {
            var trackGroundLayer = LayerMask.NameToLayer("TrackGround");
            var trackHintLayer = LayerMask.NameToLayer("Hint");
            var trackDrawLayer = LayerMask.NameToLayer("Draw");
            var trackIntangibleLayer = LayerMask.NameToLayer("Intangible");
            var trackText3DLayer = LayerMask.NameToLayer("Text3D");
            var activeLayerIndex = LayerMask.NameToLayer(activeLayer);

            var layerMask = (1 << trackGroundLayer) | (1 << trackHintLayer) | (1 << trackDrawLayer) |
                            (1 << activeLayerIndex) | (1 << trackIntangibleLayer) | (1 << trackText3DLayer);

            return layerMask;
        }

        public static IEnumerator SetPositionSmoothly(Transform objectTransform, Vector3 startPos, Vector3 endPos, float time = 0.5f)
        {
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                var t = elapsedTime / time;
                objectTransform.position = Vector3.Lerp(startPos, endPos, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectTransform.position = endPos;
        }
        
        public static Color GetColorFromOption(ColorOption option)
        {
            switch (option)
            {
                case ColorOption.Белый:
                    return Color.white;
                case ColorOption.Красный:
                    return Color.red;
                case ColorOption.Зелёный:
                    return Color.green;
                case ColorOption.Жёлтый:
                    return Color.yellow;
                case ColorOption.Синий:
                    return Color.blue;
                default:
                    return Color.white;
            }
        }
        
        public static Color HexToColor(string hex)
        {
            hex = hex.TrimStart('#');

            var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            
            byte a = 255;

            return new Color32(r, g, b, a);
        }
    }
}