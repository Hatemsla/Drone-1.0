using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DroneFootball;
using Newtonsoft.Json;
using Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

namespace Builder
{
    public class BuilderManager : MonoBehaviour
    {
        public string levelName;
        public float interfaceScale;
        public float currentYawSensitivity;
        public int currentSelectObjectIndex;
        public bool canPlace;
        public bool isMove;
        public bool isGameMode;
        public BuilderUI builderUI;
        public DroneBuilderController droneBuilderController;
        public DroneBuilderCheckNode droneBuilderCheckNode;
        public DroneBuilderSoundController droneBuilderSoundController;
        public UndoRedoManager undoRedoManager;
        public LayerMask layerMask;
        public GameObject pendingObject;
        public GameObject copyObject;
        public TrackObject currentObjectType;
        public Vector3 mousePos;
        public Transform targetCheckpoint;
        public GameObject[] objects;
        public List<GameObject> objectsPool;
        [HideInInspector] public Scene levelScene;
        
        private int _currentGroundIndex;
        private bool _isTabPanel;
        private bool _isLevelEnd;
        private Connection[] _connections;
        private RaycastHit _hit;
        private Selection _selection;
        private Camera _mainCamera;
        private Vector3 _mainCameraPrevPosition;
        private Vector3 _startPointerSize;
        private Quaternion _mainCameraPrevRotation;
        private float _dronePrevRotationY;
        private Vector3 _dronePrevPosition;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _startPointerSize = builderUI.pathArrow.sizeDelta;
            _selection = FindObjectOfType<Selection>();
            _selection.Select(droneBuilderController.gameObject);
            _selection.Deselect();
            
            for (int i = 0; i < builderUI.createButtons.Count; i++)
            {
                var i1 = i;
                builderUI.createButtons[i].onClick.AddListener(delegate { SelectObject(i1); });
            }
        }

        private void Start()
        {
            CreateObjectsPoolScene();
        }

        private void Update()
        {
            CheckTabPanel();

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    if (_selection.selectedObject != null)
                    {
                        copyObject = _selection.selectedObject;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.V))
                {
                    if (copyObject != null)
                    {
                        PasteObject(copyObject);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    undoRedoManager.UndoCommand();
                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {
                    undoRedoManager.RedoCommand();
                }
            }

            Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 10000, layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
            {
                mousePos = _hit.point;
                
                if (pendingObject == null) return;
                
                switch (currentObjectType.objectType)
                {
                    case ObjectsType.Floor:
                        pendingObject.transform.position = _hit.point;
                        break;
                    default:
                        pendingObject.transform.position =
                            new Vector3(_hit.point.x, _hit.point.y + currentObjectType.yOffset, _hit.point.z);
                        break;
                }
            }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceObject();
            }

            if(currentObjectType == null || pendingObject == null)
                return;
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateObject(pendingObject.transform.up, -10, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateObject(pendingObject.transform.up, 10, Space.World);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
                var rotateAmount = mouseScroll > 0 ? 1 : -1;
                RotateObject(pendingObject.transform.up, 10 * rotateAmount, Space.World);
            }

