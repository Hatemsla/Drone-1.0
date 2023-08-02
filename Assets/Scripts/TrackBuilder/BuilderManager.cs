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
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

namespace Builder
{
    public class BuilderManager : MonoBehaviour
    {
        public static BuilderManager Instance;
        public string levelName;
        public float interfaceScale;
        public float currentYawSensitivity;
        public int currentSelectObjectIndex;
        public bool isMove;
        public bool isGameMode;
        public bool isLoadLevel;
        public bool isGameLevel;
        public bool isInputText;
        public bool isActivRed;
        public bool isActivBlue;
        public bool isActivYellow;
        public bool isActivGreen;
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
        public ObjectsType noScaleEditableObjects;
        public List<GameObject> pendingObjects = new();
        public GameObject copyObject;
        public TrackObject currentObjectType;
        public Vector3 mousePos;
        public Transform targetCheckpoint;
        public GameObject[] objects;
        public List<GameObject> objectsPool;

        [HideInInspector] public Scene levelScene;
        public event Action TestLevelEvent;
        public event Action LoadingCompleteEvent;
        public event Action ObjectChangeSceneEvent;

        private List<Lamp> _lamps;
        private int _currentGroundIndex;
        private bool _isTabPanel;
        private bool _isExitPanel;
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
        private float _objectHeightValue;

        private void Awake()
        {
            Instance = this;
            _startPointerSize = builderUI.pathArrow.sizeDelta;
            _selection = FindObjectOfType<Selection>();
            _selection.Select(droneBuilderController.gameObject);
            _selection.Deselect();
        }

        private void Start()
        {
            for (var i = 0; i < builderUI.createButtons.Count; i++)
            {
                var i1 = i;
                builderUI.createButtons[i].onClick.AddListener(delegate { SelectObject(i1); });
                var trackObject = objects[i].GetComponent<TrackObject>();
                builderUI.objectPreInfos[i].objectName = trackObject.objectName;
                builderUI.objectPreInfos[i].objectDesc = trackObject.objectDescription;
            }

            builderUI.pathArrow.gameObject.SetActive(false);

            LoadingCompleteEvent += RewindManager.Instance.FindRewindObjects;
            LoadingCompleteEvent += RewindManager.Instance.RestartTracking;

            if (isLoadLevel)
            {
                StartCoroutine(LoadScene());
            }
            else if (isGameLevel)
            {
                LoadingCompleteEvent += TestLevel;
                StartLevel();
            }
            else
            {
                CreateObjectsPoolScene();
            }
        }

        private void OnEnable()
        {
            TestLevelEvent += InputManager.Instance.TurnActionMaps;
            InputManager.Instance.CopyObjectEvent += CopyObject;
            InputManager.Instance.PasteObjectEvent += PasteObject;
            InputManager.Instance.UndoObjectEvent += UndoCommand;
            InputManager.Instance.RedoObjectEvent += RedoCommand;
            InputManager.Instance.PlaceObjectEvent += PlaceObject;
            InputManager.Instance.PlaceAndPickupObjectEvent += PlaceAndPickupObject;
            InputManager.Instance.RotateYObjectEvent += RotateYObject;
            InputManager.Instance.RotateXObjectEvent += RotateXObject;
            InputManager.Instance.ChangeObjectHeightEvent += ChangeObjectHeight;
            InputManager.Instance.ChangeObjectScaleEvent += ChangeObjectScale;
            InputManager.Instance.ExitGameEvent += CheckTabPanel;
            InputManager.Instance.ExitBuilderEvent += OpenExitPanel;
        }

