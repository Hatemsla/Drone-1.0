using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cinemachine;
using Drone;
using DroneFootball;
using Newtonsoft.Json;
using Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

namespace Builder
{
    public class TestLevelEvent : UnityEvent {}
    
    public class BuilderManager : MonoBehaviour
    {
        public static BuilderManager Instance;
        public string levelName;
        public float interfaceScale;
        public float currentYawSensitivity;
        public float currentTime;
        public int currentSelectObjectIndex;
        public bool canPlace;
        public bool isMove;
        public bool isGameMode;
        public bool isLoadLevel;
        public bool isGameLevel;
        public BuilderUI builderUI;
        public EditMenu editMenu;
        public BuilderAudioManager audioManager;
        public DroneBuilderController droneBuilderController;
        public DroneBuilderCheckNode droneBuilderCheckNode;
        public DroneBuilderSoundController droneBuilderSoundController;
        public UndoRedoManager undoRedoManager;
        public AsyncLoad asyncLoad;
        public Timer timer;
        public CinemachineBrain cameraBrain;
        public BuilderCameraController cameraController;
        public FreeFlyCamera freeFlyCamera;
        public LayerMask layerMask;
        public GameObject pendingObject;
        public List<GameObject> pendingObjects = new();
        public GameObject copyObject;
        public TrackObject currentObjectType;
        public Vector3 mousePos;
        public Transform targetCheckpoint;
        public GameObject[] objects;
        public List<GameObject> objectsPool;
        [HideInInspector] public Scene levelScene;

        public TestLevelEvent TestLevelEvent;

        private int _currentGroundIndex;
        private bool _isTabPanel;
        private bool _isLevelEnd;
        private Connection[] _connections;
        private RaycastHit _hit;
        private Selection _selection;
        private Vector3 _mainCameraPrevPosition;
        private Vector3 _startPointerSize;
        private Quaternion _mainCameraPrevRotation;
        private float _dronePrevRotationY;
        private Vector3 _dronePrevPosition;
        private Vector3 _prevMousePos;

        public Action loadingComplete;

        private void Awake()
        {
            Instance = this;
            TestLevelEvent = new TestLevelEvent();
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
            if (isLoadLevel)
            {
                StartCoroutine(LoadScene());
            }
            else if (isGameLevel)
            {
                loadingComplete += TestLevel;
                StartLevel();
            }
            else
            {
                CreateObjectsPoolScene();
            }
        }

        private void Update()
        {
            if (isMove)
            {
                currentTime += Time.deltaTime;
                SetDroneParameters();
            }

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
                    if(pendingObject != null || pendingObjects.Count > 0)
                        PlaceObjects();
                
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

            var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 10000, layerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
            {
                mousePos = _hit.point;
                
                if (pendingObjects.Count == 0 || pendingObject == null) return;

                if (pendingObjects.Count > 1)
                {
                    var commonCenter = CalculateCommonCenter(pendingObjects);
                    var offset = mousePos - commonCenter;

                    foreach (var pendingObj in pendingObjects)
                    {
                        pendingObj.transform.position += new Vector3(offset.x, 0, offset.z);
                    }
                }
                else
                {
                    pendingObject.transform.position = currentObjectType.objectType switch
                    {
                        ObjectsType.Floor => mousePos,
                        _ => new Vector3(mousePos.x, mousePos.y + currentObjectType.yOffset, mousePos.z)
                    };
                }
            }

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceObjects();
            }

            if(currentObjectType == null || pendingObject == null || pendingObjects.Count > 1)
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

            if (currentObjectType.objectType != ObjectsType.Drone && currentObjectType.objectType != ObjectsType.Gate)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    var angleX = currentObjectType.Rotation.eulerAngles.x;
                    var angleY = currentObjectType.Rotation.eulerAngles.y;
                    var angleZ = currentObjectType.Rotation.eulerAngles.z;
                    editMenu.SetEditPanelParams(currentObjectType.objectName, currentObjectType.objectDescription,
                        currentObjectType.Position.x, currentObjectType.Position.y, currentObjectType.Position.z, 
                        angleX, angleY, angleZ,
                        currentObjectType.Scale.x + 0.5f, currentObjectType);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    var angleX = currentObjectType.Rotation.eulerAngles.x;
                    var angleY = currentObjectType.Rotation.eulerAngles.y;
                    var angleZ = currentObjectType.Rotation.eulerAngles.z;
                    editMenu.SetEditPanelParams(currentObjectType.objectName, currentObjectType.objectDescription,
                        currentObjectType.Position.x, currentObjectType.Position.y, currentObjectType.Position.z, 
                        angleX, angleY, angleZ,
                        currentObjectType.Scale.x - 0.5f, currentObjectType);
                }
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

