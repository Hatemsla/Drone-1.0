using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Builder
{
    public class BuilderManager : MonoBehaviour
    {
        public string levelName;
        public float gridSize;
        public bool canPlace;
        public Scene levelScene;
        public BuilderUI builderUI;
        public LayerMask layerMask;
        public GameObject pendingObject;
        public TrackObject currentObjectType;
        public Vector3 mousePos;
        public GameObject[] objects;
        public List<GameObject> objectsPool;
        
        private int _currentGroundIndex;
        private RaycastHit _hit;
        private Selection _selection;
            
        private void Start()
        {
            _selection = FindObjectOfType<Selection>();
            CreateObjectsPoolScene();
        }

        private void Update()
        {
            Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 10000, layerMask) && !EventSystem.current.IsPointerOverGameObject())
            {
                mousePos = _hit.point;
                
                if (pendingObject == null) return;
                
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Floor:
                        pendingObject.transform.position = _hit.point;
                        break;
                    case ObjectsType.Wall:
                        pendingObject.transform.position =
                            new Vector3(_hit.point.x, _hit.point.y + 1.25f, _hit.point.z);
                        break;
                    case ObjectsType.Slant:
                        pendingObject.transform.position =
                            new Vector3(_hit.point.x, _hit.point.y + 1.25f, _hit.point.z);
                        break;
                    default:
                        pendingObject.transform.position = _hit.point;
                        break;
                }
            }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceObject();
            }
                
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateObject(Vector3.up, currentObjectType.objectType == ObjectsType.Gate ? -45 : -90, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateObject(Vector3.up, currentObjectType.objectType == ObjectsType.Gate ? 45 : 90, Space.World);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Slant when currentObjectType.rotateStateIndex >= 0:
                        currentObjectType.rotateStateIndex--;
                        RotateObject(Vector3.forward, -20f, Space.Self);
                        break;
                    case ObjectsType.Gate when currentObjectType.heightStateIndex <= 0:
                        currentObjectType.heightStateIndex++;
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Slant when currentObjectType.rotateStateIndex <= 0:
                        currentObjectType.rotateStateIndex++;
                        RotateObject(Vector3.forward, 20f, Space.Self);
                        break;
                    case ObjectsType.Gate when currentObjectType.heightStateIndex >= 0:
                        currentObjectType.heightStateIndex--;
                        break;
                }
            }
        }

        public void PlaceObject()
        {
            try
            {
                ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer("TrackGround"));
                currentObjectType = null;
                pendingObject = null;
            }
            catch
            {
                // ignored
            }
        }
        
        public void ChangeLayerRecursively(Transform obj, int layer)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) != "FloorConnection" && LayerMask.LayerToName(obj.gameObject.layer) != "WallConnection")
            {
                obj.gameObject.layer = layer;
            }

            foreach (Transform child in obj)
            {
                ChangeLayerRecursively(child, layer);
            }
        }

        private void RotateObject(Vector3 axis, float rotateAmount, Space space)
        {
            pendingObject.transform.Rotate(axis, rotateAmount, space);
        }
        
        public void SelectObject(int index)
        {
            pendingObject = Instantiate(objects[index], mousePos, transform.rotation);
            SceneManager.MoveGameObjectToScene(pendingObject, levelScene);
            objectsPool.Add(pendingObject);
            _selection.Deselect();
            _selection.Select(pendingObject);
            currentObjectType = pendingObject.GetComponent<TrackObject>();
            currentObjectType.isActive = true;
        }

        private void CreateObjectsPoolScene()
        {
            levelScene = SceneManager.CreateScene("ObjectsPool");
            MoveObjectsToPoolScene();
        }

        private void MoveObjectsToPoolScene()
        {
            foreach (var obj in objectsPool)
            {
                SceneManager.MoveGameObjectToScene(obj, levelScene);
            }
        }

        public void LoadScene()
        {
            Dictionary<string, Dictionary<string, string>> loadedData =
                new Dictionary<string, Dictionary<string, string>>();
            
            if(objectsPool.Count > 0)
                ClearObject();
            
            string jsonData = File.ReadAllText(Application.dataPath + "/Levels/" + levelName + ".json");
            loadedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp in loadedData)
            {
                string objectName = kvp.Value["name"].Substring(0, kvp.Value["name"].IndexOf('('));
                Vector3 position = ParseVector3(kvp.Value["position"]);
                Vector3 rotation = ParseVector3(kvp.Value["rotation"]);
                Vector3 scale = ParseVector3(kvp.Value["scale"]);
                int layer = Convert.ToInt32(kvp.Value["layer"]);
                GameObject newObj = Instantiate(Resources.Load<GameObject>("TrackObjects/" + objectName), position, Quaternion.Euler(rotation));
                newObj.layer = layer;
                newObj.transform.localScale = scale;
                newObj.name = kvp.Value["name"];
                objectsPool.Add(newObj);
            }
        }
        
        private Vector3 ParseVector3(string str)
        {
            string[] values = str.Split(' ');
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            float z = float.Parse(values[2]);
            return new Vector3(x, y, z);
        }
        
        private void ClearObject()
        {
            foreach (var obj in objectsPool)
            {
                Destroy(obj);
            }
            objectsPool.Clear();
        }
        
        private float RoundToNearsGrid(float pos)
        {
            float xDiff = pos % gridSize;
            pos -= xDiff;

            if (xDiff > (gridSize / 2))
            {
                pos += gridSize;
            }

            return pos;
        }
    }
}