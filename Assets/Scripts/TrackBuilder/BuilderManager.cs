using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Drone.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

namespace Drone.Builder
{
    public sealed class BuilderManager : MonoBehaviour
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
        public bool isActivWhite;
        public bool isActivRed;
        public bool isActivBlue;
        public bool isActivYellow;
        public bool isActivGreen;
        public BuilderUI builderUI;
        public EditMenu editMenu;
        public BuilderAudioManager builderAudioManager;
        public DroneBuilderController droneBuilderController;
        public DroneBuilderCheckNode droneBuilderCheckNode;
        public DroneBuilderSoundController droneBuilderSoundController;
        public DroneRpgController droneRpgController;
        public UndoRedoManager undoRedoManager;
        public AsyncLoad asyncLoad;
        public Timer timer;
        public CinemachineBrain cameraBrain;
        public LayerMask layerMask;
        public GameObject pendingObject;
        public ObjectsType noScaleEditableObjects;
        public GameObject copyObject;
        public TrackObject currentObjectType;
        public Vector3 mousePos;
        public Transform targetCheckpoint;
        public List<GameObject> pendingObjects = new();
        public List<GameObject> objects;
        public List<GameObject> objectsPool;

        // Systems
        private LoadLevelSystem _loadLevelSystem;
        
        [HideInInspector] public Scene levelScene;
        public event Action LoadingCompleteEvent;
        public event Action TestLevelEvent;
        public event Action StartGame;
        public event Action StopGame;
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
        private Camera _main;

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
            _loadLevelSystem = new LoadLevelSystem();
            for (var i = 0; i < builderUI.createButtons.Count; i++)
            {
                var i1 = i;
                builderUI.createButtons[i].onClick.AddListener(delegate { SelectObject(i1); });
                var trackObject = objects[i].GetComponent<TrackObject>();
                builderUI.objectPreInfos[i].objectName = trackObject.objectName;
                builderUI.objectPreInfos[i].objectDesc = trackObject.objectDescription;
            }

            builderUI.pathArrow.gameObject.SetActive(false);

            _loadLevelSystem.LoadingCompleteEvent += RewindManager.Instance.FindRewindObjects;
            _loadLevelSystem.LoadingCompleteEvent += RewindManager.Instance.RestartTracking;
            _loadLevelSystem.LoadingCompleteEvent += OnLoadingComplete;
            _loadLevelSystem.ObjectsCreatedEvent += GetCreatedObjects;

            if (isLoadLevel)
            {
                LoadLevel();
            }
            else if (isGameLevel)
            {
                _loadLevelSystem.LoadingCompleteEvent += TestLevel;
                StartLevel();
            }
            else
            {
                CreateObjectsPoolScene();
            }
            
