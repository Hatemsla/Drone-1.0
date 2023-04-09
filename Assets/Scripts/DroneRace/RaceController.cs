using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DB;
using UnityEngine;
using UnityEngine.UI;

namespace DroneRace
{
    public class RaceController : MonoBehaviour
    {
        public float interfaceScale;
        public float currentAIDroneSpeed;
        public int clipIndex;
        public bool isSimpleMode;
        public bool isGameStart;
        public Transform targetCheckpoint;
        public DroneRaceController droneRaceController;
        public DroneRaceAI droneRaceAI;
        public DroneRaceUIManager raceUIManager;
        public DroneRaceCheckNode playerNode;
        public Timer timer;
        public RacePath racePath;
        public List<DroneRaceCheckNode> droneRaceCheckNodes;
        
        private Camera _mainCamera;
        private DroneRaceCheckNode _checkNode;
        private Vector3 _startPointerSize;
        private int _playerRacePosition;
        private bool _isResult;

        private void Awake()
        {
            _startPointerSize = raceUIManager.pathArrow.sizeDelta;
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            droneRaceController.isSimpleMode = isSimpleMode;
            _checkNode = droneRaceController.droneRaceCheckNode;
            foreach (var drone in droneRaceCheckNodes)
            {
                racePath.nodes[0].localScale = new Vector3(3, 3, 3);
            }

            droneRaceAI.speed *= currentAIDroneSpeed;
        }

        private void Update()
        {
            CheckStartGame();
            CheckEndGame();
            
            droneRaceCheckNodes = droneRaceCheckNodes.OrderByDescending(x => x.currentNode).ThenBy(x => x.wayDistance).ToList();
            raceUIManager.racePositionText.text = $"Позиция: {_playerRacePosition + 1}";
            _playerRacePosition = droneRaceCheckNodes.IndexOf(playerNode);
        }

        private void LateUpdate()
        {
            targetCheckpoint = _checkNode.nodes[_checkNode.currentNode];
            Vector3 realPos = _mainCamera.WorldToScreenPoint(targetCheckpoint.position);
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

        private void CheckStartGame()
        {
            if (timer.waitForStartGame >= 0)
            {
                raceUIManager.timeToStartGameText.text = $"До начала осталось: {timer.waitForStartGame:f1}";
            }
            else
            {
                raceUIManager.timeToStartGameText.gameObject.SetActive(false);
                raceUIManager.descriptionPanel.SetActive(false);
                isGameStart = true;
            }
        }

        private void CheckEndGame()
        {
            if (timer.waitForEndGame >= 0)
            {
                float minutes = Mathf.FloorToInt(timer.waitForEndGame / 60);
                float seconds = Mathf.FloorToInt(timer.waitForEndGame % 60);
                raceUIManager.timeToEndGameText.text = $"{minutes:00}:{seconds:00}";
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

        private IEnumerator BackToMenu()
        {
            yield return new WaitForSeconds(3);
            raceUIManager.backBtn.onClick.Invoke();
        }
    }
}
