using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cinemachine;
using Drone.RuntimeHandle;
using Drone.RuntimeHandle.Handles;
using Drone.Sockets;
using Drone.TrackBuilder.EditButtons;
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

        [Header("Builder tools")]
        public Transform objectOrigin;
        public float interfaceScale;
        public float currentYawSensitivity;
        public int currentSelectObjectIndex;
        public bool isSimpleMode;
        public bool isFastDownMode;
        public bool isMove;
        public bool isGameMode;
        public bool isLoadLevel;
        public bool isGameLevel;
        public bool isActivWhite;
        public bool isActivRed;
        public bool isActivBlue;
        public bool isActivYellow;
        public bool isActivGreen;
        
        [Header("UI and Components")]
        public BuilderUI builderUI;
        [SerializeField] private Selection selection;
        [SerializeField] private EditMenu editMenu;
        public ActionType objectActionType;
        public RuntimeTransformHandle runtimeTransformHandle;
        public BuilderAudioManager builderAudioManager;
        public DroneBuilderController droneBuilderController;
        public DroneBuilderCheckNode droneBuilderCheckNode;
        public DroneBuilderSoundController droneBuilderSoundController;
        public DroneRpgController droneRpgController;
        public UndoRedoManager undoRedoManager;
        public AsyncLoad asyncLoad;
        public Timer timer;
        public GameData gameData;
        public CinemachineBrain cameraBrain;
        public LayerMask trackObjectLayerMask;
        
        [Header("Game Objects")]
        public GameObject pendingObject;
        public ObjectsType noScaleEditableObjects;
        public GameObject copyObject;
        public TrackObject currentObjectType;
        public Vector3 mousePos;
        public List<ObjectAction> editButtonsAction;
        public List<GameObject> pendingObjects = new();
        public List<GameObject> objects;
        public List<GameObject> objectsPool;

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
        private Vector3 _mainCameraPrevPosition;
        private Vector3 _startPointerSize;
        private Quaternion _mainCameraPrevRotation;
        private float _dronePrevRotationY;
        private Vector3 _dronePrevPosition;
        private Vector2 _prevMousePos;
        private float _objectHeightValue;

        private bool _isBuilderExitTab;
        private bool _isGameExitTab;
        private bool _isEditorExitTab;
        private bool _isFreeMoveBefore;

        private void Awake()
        {
            Instance = this;
            _startPointerSize = builderUI.pathArrow.sizeDelta;
        }

        private void Start()
        {
            selection.Select(droneBuilderController.gameObject);
            selection.Deselect();
            
            _loadLevelSystem = new LoadLevelSystem(this);
            for (var i = 0; i < builderUI.createButtons.Count; i++)
            {
                var i1 = i;
                builderUI.createButtons[i].onClick.AddListener(delegate { SelectObject(i1); });
                var trackObject = objects[i].GetComponent<TrackObject>();
                builderUI.objectPreInfos[i].objectName = trackObject.objectName;
                builderUI.objectPreInfos[i].objectDesc = trackObject.objectDescription;
            }

            foreach (var ed in builderUI.editButtonsList)
            {
                if (ed.GetComponent<SelectAction>())
                    continue;
                var color = ed.color;
                color.a = 0.5f;
                ed.color = color;
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
                LoadLevel();
            }
            else
            {
                builderUI.loadLevelPanel.SetActive(false);
                CreateObjectsPoolScene();
            }
        }

        private void OnLoadingComplete() => LoadingCompleteEvent?.Invoke();

        private void GetCreatedObjects(List<GameObject> obj)
        {
            objectsPool = new List<GameObject>(obj);
            
            CreateObjectsPoolScene();
        }

        private void LoadLevel()
        {
            if (objectsPool.Count > 0)
                ClearObject();

            StartCoroutine(_loadLevelSystem.LoadScene(gameData.currentLevel));
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
            InputManager.Instance.ChangeObjectHeightEvent += ChangeObjectHeight;
            InputManager.Instance.ChangeObjectScaleEvent += ChangeObjectScale;
            InputManager.Instance.ExitGameEvent += CheckTabPanel;
            InputManager.Instance.ExitBuilderEvent += OpenExitPanel;
            InputManager.Instance.SelectObjectEvent += SetOrigin;
            InputManager.Instance.FreeMoveEvent += FreeMove;
            InputManager.Instance.SwitchToLocalGlobal += OnSwitchLocalGlobal;

            InputManager.Instance.SelectEvent += OnSetSelect;
            InputManager.Instance.PositionEvent += OnSetPosition;
            InputManager.Instance.RotationEvent += OnSetRotation;
            InputManager.Instance.ScaleEvent += OnSetScale;
            
            foreach (var edit in editButtonsAction) edit.EventAction += EditButtonsHandler;
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
            InputManager.Instance.ChangeObjectHeightEvent -= ChangeObjectHeight;
            InputManager.Instance.ChangeObjectScaleEvent -= ChangeObjectScale;
            InputManager.Instance.ExitGameEvent -= CheckTabPanel;
            InputManager.Instance.ExitBuilderEvent -= OpenExitPanel;
            InputManager.Instance.SelectObjectEvent -= SetOrigin;
            InputManager.Instance.FreeMoveEvent -= FreeMove;
            InputManager.Instance.SwitchToLocalGlobal -= OnSwitchLocalGlobal;
            
            InputManager.Instance.SelectEvent -= OnSetSelect;
            InputManager.Instance.PositionEvent -= OnSetPosition;
            InputManager.Instance.RotationEvent -= OnSetRotation;
            InputManager.Instance.ScaleEvent -= OnSetScale;
            
            foreach (var edit in editButtonsAction) edit.EventAction -= EditButtonsHandler;
        }
        
        private void OnSwitchLocalGlobal()
        {
            if (runtimeTransformHandle.space == HandleSpace.LOCAL)
            {
                runtimeTransformHandle.space = HandleSpace.WORLD;
                builderUI.spaceText.text = "world";
            }
            else
            {
                runtimeTransformHandle.space = HandleSpace.LOCAL;
                builderUI.spaceText.text = "local";
            }
        }
        
        private void OnSetSelect()
        {
            EditButtonsHandler(editButtonsAction.FirstOrDefault(x => x is SelectAction));
        }
        
        private void OnSetPosition()
        {
            EditButtonsHandler(editButtonsAction.FirstOrDefault(x => x is MoveAction));
        }
        
        private void OnSetRotation()
        {
            EditButtonsHandler(editButtonsAction.FirstOrDefault(x => x is RotateAction));
        }
        
        private void OnSetScale()
        {
            EditButtonsHandler(editButtonsAction.FirstOrDefault(x => x is ScaleAction));
        }
        
        private void FreeMove()
        {
            if(selection.selectedObject == null) return;
            
            if (runtimeTransformHandle.gameObject.activeSelf || !_isFreeMoveBefore)
            {
                runtimeTransformHandle.gameObject.SetActive(false);
                objectActionType = ActionType.FreeMove;
                selection.Move();
                SetAlphaEditButton((editButtonsAction.FirstOrDefault(x => x is MoveAction) as MoveAction)?.image);
                runtimeTransformHandle.type = HandleType.POSITION;
            }
            else
            {
                EditButtonsHandler(editButtonsAction.FirstOrDefault(x => x is MoveAction));
            }
            _isFreeMoveBefore = !_isFreeMoveBefore;
        }

        public void EditButtonsHandler(ObjectAction objectAction)
        {
            switch (objectAction)
            {
                case SelectAction selectAction:
                    objectActionType = ActionType.Select;
                    SetAlphaEditButton(selectAction.image);
                    runtimeTransformHandle.gameObject.SetActive(false);
                    _isFreeMoveBefore = false;
                    break;
                case SetOriginAction setOriginAction:
                    objectActionType = ActionType.SetOrigin;
                    SetAlphaEditButton(setOriginAction.image);
                    runtimeTransformHandle.gameObject.SetActive(false);
                    break;
                case MoveAction moveAction:
                    if(selection.selectedObject == null) return;
                    objectActionType = ActionType.Move;
                    selection.Move();
                    SetAlphaEditButton(moveAction.image);
                    runtimeTransformHandle.gameObject.SetActive(true);
                    runtimeTransformHandle.type = HandleType.POSITION;
                    TrackBuilderUtils.ChangeLayerRecursively(runtimeTransformHandle.transform, LayerMask.NameToLayer(Idents.Layers.TrackGround));
                    break;
                case RotateAction rotateAction:
                    if(selection.selectedObject == null) return;
                    objectActionType = ActionType.Rotate;
                    SetAlphaEditButton(rotateAction.image);
                    runtimeTransformHandle.gameObject.SetActive(true);
                    runtimeTransformHandle.type = HandleType.ROTATION;
                    TrackBuilderUtils.ChangeLayerRecursively(runtimeTransformHandle.transform, LayerMask.NameToLayer(Idents.Layers.TrackGround));
                    break;
                case ScaleAction scaleAction:
                    if(selection.selectedObject.GetComponent<DroneController>() || selection.selectedObject == null) return;
                    objectActionType = ActionType.Scale;
                    SetAlphaEditButton(scaleAction.image);
                    runtimeTransformHandle.gameObject.SetActive(true);
                    runtimeTransformHandle.type = HandleType.SCALE;
                    TrackBuilderUtils.ChangeLayerRecursively(runtimeTransformHandle.transform, LayerMask.NameToLayer(Idents.Layers.TrackGround));
                    break;
                case CopyAction copyAction:
                    if(selection.selectedObject == null) return;
                    objectActionType = ActionType.Copy;
                    CopyObject();
                    SetAlphaEditButton(copyAction.image);
                    runtimeTransformHandle.gameObject.SetActive(false);
                    break;
                case PasteAction pasteAction:
                    if(copyObject == null) return;
                    objectActionType = ActionType.Paste;
                    PasteObject();
                    SetAlphaEditButton(pasteAction.image);
                    runtimeTransformHandle.gameObject.SetActive(false);
                    break;
            }
        }

        private void SetAlphaEditButton(Image image)
        {
            foreach (var ed in builderUI.editButtonsList)
            {
                if (ed == image)
                {
                    var color = ed.color;
                    color.a = 1f;
                    ed.color = color;
                }
                else
                {
                    var color = ed.color;
                    color.a = 0.5f;
                    ed.color = color;
                }
            }
        }

        private void TurnPlayerActionMap() => InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
        private void TurnBuilderActionMap() => InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Builder);
        
        private void OpenExitPanel()
        {
            if (!builderUI.loadLevelPanel.activeInHierarchy)
            {
                _isExitPanel = !_isExitPanel;
                builderUI.exitBuilderPanel.SetActive(_isExitPanel);
                builderUI.createPanel.SetActive(!_isExitPanel);
                builderUI.editButtons.SetActive(!_isExitPanel);
                if (_isExitPanel)
                {
                    builderUI.objectEditPanel.SetActive(false);
                }
                else
                {
                    builderUI.objectEditPanel.SetActive(selection.selectedObject);
                }

                if (builderUI.exitTabPanel.activeSelf && !_isExitPanel)
                    builderUI.exitTabPanel.SetActive(false);
                if (builderUI.restartTabPanel.activeSelf && !_isExitPanel)
                    builderUI.restartTabPanel.SetActive(false);
            }
        }

        private void ChangeObjectScale(float value)
        {
            if (IsNoEditObject() || noScaleEditableObjects.HasFlag(currentObjectType.objectType) || objectActionType is not ActionType.Scale)
                return;

            editMenu.SetEditPanelParams(currentObjectType, currentObjectType.Scale.x + value * 0.5f);
        }

        private void RotateYObject(float value)
        {
            if (IsNoEditObject() || objectActionType is not ActionType.Rotate)
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
            
            editMenu.UpdateRotationsView(selection.selectedTrackObject);
        }

        private bool IsNoEditObject() => currentObjectType == null || pendingObject == null || pendingObjects.Count > 1;

        private void PlaceAndPickupObject()
        {
            if (selection.selectedObjects.Count > 0 && selection.selectedObject != null)
            {
                PlaceObjects();
                SelectObject(currentSelectObjectIndex);
            }
        }

        private void PlaceObject()
        {
            if (selection.selectedObjects.Count > 0 && selection.selectedObject != null)
            {
                PlaceObjects();
                selection.Deselect();
            }
        }

        private void Update()
        {
            if (isMove)
            {
                SetDroneParameters();
            }

            if (objectActionType is ActionType.FreeMove)
            {
                var ray = cameraBrain.OutputCamera!.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out _hit, 10000, trackObjectLayerMask, QueryTriggerInteraction.Ignore) &&
                    !EventSystem.current.IsPointerOverGameObject())
                {
                    mousePos = _hit.point;

                    if (pendingObjects.Count == 0 || pendingObject == null)
                        return;

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
                editMenu.UpdatePositionsView(selection.selectedTrackObject);
            }
        }

        private void SetOrigin()
        {
            if (objectActionType is not ActionType.SetOrigin) return;
            
            var ray = cameraBrain.OutputCamera!.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out _hit, 10000, trackObjectLayerMask, QueryTriggerInteraction.Ignore) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                objectOrigin.position = _hit.point;
            }
        }

        private void UpdateObjectHeight()
        {
            if (IsNoEditObject())
                return;

            currentObjectType.yOffset += _objectHeightValue * 2 * Time.deltaTime;
        }

        private void UndoCommand() => undoRedoManager.UndoCommand();
        private void RedoCommand() => undoRedoManager.RedoCommand();

        public void PasteObject()
        {
            if (pendingObject != null || pendingObjects.Count > 0)
                PlaceObjects();

            if (copyObject != null)
            {
                PasteObject(copyObject);
            }
        }

        public void CopyObject()
        {
            if (selection.selectedObject != null)
            {
                copyObject = selection.selectedObject;
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

            foreach (var obj in objectsList) center += obj.transform.position;

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
                builderUI.droneView.SetActive(!_isTabPanel);
                if(builderUI.exitTabPanel.activeSelf && !_isTabPanel)
                    builderUI.exitTabPanel.SetActive(false);
                if(builderUI.restartTabPanel.activeSelf && !_isTabPanel)
                    builderUI.restartTabPanel.SetActive(false);
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

            Pointer.PointerPosition(builderUI, droneBuilderCheckNode, cameraBrain, _startPointerSize, interfaceScale);
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
                objectOrigin.gameObject.SetActive(false);
                _lamps = FindObjectsOfType<Lamp>().ToList();
                var outlines = FindObjectsOfType<Outline>();
                _connections = FindObjectsOfType<Connection>();
                TrackBuilderUtils.TurnAllOutlineEffects(outlines, false);
                TrackBuilderUtils.TurnAllConnections(_connections, false);
                _dronePrevRotationY = droneBuilderController.transform.localRotation.eulerAngles.y;
                droneBuilderController.yaw = _dronePrevRotationY;
                _dronePrevPosition = droneBuilderController.transform.position;
                if (droneBuilderCheckNode.nodes.Count > 0)
                    builderUI.pathArrow.gameObject.SetActive(true);
                droneBuilderCheckNode.currentNode = 0;
            }
            else
            {
                StopGame?.Invoke();
                objectOrigin.gameObject.SetActive(true);
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
                    pendingObjects = new List<GameObject>(selection.selectedObjects);
                    foreach (var pendingObj in pendingObjects)
                    {
                        TrackBuilderUtils.ChangeLayerRecursively(pendingObj.transform,
                            LayerMask.NameToLayer(Idents.Layers.TrackGround));
                    }
                }
                else
                {
                    pendingObject = selection.selectedObject;
                    currentObjectType = selection.selectedObject.GetComponent<TrackObject>();
                    TrackBuilderUtils.ChangeLayerRecursively(pendingObject.transform,
                        LayerMask.NameToLayer(Idents.Layers.TrackGround));
                    undoRedoManager.ExecuteCommand(new PlaceCommand(objects[currentSelectObjectIndex],
                        currentObjectType.Position, currentObjectType.Scale, currentObjectType.Rotation, pendingObject,
                        currentObjectType.yOffset));
                }

                EditButtonsHandler(editButtonsAction.FirstOrDefault(x => x is SelectAction));
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
                TrackBuilderUtils.ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer(Idents.Layers.TrackGround));
                currentObjectType = null;
                pendingObject = null;
            }
            catch
            {
                // ignored
            }
        }

        private void ChangeObjectHeight(float value) => _objectHeightValue = value;

        private void RotateObject(Vector3 axis, float rotateAmount, Space space) => pendingObject.transform.Rotate(axis * rotateAmount, space);

        public void SelectObject(int index)
        {
            if (pendingObject != null)
                PlaceObjects();

            currentSelectObjectIndex = index;
            var newObj = Instantiate(objects[index], objectOrigin.position, transform.rotation);
            // pendingObject = Instantiate(objects[index], objectOrigin.position, transform.rotation);
            // pendingObjects.Add(pendingObject);
            TrackBuilderUtils.ChangeLayerRecursively(newObj.transform,
                LayerMask.NameToLayer(Idents.Layers.TrackGround));
            MoveGameObjectToScene(newObj, levelScene);
            objectsPool.Add(newObj);
            selection.Deselect();
            selection.Select(newObj);
            currentObjectType = newObj.GetComponent<TrackObject>();
            currentObjectType.isActive = true;

            if (currentObjectType.objectType == ObjectsType.Gate)
            {
                currentObjectType.GetComponent<BuilderCheckpointTrigger>().checkpointId =
                    droneBuilderCheckNode.nodes.Count;
                droneBuilderCheckNode.AddNode(newObj.transform);
            }
        }

        private void PasteObject(GameObject obj)
        {
            if (obj == null)
                return;

            objectActionType = ActionType.Select;
            pendingObject = Instantiate(obj, objectOrigin.position, copyObject.transform.rotation);
            pendingObjects.Add(pendingObject);
            TrackBuilderUtils.ChangeLayerRecursively(pendingObject.transform, LayerMask.NameToLayer(Idents.Layers.Track));
            TrackBuilderUtils.TurnTrackObjects(pendingObjects, true);
            MoveGameObjectToScene(pendingObject, levelScene);
            objectsPool.Add(pendingObject);
            selection.Deselect();
            selection.Select(pendingObject);
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

        public void OnSetTargetSpeed(float value)
        {
            droneBuilderController.targetSpeed = value;

            builderUI.targetSpeedSlider1.value = value;
            builderUI.targetSpeedSlider2.value = value;
            builderUI.targetSpeedSlider3.value = value;
            
            builderUI.targetText1Value.text = value.ToString("f1", CultureInfo.InvariantCulture);
            builderUI.targetText2Value.text = value.ToString("f1", CultureInfo.InvariantCulture);
            builderUI.targetText3Value.text = value.ToString("f1", CultureInfo.InvariantCulture);
        }
        
        public void OpenExitTabPanel()
        {
            builderUI.exitTabPanel.SetActive(true);
            _isBuilderExitTab = builderUI.exitBuilderPanel.activeSelf;
            _isGameExitTab = builderUI.gameTabPanel.activeSelf;
            _isEditorExitTab = builderUI.editorTabPanel.activeSelf;
            builderUI.editorTabPanel.SetActive(false);
            builderUI.gameTabPanel.SetActive(false);
            builderUI.exitBuilderPanel.SetActive(false);
        }

        public void OpenRestartTabPanel()
        {
            builderUI.restartTabPanel.SetActive(true);
            _isBuilderExitTab = builderUI.exitBuilderPanel.activeSelf;
            _isGameExitTab = builderUI.gameTabPanel.activeSelf;
            _isEditorExitTab = builderUI.editorTabPanel.activeSelf;
            builderUI.editorTabPanel.SetActive(false);
            builderUI.gameTabPanel.SetActive(false);
            builderUI.exitBuilderPanel.SetActive(false);
        }

        public void CloseExitTabPanel()
        {
            builderUI.exitTabPanel.SetActive(false);
            if (_isBuilderExitTab)
            {
                builderUI.exitBuilderPanel.SetActive(true);
                _isBuilderExitTab = false;
            }
            else if (_isGameExitTab)
            {
                builderUI.gameTabPanel.SetActive(true);
                _isGameExitTab = false;
            }
            else if (_isEditorExitTab)
            {
                builderUI.editorTabPanel.SetActive(true);
                _isEditorExitTab = false;
            }
        }
        
        public void CloseRestartTabPanel()
        {
            builderUI.restartTabPanel.SetActive(false);
            if (_isBuilderExitTab)
            {
                builderUI.exitBuilderPanel.SetActive(true);
                _isBuilderExitTab = false;
            }
            else if (_isGameExitTab)
            {
                builderUI.gameTabPanel.SetActive(true);
                _isGameExitTab = false;
            }
            else if (_isEditorExitTab)
            {
                builderUI.editorTabPanel.SetActive(true);
                _isEditorExitTab = false;
            }
        }

        public void RestartLevel()
        {
            droneRpgController.IsReset = true;
            droneRpgController.TimeForEndGame = gameData.builderTimeInSeconds;
            
            GameManager.Instance.gameData.isLoadLevel = false;
            GameManager.Instance.gameData.isStartBuilder = true;
            GameManager.Instance.gameData.isTeleportLevel = false;
            SceneManager.LoadScene(4);
        }
    }
}