        private void SetDroneParameters()
        {
            builderUI.speedText.text = $"{droneBuilderController.currentSpeed:00}";
            float minutes = Mathf.FloorToInt(currentTime / 60);
            float seconds = Mathf.FloorToInt(currentTime % 60);
            builderUI.timeText.text = $"{minutes:00}:{seconds:00}";
            builderUI.batteryText.text = $"{droneBuilderController.droneRpgController.DroneData.Battery:00}"; 
            builderUI.checkpointsCountText.text = $"{droneBuilderCheckNode.currentNode}/{droneBuilderCheckNode.nodes.Count}";
            builderUI.armorBar.TurnBars(droneBuilderController.droneRpgController.GetCurrentHealthIndex(droneBuilderController.droneRpgController.DroneData.Armor));
            builderUI.healthBar.TurnBars(droneBuilderController.droneRpgController.GetCurrentHealthIndex(droneBuilderController.droneRpgController.DroneData.Health));
        }

        private Vector3 CalculateCommonCenter(List<GameObject> objectsList)
        {
            var center = Vector3.zero;

            foreach (var obj in objectsList)
            {
                center += obj.transform.position;
            }

            center /= objectsList.Count;

            return center;
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
            var realPos = cameraBrain.OutputCamera.WorldToScreenPoint(targetCheckpoint.position);
            var rect = new Rect(0, 0, Screen.width, Screen.height);

            var outPos = realPos;
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
                if (cameraBrain.transform.position.y < targetCheckpoint.position.y)
                {
                    direction = -1;
                    outPos.y = Screen.height; // позиция иконки - сверху				
                }
            }
            // ограничиваем позицию областью экрана
            var offset = builderUI.pathArrow.sizeDelta.x / 2;
            outPos.x = Mathf.Clamp(outPos.x, offset, Screen.width - offset);
            outPos.y = Mathf.Clamp(outPos.y, offset, Screen.height - offset);

            var pos = realPos - outPos; // направление к цели из PointerUI 

            RotatePointer(direction * pos);

