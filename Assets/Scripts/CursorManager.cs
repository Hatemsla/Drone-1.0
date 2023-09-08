using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Drone
{
    public sealed class CursorManager : MonoBehaviour
    {
        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D builderCursor;
        [SerializeField] private Vector2 defaultCursorHotspot;
        [SerializeField] private Vector2 builderCursorHotspot;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (scene.buildIndex)
            {
                case 1:
                case 2:
                case 3:
                    Cursor.SetCursor(defaultCursor, defaultCursorHotspot, CursorMode.Auto);
                    break;
                case 4:
                    Cursor.SetCursor(builderCursor, builderCursorHotspot, CursorMode.Auto);
                    break;
            }
        }
    }
}