        private void OnDisable()
        {
            LoadingCompleteEvent -= RewindManager.Instance.FindRewindObjects;
            LoadingCompleteEvent -= RewindManager.Instance.RestartTracking;
            LoadingCompleteEvent -= TestLevel;
            TestLevelEvent -= InputManager.Instance.TurnActionMaps;
            InputManager.Instance.CopyObjectEvent -= CopyObject;
            InputManager.Instance.PasteObjectEvent -= PasteObject;
            InputManager.Instance.UndoObjectEvent -= UndoCommand;
            InputManager.Instance.RedoObjectEvent -= RedoCommand;
            InputManager.Instance.PlaceObjectEvent -= PlaceObject;
            InputManager.Instance.PlaceAndPickupObjectEvent -= PlaceAndPickupObject;
            InputManager.Instance.RotateYObjectEvent -= RotateYObject;
            InputManager.Instance.RotateXObjectEvent -= RotateXObject;
            InputManager.Instance.ChangeObjectHeightEvent -= ChangeObjectHeight;
            InputManager.Instance.ChangeObjectScaleEvent -= ChangeObjectScale;
            InputManager.Instance.ExitGameEvent -= CheckTabPanel;
            InputManager.Instance.ExitBuilderEvent -= OpenExitPanel;
        }
        
        private void OpenExitPanel()
        {
            _isExitPanel = !_isExitPanel;
            builderUI.exitBuilderPanel.SetActive(_isExitPanel);
            builderUI.createPanel.SetActive(!_isExitPanel);
            builderUI.editButtons.SetActive(!_isExitPanel);
            builderUI.objectEditPanel.SetActive(!_isExitPanel);
            Time.timeScale = _isExitPanel ? 0f : 1f;
        }

        private void RotateXObject(float value)
        {
            if (IsNoEditObject())
                return;

            if (value > 0)
                RotateObject(pendingObject.transform.right, 10f, Space.World);
            else if (value < 0)
                RotateObject(pendingObject.transform.right, -10f, Space.World);
        }

        private void ChangeObjectScale(float value)
        {
            if (IsNoEditObject() || noScaleEditableObjects.HasFlag(currentObjectType.objectType))
                return;

            var angleX = currentObjectType.Rotation.eulerAngles.x;
            var angleY = currentObjectType.Rotation.eulerAngles.y;
            var angleZ = currentObjectType.Rotation.eulerAngles.z;
            editMenu.SetEditPanelParams(currentObjectType.objectName, currentObjectType.objectDescription,
                currentObjectType.Position.x, currentObjectType.Position.y, currentObjectType.Position.z,
                angleX, angleY, angleZ,
                currentObjectType.Scale.x + value * 0.5f, currentObjectType);
        }

        private void RotateYObject(float value)
        {
            if (IsNoEditObject())
                return;

            if (value > 0)
                RotateObject(pendingObject.transform.up, 10, Space.World);
            else if (value < 0)
                RotateObject(pendingObject.transform.up, -10, Space.World);
        }

        private bool IsNoEditObject()
        {
            return currentObjectType == null || pendingObject == null || pendingObjects.Count > 1;
        }

        private void PlaceAndPickupObject()
        {
            if (_selection.selectedObjects.Count > 0 && _selection.selectedObject != null)
            {
                PlaceObjects();
                SelectObject(currentSelectObjectIndex);
            }
        }

        private void PlaceObject()
        {
            if (_selection.selectedObjects.Count > 0 && _selection.selectedObject != null)
            {
                PlaceObjects();
                _selection.Deselect();
            }
        }

        private void Update()
        {
            if (isInputText)
                return;

            if (isMove)
            {
                SetDroneParameters();
            }

            var ray = Camera.main!.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out _hit, 10000, layerMask, QueryTriggerInteraction.Ignore) &&
                !EventSystem.current.IsPointerOverGameObject())
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

