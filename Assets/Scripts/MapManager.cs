using System;
using DroneFootball;
using UnityEngine;

namespace Drone
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