            _main = Camera.main;
        }

        private void OnLoadingComplete() => LoadingCompleteEvent?.Invoke();

        private void GetCreatedObjects(List<GameObject> obj)
        {
            objectsPool = new List<GameObject>(obj);
            
            CreateObjectsPoolScene();
        }

        private void StartLevel()
        {
            if (objectsPool.Count > 0)
                ClearObject();
            
            StartCoroutine(_loadLevelSystem.LoadScene(levelName));
        }

        private void LoadLevel()
        {
            if (objectsPool.Count > 0)
                ClearObject();
            
            StartCoroutine(_loadLevelSystem.LoadScene(levelName));
        }

        private void OnEnable()
        {
            StartGame += TurnPlayerActionMap;
            StopGame += TurnBuilderActionMap;
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
            _loadLevelSystem.LoadingCompleteEvent -= RewindManager.Instance.FindRewindObjects;
            _loadLevelSystem.LoadingCompleteEvent -= RewindManager.Instance.RestartTracking;
            _loadLevelSystem.LoadingCompleteEvent -= OnLoadingComplete;
            _loadLevelSystem.LoadingCompleteEvent -= TestLevel;
            StartGame -= TurnPlayerActionMap;
            StopGame -= TurnBuilderActionMap;
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

        private void TurnPlayerActionMap() => InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
        private void TurnBuilderActionMap() => InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Builder);
        
        private void OpenExitPanel()
        {
            _isExitPanel = !_isExitPanel;
            builderUI.exitBuilderPanel.SetActive(_isExitPanel);
            builderUI.createPanel.SetActive(!_isExitPanel);
            builderUI.editButtons.SetActive(!_isExitPanel);
            builderUI.objectEditPanel.SetActive(!_isExitPanel);
        }

        private void RotateXObject(float value)
        {
            if (IsNoEditObject())
                return;

            switch (value)
            {
                case > 0:
                    RotateObject(pendingObject.transform.right, 10f, Space.World);
                    break;
                case < 0:
                    RotateObject(pendingObject.transform.right, -10f, Space.World);
                    break;
            }
        }

        private void ChangeObjectScale(float value)
        {
            if (IsNoEditObject() || noScaleEditableObjects.HasFlag(currentObjectType.objectType))
                return;

            var angleX = currentObjectType.Rotation.eulerAngles.x;
            var angleY = currentObjectType.Rotation.eulerAngles.y;
            var angleZ = currentObjectType.Rotation.eulerAngles.z;
            editMenu.SetEditPanelParams(currentObjectType.objectName,
                currentObjectType.Position.x, currentObjectType.Position.y, currentObjectType.Position.z,
                angleX, angleY, angleZ,
                currentObjectType.Scale.x + value * 0.5f, currentObjectType);
        }

        private void RotateYObject(float value)
        {
            if (IsNoEditObject())
                return;

            switch (value)
            {
                case > 0:
                    RotateObject(pendingObject.transform.up, 10, Space.World);
                    break;
                case < 0:
                    RotateObject(pendingObject.transform.up, -10, Space.World);
                    break;
            }
        }

        private bool IsNoEditObject() => currentObjectType == null || pendingObject == null || pendingObjects.Count > 1;

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
            if (isMove)
            {
                SetDroneParameters();
            }

            var ray = _main!.ScreenPointToRay(Mouse.current.position.ReadValue());

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
                    droneBuilderSoundController.droneFlySound.Stop();
                else
                    droneBuilderSoundController.droneFlySound.Play();
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
                StartGame?.Invoke();
                _lamps = FindObjectsOfType<Lamp>().ToList();
                var outlines = FindObjectsOfType<Outline>();
                _connections = FindObjectsOfType<Connection>();
                TrackBuilderUtils.TurnAllOutlineEffects(outlines, false);
                TrackBuilderUtils.TurnAllConnections(_connections, false);
                _dronePrevRotationY = droneBuilderController.transform.localRotation.eulerAngles.y;
                droneBuilderController.yaw = _dronePrevRotationY;
                _dronePrevPosition = droneBuilderController.transform.position;
                cameraBrain.transform.SetParent(droneBuilderController.transform);
                if (droneBuilderCheckNode.nodes.Count > 0)
                    builderUI.pathArrow.gameObject.SetActive(true);
                droneBuilderCheckNode.currentNode = 0;
            }
            else
            {
                StopGame?.Invoke();
                TurnOnLamps();
                TrackBuilderUtils.TurnAllConnections(_connections, true);
                droneBuilderController.yaw = _dronePrevRotationY;
                droneBuilderController.transform.position = _dronePrevPosition;
                droneBuilderController.transform.localRotation = Quaternion.Euler(0, _dronePrevRotationY, 0);
                _isTabPanel = false;
            }

            foreach (var obj in objectsPool)
                obj.SetActive(true);
        }
        
        private void PlaceObjects()
        {
            try
            {
                if (pendingObjects.Count > 1)
                {
                    pendingObjects = new List<GameObject>(_selection.selectedObjects);
                    foreach (var pendingObj in pendingObjects)
                    {
                        TrackBuilderUtils.ChangeLayerRecursively(pendingObj.transform,
                            LayerMask.NameToLayer(Idents.Layers.TrackGround));
                    }
                }
                else
                {
                    pendingObject = _selection.selectedObject;
                    currentObjectType = _selection.selectedObject.GetComponent<TrackObject>();
                    TrackBuilderUtils.ChangeLayerRecursively(pendingObject.transform,
                        LayerMask.NameToLayer(Idents.Layers.TrackGround));
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

        private void RotateObject(Vector3 axis, float rotateAmount, Space space) =>pendingObject.transform.Rotate(axis * rotateAmount, space);

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

        private void MoveObjectsToPoolScene()
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
            RewindManager.Instance.rewindedObjects.Clear();
            pendingObjects.Clear();
            
            foreach (var obj in objectsPool)
            {
                Destroy(obj);
            }
            
            objectsPool.Clear();
        }

        public void TurnUI()
        {
            builderUI.uiPanel.SetActive(false);
            builderUI.droneView.SetActive(false);
        }

        public void MoveObject(GameObject selectedObject, List<GameObject> selectedObjects)
        {
            pendingObject = selectedObject;
            pendingObjects = new List<GameObject>(selectedObjects);
            currentObjectType = selectedObject.GetComponentInParent<TrackObject>();
            currentObjectType.isActive = true;
        }

        public void DeleteObject()
        {
            pendingObjects.Clear();
            pendingObject = null;
        }
    }
}