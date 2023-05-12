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
            string[] values = str.Split(' ');
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            float z = float.Parse(values[2]);
            return new Vector3(x, y, z);
        }
        
        public static void ChangeLayerRecursively(Transform obj, int layer)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) != "FloorConnection" && LayerMask.LayerToName(obj.gameObject.layer) != "WallConnection" && LayerMask.LayerToName(obj.gameObject.layer) != "SlantConnection")
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
    }
}