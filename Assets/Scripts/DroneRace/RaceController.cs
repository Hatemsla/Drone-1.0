using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DB;
using DroneFootball;
using UnityEngine;
using UnityEngine.UI;

namespace DroneRace
{
    public class RaceController : MonoBehaviour
    {
        [SerializeField] private Transform targetCheckpoint;
        [SerializeField] private CheckNode checkNode;
        [SerializeField] private DroneRaceAI droneRaceAI;
        [SerializeField] private CheckNode playerNode;
        [SerializeField] private RacePath racePath;
        public DroneController droneRaceController;
        public DroneRaceUIManager raceUIManager;
        public DroneRaceAudioController droneRaceAudioController;
        public float interfaceScale;
        public float currentAIDroneSpeed;
        public int clipIndex;
        public bool isSimpleMode;
        public bool isGameStart;
        public Timer timer;
        public AsyncLoad asyncLoad;
        
        private List<CheckNode> _droneRaceCheckNodes;
        private CinemachineBrain _mainCamera;
        private Vector3 _startPointerSize;
        private int _playerRacePosition;
        private bool _isResult;
        private bool _isTabPanel;

        private void Awake()
        {
            _startPointerSize = raceUIManager.pathArrow.sizeDelta;
            _mainCamera = FindObjectOfType<CinemachineBrain>();
        }

        private void Start()
        {
            droneRaceController.isSimpleMode = isSimpleMode;
            checkNode = droneRaceController.GetComponent<CheckNode>();
            _droneRaceCheckNodes = FindObjectsOfType<CheckNode>().ToList();
            foreach (var drone in _droneRaceCheckNodes)
                racePath.nodes[0].localScale = new Vector3(3, 3, 3);

            droneRaceAI.speed *= currentAIDroneSpeed;
        }

        private void Update()
        {
            CheckStartGame();
            CheckEndGame();
            CheckTabPanel();
        }

        private void LateUpdate()
        {
            targetCheckpoint = checkNode.nodes[checkNode.currentNode];
            Vector3 realPos = _mainCamera.OutputCamera.WorldToScreenPoint(targetCheckpoint.position);
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            Vector3 outPos = realPos;
            float direction = 1;

            raceUIManager.pathArrow.GetComponent<Image>().sprite = raceUIManager.outOfScreenIcon;

            if (!IsBehind(targetCheckpoint.position)) // если цель спереди
            {
                if (rect.Contains(realPos)) // и если цель в окне экрана
                {
                    raceUIManager.pathArrow.GetComponent<Image>().sprite = raceUIManager.pointerIcon;
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
            float offset = raceUIManager.pathArrow.sizeDelta.x / 2;
            outPos.x = Mathf.Clamp(outPos.x, offset, Screen.width - offset);
            outPos.y = Mathf.Clamp(outPos.y, offset, Screen.height - offset);

            Vector3 pos = realPos - outPos; // направление к цели из PointerUI 

            RotatePointer(direction * pos);

            raceUIManager.pathArrow.sizeDelta = new Vector2(_startPointerSize.x / 100 * interfaceScale, _startPointerSize.y / 100 * interfaceScale);
            raceUIManager.pathArrow.position = outPos;
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
            raceUIManager.pathArrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void CheckTabPanel()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isTabPanel = !_isTabPanel;
                if(_isTabPanel)
                    droneRaceAudioController.droneFly.Stop();
                else if(isGameStart)
                    droneRaceAudioController.droneFly.Play();
                raceUIManager.tabPanel.SetActive(_isTabPanel);
                Time.timeScale = _isTabPanel ? 0f : 1f;
            }
        }
        
        private void CheckStartGame()
        {
            if (timer.waitForStartGame >= 0)
            {
                raceUIManager.timeToStartGameText.text = $"00:{timer.waitForStartGame:00}";
            }
            else
            {
                raceUIManager.droneView.SetActive(true);
                raceUIManager.timeToStartGameText.gameObject.SetActive(false);
                raceUIManager.descriptionPanel.SetActive(false);
                raceUIManager.backgroundImage.SetActive(false);
                isGameStart = true;
                if(!droneRaceAudioController.droneFly.isPlaying && !_isTabPanel)
                    droneRaceAudioController.droneFly.Play();
            }
        }

        private void CheckEndGame()
        {
            if (timer.waitForEndGame >= 0)
            {
                float minutes = Mathf.FloorToInt(timer.waitForEndGame / 60);
                float seconds = Mathf.FloorToInt(timer.waitForEndGame % 60);
                raceUIManager.timeText.text = $"{minutes:00}:{seconds:00}";
                raceUIManager.speedText.text = $"{droneRaceController.currentSpeed:00}";
                raceUIManager.checkpointsCountText.text = $"{_playerRacePosition + 1}";
                _droneRaceCheckNodes = _droneRaceCheckNodes.OrderByDescending(x => x.currentNode).ThenBy(x => x.wayDistance).ToList();
                _playerRacePosition = _droneRaceCheckNodes.IndexOf(playerNode);
            }
            else
            {
                if (!_isResult)
                {
                    var db = FindObjectOfType<DBManager>();
                    if (_playerRacePosition == 0)
                    {
                        raceUIManager.matchResultText.text = "Вы победили!";
                        db.UserStatisticRace.GamesCount++;
                        db.UserStatisticRace.WinsCount++;
                    }
                    else
                    {
                        raceUIManager.matchResultText.text = "Вы проиграли(";
                        db.UserStatisticRace.GamesCount++;
                        db.UserStatisticRace.LosesCount++;
                    }

                    raceUIManager.matchResultPanel.SetActive(true);
                    isGameStart = false;
                    StartCoroutine(BackToMenu());
                    _isResult = true;
                }
            }
        }

        public void TurnUI()
        {
            raceUIManager.droneView.SetActive(false);
            raceUIManager.uiPanel.SetActive(false);
        }

        private IEnumerator BackToMenu()
        {
            yield return new WaitForSeconds(3);
            raceUIManager.backBtn.onClick.Invoke();
        }
    }
}
