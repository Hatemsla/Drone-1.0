﻿using Drone.Builder;
using Drone.DB;
using Drone.DroneFootball;
using Drone.DroneRace;
using Drone.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Drone
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameData gameData;
        public DroneData droneData;
        public DBManager dbManager;
        public Server server;
        public ScratchClient scratchClient;
        public RaceController raceController;
        public FootballController footballController;
        public BuilderManager builderManager;
        public AsyncLoad asyncLoad;
        private static readonly int ShowPanel = Animator.StringToHash("ShowPanel");

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            droneData.Reset();
            gameData.Reset();
            
            dbManager = GetComponent<DBManager>();
            server = GetComponent<Server>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (scene.buildIndex)
            {
                case 1:
                {
                    var gameManagers = FindObjectsOfType<GameManager>();

                    foreach (var gameManager in gameManagers)
                        if(gameManager != this)
                            Destroy(gameManager.gameObject);

                    asyncLoad = FindObjectOfType<AsyncLoad>();
                    InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Ui);
                    
                    droneData.Reset();
                    break;
                }
                case 2:
                    raceController = FindObjectOfType<RaceController>();
                    raceController.raceUIManager.backBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, raceController.raceUIManager.uiPanel,
                            raceController.raceUIManager.loadPanel);
                    });
                    raceController.raceUIManager.confirmExitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    raceController.isSimpleMode = gameData.isSimpleMode;
                    raceController.isFastDownMode = gameData.isFastDownMode;
                    raceController.droneRaceController.yawPower = gameData.currentYawSensitivity;
                    server.player = raceController.droneRaceController;
                    raceController.currentAIDroneSpeed = gameData.currentAIDroneSpeed;
                    raceController.timer.timeForEndGame = gameData.raceTimeInSeconds;

                    asyncLoad = raceController.asyncLoad;

                    InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
                    break;
                case 3:
                    footballController = FindObjectOfType<FootballController>();
                    footballController.footballUIManager.backBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, footballController.footballUIManager.uiPanel,
                            footballController.footballUIManager.loadPanel);
                    });
                    footballController.footballUIManager.confirmExitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    footballController.isSimpleMode = gameData.isSimpleMode;
                    footballController.isFastDownMode = gameData.isFastDownMode;
                    footballController.droneFootballController.yawPower = gameData.currentYawSensitivity;
                    server.player = footballController.droneFootballController;
                    footballController.currentGateScale = gameData.currentGateScale;
                    footballController.currentAIDroneSpeed = gameData.currentAIDroneSpeed;
                    footballController.timer.timeForEndGame = gameData.footballTimeInSeconds;

                    asyncLoad = footballController.asyncLoad;
                    
                    InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
                    break;
                case 4:
                {
                    builderManager = FindObjectOfType<BuilderManager>();
                    builderManager.currentYawSensitivity = gameData.currentYawSensitivity;
                    builderManager.builderUI.confirmExitBtn.onClick.AddListener(GameManagerUtils.Exit);
                    builderManager.isSimpleMode = gameData.isSimpleMode;
                    builderManager.isFastDownMode = gameData.isFastDownMode;
                    builderManager.builderUI.backEditorTabBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel,
                            builderManager.builderUI.loadPanel);
                    });
                    builderManager.builderUI.backGameTabBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel,
                            builderManager.builderUI.loadPanel);
                    });
                    builderManager.builderUI.backBuilderTabBtn.onClick.AddListener(delegate
                    {
                        GameManagerUtils.BackToMenu(asyncLoad, builderManager.builderUI.uiPanel,
                            builderManager.builderUI.loadPanel);
                    });
                    builderManager.builderUI.saveBtn.onClick.AddListener(SaveLevel);
                    builderManager.droneBuilderController.isSimpleMode = gameData.isSimpleMode;
                    server.player = builderManager.droneBuilderController;
                    asyncLoad = builderManager.asyncLoad;
                    builderManager.timer.timeForEndGame = gameData.builderTimeInSeconds;
                    
                    if(!gameData.isTeleportLevel)
                        builderManager.gameData.currentLevel = gameData.originLevel;

                    if (gameData.isLoadLevel)
                    {
                        builderManager.isGameMode = false;
                        builderManager.isLoadLevel = gameData.isLoadLevel;
                    }

                    if (gameData.isStartBuilder)
                    {
                        builderManager.isGameMode = true;
                        builderManager.isGameLevel = true;
                        InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Player);
                        return;
                    }
                    
                    InputManager.Instance.TurnCustomActionMap(Idents.ActionMaps.Builder);
                    break;
                }
            }
        }

        private void SaveLevel()
        {
            LevelManager.SaveLevel(builderManager, gameData.originLevel);
            builderManager.builderUI.savePanelAnimator.SetTrigger(ShowPanel);
        }

        public void GetScratchData(ScratchData droneData)
        {
            switch (droneData.Mode)
            {
                case "AltHold":
                    SetAltholdMode(droneData);
                    break;
                case "Angle":
                    SetAngleMode(droneData);
                    break;
                case "Ручной":
                    break;
            }
        }
        
        private void SetAngleMode(ScratchData droneData)
        {
            if (footballController)
            {
                footballController.droneFootballController.isSimpleMode = false;
                footballController.droneFootballController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                footballController.droneFootballController.pedals = droneData.Yaw / 100;
                footballController.droneFootballController.throttle = droneData.Angle / 100;
                return;
            }

            if (raceController)
            {
                raceController.droneRaceController.isSimpleMode = false;
                raceController.droneRaceController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                raceController.droneRaceController.pedals = droneData.Yaw / 100;
                raceController.droneRaceController.throttle = droneData.Angle / 100;
                return;
            }

            if (builderManager)
            {
                builderManager.droneBuilderController.isSimpleMode = false;
                builderManager.droneBuilderController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                builderManager.droneBuilderController.pedals = droneData.Yaw / 100;
                builderManager.droneBuilderController.throttle = droneData.Angle / 100;
            }
        }

        private void SetAltholdMode(ScratchData droneData)
        {
            if (footballController)
            {
                footballController.droneFootballController.isSimpleMode = true;
                footballController.droneFootballController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                footballController.droneFootballController.pedals = droneData.Yaw / 100;
                footballController.droneFootballController.throttle = droneData.Althold / 100;
                return;
            }

            if (raceController)
            {
                raceController.droneRaceController.isSimpleMode = true;
                raceController.droneRaceController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                raceController.droneRaceController.pedals = droneData.Yaw / 100;
                raceController.droneRaceController.throttle = droneData.Althold / 100;
                return;
            }

            if (builderManager)
            {
                builderManager.droneBuilderController.isSimpleMode = true;
                builderManager.droneBuilderController.cyclic =
                    new Vector2(droneData.Roll / 100, droneData.Pitch / 100);
                builderManager.droneBuilderController.pedals = droneData.Yaw / 100;
                builderManager.droneBuilderController.throttle = droneData.Althold / 100;
            }
        }
    }
}