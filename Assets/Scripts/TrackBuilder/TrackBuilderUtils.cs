using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            var normalizedValue = (value - fromMin) / (fromMax - fromMin);
            var remappedValue = (normalizedValue * (toMax - toMin)) + toMin;

            return remappedValue;
        }
        
        public static void ChangeLayerRecursively(Transform obj, int layer)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.FloorConnection &&
                LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.WallConnection 
                && LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.SlantConnection &&
                LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.IgnoreRaycast
                && LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.Hint &&
                LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.Draw
                && LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.Intangible &&
                LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.Text3D &&
                LayerMask.LayerToName(obj.gameObject.layer) != Idents.Layers.PipeConnection)
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
            var trackGroundLayer = LayerMask.NameToLayer(Idents.Layers.TrackGround);
            var trackHintLayer = LayerMask.NameToLayer(Idents.Layers.Hint);
            var trackDrawLayer = LayerMask.NameToLayer(Idents.Layers.Draw);
            var trackIntangibleLayer = LayerMask.NameToLayer(Idents.Layers.Intangible);
            var trackText3DLayer = LayerMask.NameToLayer(Idents.Layers.Text3D);
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
        
        public static Color HexToColor(string hex)
        {
            hex = hex.TrimStart('#');

            var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            
            byte a = 255;

            return new Color32(r, g, b, a);
        }
        
        public static string[] LoadSounds()
        {
            var soundFiles = Directory.GetFiles(Path.Combine(Application.dataPath, "SoundsSource"), "*.mp3");

            for (var i = 0; i < soundFiles.Length; i++)
                soundFiles[i] = Path.GetFileNameWithoutExtension(soundFiles[i]);

            return soundFiles;
        }
    }
}