using System;
using UnityEngine;

namespace DroneFootball
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private Map mapPrefab;
        [SerializeField] private GameObject content;

        private void Start()
        {
            var maps = LevelManager.LoadMaps();

            foreach (var map in maps)
            {
                var mapView = Instantiate(mapPrefab, content.transform, false);
                mapView.mapName = map;
            }
        }
    }
}