            builderUI.pathArrow.sizeDelta = new Vector2(_startPointerSize.x / 100 * interfaceScale, _startPointerSize.y / 100 * interfaceScale);
            builderUI.pathArrow.position = outPos;
        }
        
        private bool IsBehind(Vector3 point) // true если point сзади камеры
        {
            var forward = cameraBrain.transform.TransformDirection(Vector3.forward);
            var toOther = point - cameraBrain.transform.position;
            return Vector3.Dot(forward, toOther) < 0;
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
                foreach (var obj in objectsPool)
                    obj.SetActive(true);
                
                currentTime = 0;
                droneBuilderController.droneRpgController.DroneData = new DroneData(100, 100, 100);
                builderUI.droneView.SetActive(true);
                freeFlyCamera.enabled = false;
                freeFlyCamera.GetComponent<CinemachineVirtualCamera>().Priority = 0;
                cameraController.isSwitch = true;
                cameraController.SetUpCamerasDefaultPriority();
                var outlines = FindObjectsOfType<Outline>();
                TrackBuilderUtils.TurnAllOutlineEffects(outlines, false);
                _connections = FindObjectsOfType<Connection>();
                TrackBuilderUtils.TurnAllConnections(_connections, false);
                _dronePrevRotationY = droneBuilderController.transform.localRotation.eulerAngles.y;
                droneBuilderController.yaw = _dronePrevRotationY;
                _dronePrevPosition = droneBuilderController.transform.position;
                cameraBrain.transform.SetParent(droneBuilderController.transform);
                droneBuilderController.GetComponent<Rigidbody>().isKinematic = false;
                droneBuilderController.GetComponent<Rigidbody>().useGravity = true;
                builderUI.createPanel.SetActive(false);
                builderUI.editButtons.SetActive(false);
                builderUI.objectEditPanel.SetActive(false);
                if(droneBuilderCheckNode.nodes.Count > 0)
                    builderUI.pathArrow.gameObject.SetActive(true);
                droneBuilderCheckNode.currentNode = 0;
                droneBuilderSoundController.droneFly.Play();
                _selection.Deselect();
                _selection.enabled = false;
            }
            else
            {
                TestLevelEvent.Invoke();
                builderUI.droneView.SetActive(false);
                freeFlyCamera.enabled = true;
                freeFlyCamera.GetComponent<CinemachineVirtualCamera>().Priority = 10;
                cameraController.isSwitch = false;
                cameraController.SetUpCamerasZeroPriority();
                TrackBuilderUtils.TurnAllConnections(_connections, true);
                droneBuilderController.yaw = _dronePrevRotationY;
                droneBuilderController.transform.position = _dronePrevPosition;
                droneBuilderController.transform.localRotation = Quaternion.Euler(0, _dronePrevRotationY, 0);
                droneBuilderController.GetComponent<Rigidbody>().isKinematic = true;
                droneBuilderController.GetComponent<Rigidbody>().useGravity = false;
                builderUI.editorTabPanel.SetActive(false);
                builderUI.gameTabPanel.SetActive(false);
                Time.timeScale = 1f;
                _isTabPanel = false;
                builderUI.createPanel.SetActive(true);
                builderUI.editButtons.SetActive(true);
                builderUI.objectEditPanel.SetActive(true);
                builderUI.pathArrow.gameObject.SetActive(false);
                droneBuilderSoundController.droneFly.Stop();
                _selection.enabled = true;
            }
        }

        public void StartLevel()
        {
            StartCoroutine(LoadScene());
        }
        
        public IEnumerator LoadScene()
        {
            builderUI.pathArrow.gameObject.SetActive(false);
            builderUI.editButtons.SetActive(false);
            builderUI.createPanel.SetActive(false);
            builderUI.loadLevelPanel.SetActive(true);
            audioManager.StartPlay();
            Dictionary<string, Dictionary<string, string>> loadedData;
            
            if(objectsPool.Count > 0)
                ClearObject();

            var filePath = Application.dataPath + "/Levels/" + levelName + ".json";

            var jsonData = File.ReadAllText(filePath);
            loadedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
            
            foreach (var kvp in loadedData)
            {
                var objectName = kvp.Value["name"].Substring(0, kvp.Value["name"].IndexOf('('));
                var position = TrackBuilderUtils.ParseVector3(kvp.Value["position"]);
                var rotation = TrackBuilderUtils.ParseVector3(kvp.Value["rotation"]);
                var scale = TrackBuilderUtils.ParseVector3(kvp.Value["scale"]);
                var layer = Convert.ToInt32(kvp.Value["layer"]);
                var yOffset = Convert.ToSingle(kvp.Value["yOffset"]);
                var maxMouseDistance = Convert.ToSingle(kvp.Value["maxMouseDistance"]);
                var newObj = Instantiate(Resources.Load<GameObject>("TrackObjects/" + objectName), position, Quaternion.Euler(rotation));
                yield return new WaitForSeconds(0.01f);
                TrackBuilderUtils.ChangeLayerRecursively(newObj.transform, layer);
                TrackBuilderUtils.OffOutlineRecursively(newObj.transform);
                newObj.transform.localScale = scale;
                newObj.name = kvp.Value["name"];
                var trackObj = newObj.GetComponent<TrackObject>();
                trackObj.yOffset = yOffset;
                trackObj.maxMouseDistance = maxMouseDistance;
                objectsPool.Add(newObj);
            }
            
            builderUI.editButtons.SetActive(true);
            builderUI.createPanel.SetActive(true);
            builderUI.loadLevelPanel.SetActive(false);
            audioManager.EndPlay();
            CreateObjectsPoolScene();
            loadingComplete?.Invoke();
        }


        public void PlaceObjects()
        {
            try
            {
                if (pendingObjects.Count > 1)
                {
                    pendingObjects = new List<GameObject>(_selection.selectedObjects);
                    foreach (var pendingObj in pendingObjects)
                    {
                        TrackBuilderUtils.ChangeLayerRecursively(pendingObj.transform, LayerMask.NameToLayer("TrackGround"));
                    }
                }
                else
                {
                    pendingObject = _selection.selectedObject;
                    currentObjectType = _selection.selectedObject.GetComponent<TrackObject>();
                    TrackBuilderUtils.ChangeLayerRecursively(pendingObject.transform,
                        LayerMask.NameToLayer("TrackGround"));
                    undoRedoManager.ExecuteCommand(new PlaceCommand(objects[currentSelectObjectIndex],
                        currentObjectType.Position, currentObjectType.Scale, currentObjectType.Rotation, pendingObject,
                        currentObjectType.yOffset));
                }
                TrackBuilderUtils.TurnTrackObjects(pendingObjects, false);
                pendingObjects.Clear();
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
                if (pendingObjects.Count != 1) return;
                TrackBuilderUtils.ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer("TrackGround"));
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

        private void RotateObject(Vector3 axis, float rotateAmount, Space space)
        {
            if(pendingObject == null)
                return;
            pendingObject.transform.Rotate(axis, rotateAmount, space);
        }
        
        public void SelectObject(int index)
        {
            if(pendingObject != null)
                PlaceObjects();

            currentSelectObjectIndex = index;
            pendingObject = Instantiate(objects[index], mousePos, transform.rotation);
            pendingObjects.Add(pendingObject);
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
            pendingObjects.Add(pendingObject);
            TrackBuilderUtils.ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer("Track"));
            TrackBuilderUtils.TurnTrackObjects(pendingObjects, true);
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
                    droneBuilderCheckNode.AddNode(obj.transform);
                SceneManager.MoveGameObjectToScene(obj, levelScene);
            }

            if (droneBuilderCheckNode.nodes.Count > 0)
            {
                builderUI.pathArrow.gameObject.SetActive(true);
            }

            FindObjectOfType<Server>().droneBuilderController = droneBuilderController;
        }
        
        private void ClearObject()
        {
            foreach (var obj in objectsPool)
            {
                Destroy(obj);
            }
            objectsPool.Clear();
            pendingObjects.Clear();
        }

        public void TurnUI()
        {
            builderUI.uiPanel.SetActive(false);
        }
    }
}