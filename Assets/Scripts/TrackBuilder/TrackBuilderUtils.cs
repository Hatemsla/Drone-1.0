using System.Collections.Generic;
using cakeslice;
using UnityEngine;

namespace Builder
{
    public static class TrackBuilderUtils
    {
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
                && LayerMask.LayerToName(obj.gameObject.layer) != "Hint" && LayerMask.LayerToName(obj.gameObject.layer) != "Draw")
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
            var activeLayerIndex = LayerMask.NameToLayer(activeLayer);

            var layerMask = (1 << trackGroundLayer) | (1 << activeLayerIndex);

            return layerMask;
        }
    }
}