            UpdateObjectHeight();
        }

        private void UpdateObjectHeight()
        {
            if (IsNoEditObject())
                return;

            currentObjectType.yOffset += _objectHeightValue * 2 * Time.deltaTime;
        }

        private void UndoCommand()
        {
            undoRedoManager.UndoCommand();
        }

        private void RedoCommand()
        {
            undoRedoManager.RedoCommand();
        }

        private void PasteObject()
        {
            if (pendingObject != null || pendingObjects.Count > 0)
                PlaceObjects();

            if (copyObject != null)
            {
                PasteObject(copyObject);
            }
        }

        private void CopyObject()
        {
            if (_selection.selectedObject != null)
            {
                copyObject = _selection.selectedObject;
            }
        }

        private void SetDroneParameters()
        {
            builderUI.speedText.text = $"{droneBuilderController.currentSpeed:00}";
            float minutes = Mathf.FloorToInt(timer.waitForEndGame / 60);
            float seconds = Mathf.FloorToInt(timer.waitForEndGame % 60);
            builderUI.timeText.text = $"{minutes:00}:{seconds:00}";
            builderUI.terminalTimeText.text = $"{minutes:00}:{seconds:00}";
            builderUI.batteryText.text = $"{droneBuilderController.droneRpgController.Battery:00}";
            builderUI.checkpointsCountText.text =
                $"{droneBuilderCheckNode.currentNode}/{droneBuilderCheckNode.nodes.Count}";
            builderUI.coinsCountText.text = $"{droneBuilderController.droneRpgController.Coins}";
            builderUI.crystalsCountText.text = $"{droneBuilderController.droneRpgController.Crystals}";
            builderUI.armorBar.TurnBars(
                droneBuilderController.droneRpgController.GetCurrentHealthIndex(droneBuilderController
                    .droneRpgController.Armor));
            builderUI.healthBar.TurnBars(
                droneBuilderController.droneRpgController.GetCurrentHealthIndex(droneBuilderController
                    .droneRpgController.Health));
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
            if (isMove)
            {
                _isTabPanel = !_isTabPanel;
                if (_isTabPanel)
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
            if (droneBuilderCheckNode.nodes.Count == 0)
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

            builderUI.pathArrow.sizeDelta = new Vector2(_startPointerSize.x / 100 * interfaceScale,
                _startPointerSize.y / 100 * interfaceScale);
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

        private void TurnOnLamps()
        {
            foreach (var lamp in _lamps)
                lamp.TurnOn();
        }

        public void TestLevel()
        {
            isMove = !isMove;
            TestLevelEvent?.Invoke();
            if (isMove)
            {
                _lamps = FindObjectsOfType<Lamp>().ToList();
                timer.currentTime = 0;
                timer.waitForEndGame = timer.timeForEndGame;
                droneBuilderController.droneRpgController.ResetDroneData();
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
                droneBuilderController.rb.isKinematic = false;
                droneBuilderController.rb.useGravity = true;
                builderUI.createPanel.SetActive(false);
                builderUI.editButtons.SetActive(false);
                if (droneBuilderCheckNode.nodes.Count > 0)
                    builderUI.pathArrow.gameObject.SetActive(true);
                droneBuilderCheckNode.currentNode = 0;
                droneBuilderSoundController.droneFly.Play();
                _selection.Deselect();
                _selection.enabled = false;
            }
            else
            {
                TurnOnLamps();
                builderUI.droneView.SetActive(false);
                freeFlyCamera.enabled = true;
                freeFlyCamera.GetComponent<CinemachineVirtualCamera>().Priority = 10;
                cameraController.isSwitch = false;
                cameraController.SetUpCamerasZeroPriority();
                TrackBuilderUtils.TurnAllConnections(_connections, true);
                droneBuilderController.yaw = _dronePrevRotationY;
                droneBuilderController.transform.position = _dronePrevPosition;
                droneBuilderController.transform.localRotation = Quaternion.Euler(0, _dronePrevRotationY, 0);
                droneBuilderController.rb.isKinematic = true;
                droneBuilderController.rb.useGravity = false;
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

            foreach (var obj in objectsPool)
                obj.SetActive(true);
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

            if (objectsPool.Count > 0)
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
                var yOffset = Convert.ToSingle(kvp.Value[nameof(currentObjectType.yOffset)]);
                var maxMouseDistance = Convert.ToSingle(kvp.Value[nameof(currentObjectType.maxMouseDistance)]);
                var rotSpeed = kvp.Value[nameof(currentObjectType.interactiveObject.windMillRotateSpeed)] != "null"
                    ? Convert.ToSingle(kvp.Value[nameof(currentObjectType.interactiveObject.magnetForce)])
                    : 0f;
                var magnetForce = kvp.Value[nameof(currentObjectType.interactiveObject.magnetForce)] != "null"
                    ? Convert.ToSingle(kvp.Value[nameof(currentObjectType.interactiveObject.magnetForce)])
                    : 0f;
                var pendulumMoveSpeed =
                    kvp.Value[nameof(currentObjectType.interactiveObject.pendulumMoveSpeed)] != "null"
                        ? Convert.ToSingle(kvp.Value[nameof(currentObjectType.interactiveObject.pendulumMoveSpeed)])
                        : 0f;
                var leftPendulumAngle =
                    kvp.Value[nameof(currentObjectType.interactiveObject.leftPendulumAngle)] != "null"
                        ? Convert.ToSingle(kvp.Value[nameof(currentObjectType.interactiveObject.leftPendulumAngle)])
                        : 0f;
                var rightPendulumAngle =
                    kvp.Value[nameof(currentObjectType.interactiveObject.rightPendulumAngle)] != "null"
                        ? Convert.ToSingle(kvp.Value[nameof(currentObjectType.interactiveObject.rightPendulumAngle)])
                        : 0f;
                var windForce = kvp.Value[nameof(currentObjectType.interactiveObject.windForce)] != "null"
                    ? Convert.ToSingle(kvp.Value[nameof(currentObjectType.interactiveObject.windForce)])
                    : 0f;
                var batteryEnergy = kvp.Value[nameof(currentObjectType.interactiveObject.batteryEnergy)] != "null"
                    ? Convert.ToSingle(kvp.Value[nameof(currentObjectType.interactiveObject.batteryEnergy)])
                    : 0f;
                var freezing = kvp.Value["isFreezing"] != "null" &&
                               Convert.ToBoolean(kvp.Value["isFreezing"]);
                var boost = kvp.Value[nameof(currentObjectType.interactiveObject.boostSpeed)] != "null"
                    ? Convert.ToSingle(kvp.Value[nameof(currentObjectType.interactiveObject.boostSpeed)])
                    : 0f;
                var hintText = kvp.Value[nameof(currentObjectType.interactiveObject.hintText)] != "null"
                    ? kvp.Value[nameof(currentObjectType.interactiveObject.hintText)]
                    : "";
                var isLampTurn = kvp.Value[nameof(currentObjectType.interactiveObject.isLampTurn)] != "null" &&
                                 Convert.ToBoolean(kvp.Value[nameof(currentObjectType.interactiveObject.isLampTurn)]);

                var newObj = Instantiate(Resources.Load<GameObject>("TrackObjects/" + objectName), position,
                    Quaternion.Euler(rotation));
                yield return new WaitForSeconds(0.01f);
                TrackBuilderUtils.ChangeLayerRecursively(newObj.transform, layer);
                TrackBuilderUtils.OffOutlineRecursively(newObj.transform);
                newObj.transform.localScale = scale;
                newObj.name = kvp.Value["name"];
                var trackObj = newObj.GetComponent<TrackObject>();
                trackObj.yOffset = yOffset;
                trackObj.maxMouseDistance = maxMouseDistance;

                switch (trackObj.interactiveType)
                {
                    case InteractiveType.Windmill:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<Windmill>();
                        trackObj.interactiveObject.windMillRotateSpeed = rotSpeed;
                        break;
                    case InteractiveType.Magnet:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<RigidbodyMagnet>();
                        trackObj.interactiveObject.magnetForce = magnetForce;
                        break;
                    case InteractiveType.Pendulum:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<Pendulum>();
                        trackObj.interactiveObject.pendulumMoveSpeed = pendulumMoveSpeed;
                        trackObj.interactiveObject.leftPendulumAngle = leftPendulumAngle;
                        trackObj.interactiveObject.rightPendulumAngle = rightPendulumAngle;
                        break;
                    case InteractiveType.Wind:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<WindZoneScript>();
                        trackObj.interactiveObject.windForce = windForce;
                        break;
                    case InteractiveType.Battery:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<Battery>();
                        trackObj.interactiveObject.batteryEnergy = batteryEnergy;
                        break;
                    case InteractiveType.Freezing:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<FreezingBall>();
                        trackObj.interactiveObject.isActive = freezing;
                        break;
                    case InteractiveType.Boost:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<BoostTrigger>();
                        trackObj.interactiveObject.boostSpeed = boost;
                        break;
                    case InteractiveType.Hint:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<Hint>();
                        trackObj.interactiveObject.hintText.text = hintText;
                        break;
                    case InteractiveType.Lamp:
                        trackObj.interactiveObject = trackObj.GetComponentInChildren<Lamp>();
                        trackObj.interactiveObject.isLampTurn = isLampTurn;
                        break;
                    case InteractiveType.Draw:
                        break;
                    case InteractiveType.None:
                        break;
                }

                objectsPool.Add(newObj);
            }

            builderUI.editButtons.SetActive(true);
            builderUI.createPanel.SetActive(true);
            builderUI.loadLevelPanel.SetActive(false);
            audioManager.EndPlay();
            CreateObjectsPoolScene();
            LoadingCompleteEvent?.Invoke();
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
                        TrackBuilderUtils.ChangeLayerRecursively(pendingObj.transform,
                            LayerMask.NameToLayer("TrackGround"));
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

        private void ChangeObjectHeight(float value) => _objectHeightValue = value;

        private void RotateObject(Vector3 axis, float rotateAmount, Space space)
        {
            pendingObject.transform.Rotate(axis, rotateAmount, space);
        }

        public void SelectObject(int index)
        {
            if (pendingObject != null)
                PlaceObjects();

            currentSelectObjectIndex = index;
            pendingObject = Instantiate(objects[index], mousePos, transform.rotation);
            pendingObjects.Add(pendingObject);
            MoveGameObjectToScene(pendingObject, levelScene);
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
            if (obj == null)
                return;

            pendingObject = Instantiate(obj, mousePos, copyObject.transform.rotation);
            pendingObjects.Add(pendingObject);
            TrackBuilderUtils.ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer("Track"));
            TrackBuilderUtils.TurnTrackObjects(pendingObjects, true);
            MoveGameObjectToScene(pendingObject, levelScene);
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
                if (obj.GetComponent<BuilderCheckpointTrigger>())
                    droneBuilderCheckNode.AddNode(obj.transform);
                MoveGameObjectToScene(obj, levelScene);
            }

            if (droneBuilderCheckNode.nodes.Count > 0)
            {
                builderUI.pathArrow.gameObject.SetActive(true);
            }

            FindObjectOfType<Server>().player = droneBuilderController;
        }

        private void MoveGameObjectToScene(GameObject obj, Scene scene)
        {
            SceneManager.MoveGameObjectToScene(obj, scene);
            ObjectChangeSceneEvent?.Invoke();
        }

        private void ClearObject()
        {
            foreach (var obj in objectsPool)
            {
                Destroy(obj);
            }

            RewindManager.Instance.rewindedObjects.Clear();
            objectsPool.Clear();
            pendingObjects.Clear();
        }

        public void TurnUI()
        {
            builderUI.uiPanel.SetActive(false);
            builderUI.droneView.SetActive(false);
        }
    }
}