            if (Input.GetKey(KeyCode.W))
            {
                ChangeObjectHeight(2 * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                ChangeObjectHeight(-2 * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {
                ChangeObjectScale(2 * Time.deltaTime);
            }
            else if(Input.GetKey(KeyCode.D))
            {
                ChangeObjectScale(-2 * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                RotateObject(Vector3.right, 10f, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                RotateObject(Vector3.right, -10f, Space.World);
            }
        }

        private void CheckTabPanel()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isMove)
            {
                _isTabPanel = !_isTabPanel;
                if(_isTabPanel)
                    droneBuilderSoundController.droneFly.Stop();
                else
                    droneBuilderSoundController.droneFly.Play();
                builderUI.editorTabPanel.SetActive(_isTabPanel && !isGameMode);
                builderUI.gameTabPanel.SetActive(_isTabPanel && isGameMode);
                builderUI.levelResultPanel.SetActive(false);
                Time.timeScale = _isTabPanel ? 0f : 1f;
            }
        }

        private void LateUpdate()
        {
            if(droneBuilderCheckNode.nodes.Count == 0)
                return;
            
            if (droneBuilderCheckNode.currentNode >= droneBuilderCheckNode.nodes.Count)
            {
                builderUI.pathArrow.gameObject.SetActive(false);
                if (!_isLevelEnd)
                    StartCoroutine(EndLevel());
                return;
            }
            
            targetCheckpoint = droneBuilderCheckNode.nodes[droneBuilderCheckNode.currentNode].transform;
            Vector3 realPos = _mainCamera.WorldToScreenPoint(targetCheckpoint.position);
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            Vector3 outPos = realPos;
            float direction = 1;

            builderUI.pathArrow.GetComponent<Image>().sprite = builderUI.outOfScreenIcon;

            if (!IsBehind(targetCheckpoint.position)) // если цель спереди
            {
                if (rect.Contains(realPos)) // и если цель в окне экрана
                {
                    builderUI.pathArrow.GetComponent<Image>().sprite = builderUI.pointerIcon;
                }
            }
            else // если цель cзади
            {
                realPos = -realPos;
                outPos = new Vector3(realPos.x, 0, 0); // позиция иконки - снизу
                if (_mainCamera.transform.position.y < targetCheckpoint.position.y)
                {
                    direction = -1;
                    outPos.y = Screen.height; // позиция иконки - сверху				
                }
            }
            // ограничиваем позицию областью экрана
            float offset = builderUI.pathArrow.sizeDelta.x / 2;
            outPos.x = Mathf.Clamp(outPos.x, offset, Screen.width - offset);
            outPos.y = Mathf.Clamp(outPos.y, offset, Screen.height - offset);

            Vector3 pos = realPos - outPos; // направление к цели из PointerUI 

            RotatePointer(direction * pos);

            builderUI.pathArrow.sizeDelta = new Vector2(_startPointerSize.x / 100 * interfaceScale, _startPointerSize.y / 100 * interfaceScale);
            builderUI.pathArrow.position = outPos;
        }
        
        private bool IsBehind(Vector3 point) // true если point сзади камеры
        {
            Vector3 forward = _mainCamera.transform.TransformDirection(Vector3.forward);
            Vector3 toOther = point - _mainCamera.transform.position;
            if (Vector3.Dot(forward, toOther) < 0) return true;
            return false;
        }

        private void RotatePointer(Vector2 direction) // поворачивает PointerUI в направление direction
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            builderUI.pathArrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private IEnumerator EndLevel()
        {
            _isLevelEnd = true;
            builderUI.levelResultPanel.SetActive(true);
            yield return new WaitForSeconds(5);
            builderUI.levelResultPanel.SetActive(false);
        }

        public void TestLevel()
        {
            isMove = !isMove;
            if (isMove)
            {
                _mainCamera.GetComponent<FreeFlyCamera>().enabled = false;
                TurnAllOutlineEffects(false);
                TurnAllConnections(false);
                _mainCameraPrevPosition = _mainCamera.transform.position;
                _mainCameraPrevRotation = _mainCamera.transform.rotation;
                _dronePrevRotationY = droneBuilderController.transform.localRotation.eulerAngles.y;
                droneBuilderController.yaw = _dronePrevRotationY;
                _dronePrevPosition = droneBuilderController.transform.position;
                _mainCamera.transform.SetParent(droneBuilderController.transform);
                _mainCamera.transform.position = droneBuilderController.GetComponent<CameraController>().camerasPositions[0].position;
                _mainCamera.transform.rotation = droneBuilderController.GetComponent<CameraController>().camerasPositions[0].rotation;
                droneBuilderController.GetComponent<Rigidbody>().isKinematic = false;
                droneBuilderController.GetComponent<Rigidbody>().useGravity = true;
                droneBuilderController.GetComponent<CameraController>().enabled = true;
                builderUI.createPanel.SetActive(false);
                builderUI.editButtons.SetActive(false);
                if(droneBuilderCheckNode.nodes.Count > 0)
                    builderUI.pathArrow.gameObject.SetActive(true);
                droneBuilderCheckNode.currentNode = 0;
                droneBuilderSoundController.droneFly.Play();
                _selection.enabled = false;
            }
            else
            {
                _mainCamera.transform.parent = null;
                _mainCamera.GetComponent<FreeFlyCamera>().enabled = true;
                TurnAllConnections(true);
                _mainCamera.transform.position = _mainCameraPrevPosition;
                _mainCamera.transform.rotation = _mainCameraPrevRotation;
                droneBuilderController.yaw = _dronePrevRotationY;
                droneBuilderController.transform.position = _dronePrevPosition;
                droneBuilderController.transform.localRotation = Quaternion.Euler(0, _dronePrevRotationY, 0);
                droneBuilderController.GetComponent<Rigidbody>().isKinematic = true;
                droneBuilderController.GetComponent<Rigidbody>().useGravity = false;
                droneBuilderController.GetComponent<CameraController>().enabled = true;
                builderUI.editorTabPanel.SetActive(false);
                builderUI.gameTabPanel.SetActive(false);
                Time.timeScale = 1f;
                _isTabPanel = false;
                builderUI.createPanel.SetActive(true);
                builderUI.editButtons.SetActive(true);
                builderUI.pathArrow.gameObject.SetActive(false);
                droneBuilderSoundController.droneFly.Stop();
                _selection.enabled = true;
            }
        }

        public void StartLevel()
        {
            LoadScene();
            TestLevel();
        }

        private void TurnAllConnections(bool turn)
        {
            if(!turn)
                _connections = FindObjectsOfType<Connection>();
            
            foreach (var connection in _connections)
            {
                connection.gameObject.SetActive(turn);
            }
        }

        public void TurnAllOutlineEffects(bool turn)
        {
            var outlines = FindObjectsOfType<Outline>();
            foreach (var outline in outlines)
            {
                outline.enabled = turn;
            }
        }

        public void PlaceObject()
        {
            try
            {
                pendingObject = _selection.selectedObject;
                ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer("TrackGround"));
                undoRedoManager.ExecuteCommand(new PlaceCommand(objects[currentSelectObjectIndex],
                    pendingObject.transform.position, pendingObject.transform.rotation, pendingObject));
                currentObjectType = null;
                pendingObject = null;
            }
            catch
            {
                // ignored
            }
        }

        public void PutObject()
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

        private void ChangeObjectHeight(float value)
        {
            if(currentObjectType == null)
                return;
            
            currentObjectType.yOffset += value;
                // _mainCamera.transform.Translate(0, value, 0, Space.Self);
        }

        private void ChangeObjectScale(float value)
        {
            pendingObject.transform.localScale += new Vector3(value, value, value);
            currentObjectType.yOffset += value;
        }
        
        public void ChangeLayerRecursively(Transform obj, int layer)
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

        public void OffOutlineRecursively(Transform obj)
        {
            if(obj.gameObject.GetComponent<Outline>())
                obj.gameObject.GetComponent<Outline>().enabled = false;

            foreach (Transform child in obj)
            {
                OffOutlineRecursively(child);
            }
        }

        private void RotateObject(Vector3 axis, float rotateAmount, Space space)
        {
            if(pendingObject == null)
                return;
            pendingObject.transform.Rotate(axis, rotateAmount, space);
        }
        
        public void SelectObject(int index)
        {
            if(pendingObject != null)
                PlaceObject();

            currentSelectObjectIndex = index;
            pendingObject = Instantiate(objects[index], mousePos, transform.rotation);
            SceneManager.MoveGameObjectToScene(pendingObject, levelScene);
            objectsPool.Add(pendingObject);
            _selection.Deselect();
            _selection.Select(pendingObject);
            currentObjectType = pendingObject.GetComponent<TrackObject>();
            currentObjectType.isActive = true;

            if (currentObjectType.objectType == ObjectsType.Gate)
            {
                currentObjectType.GetComponent<BuilderCheckpointTrigger>().checkpointId =
                    droneBuilderCheckNode.nodes.Count;
                droneBuilderCheckNode.AddNode(pendingObject.transform);
            }
        }

        private void PasteObject(GameObject obj)
        {
            if(obj == null)
                return;
            
            pendingObject = Instantiate(obj, mousePos, copyObject.transform.rotation);
            ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer("Track"));
            SceneManager.MoveGameObjectToScene(pendingObject, levelScene);
            objectsPool.Add(pendingObject);
            _selection.Deselect();
            _selection.Select(pendingObject);
            currentObjectType = pendingObject.GetComponent<TrackObject>();
            currentObjectType.isActive = true;
            
            if (currentObjectType.objectType == ObjectsType.Gate)
            {
                currentObjectType.GetComponent<BuilderCheckpointTrigger>().checkpointId =
                    droneBuilderCheckNode.nodes.Count;
                droneBuilderCheckNode.AddNode(pendingObject.transform);
            }
        }

        private void CreateObjectsPoolScene()
        {
            levelScene = SceneManager.CreateScene("ObjectsPool");
            MoveObjectsToPoolScene();
        }

        public void MoveObjectsToPoolScene()
        {
            foreach (var obj in objectsPool)
            {
                if(obj.GetComponent<BuilderCheckpointTrigger>())
                    droneBuilderCheckNode.nodes.Add(obj.GetComponent<BuilderCheckpointTrigger>());
                SceneManager.MoveGameObjectToScene(obj, levelScene);
            }

            if (droneBuilderCheckNode.nodes.Count > 0)
            {
                builderUI.pathArrow.gameObject.SetActive(true);
            }

            FindObjectOfType<Server>().droneBuilderController = droneBuilderController;
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
                ChangeLayerRecursively(newObj.transform, layer);
                OffOutlineRecursively(newObj.transform);
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
    }
}