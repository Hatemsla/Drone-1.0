using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DroneRace
{
    public class DroneRaceController : MonoBehaviour
    {
        public float interfaceScale;
        public bool isSimpleMode;
        public bool isGameStart;
        public Transform target;
        public DroneController droneController;
        public DroneRaceUIManager droneRaceUIManager;
        public DroneRaceCheckNode playerNode;
        public Timer timer;
        public List<DroneRaceCheckNode> droneRaceCheckNodes;
        
        public Camera mainCamera;
        private DroneRaceCheckNode _checkNode;
        private Vector3 _startPointerSize;
        private int _playerRacePosition;


        private void Awake()
        {
            _startPointerSize = droneRaceUIManager.pathArrow.sizeDelta;
        }

        private void Start()
        {
            droneController.isSimpleMode = isSimpleMode;
            _checkNode = droneController.droneRaceCheckNode;
            droneRaceUIManager.scoreText.text = $"Счет: {_checkNode.passedNode}";
        }

        private void Update()
        {
            CheckStartGame();
            CheckEndGame();
            
            droneRaceCheckNodes = droneRaceCheckNodes.OrderByDescending(x => x.passedNode).ThenBy(x => x.wayDistance).ToList();
            droneRaceUIManager.racePositionText.text = $"Позиция: {_playerRacePosition + 1}";
            _playerRacePosition = droneRaceCheckNodes.IndexOf(playerNode);
        }

        private void LateUpdate()
        {
            target = _checkNode.nodes[_checkNode.currentNode];
            Vector3 realPos = mainCamera.WorldToScreenPoint(target.position);
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            Vector3 outPos = realPos;
            float direction = 1;

            droneRaceUIManager.pathArrow.GetComponent<Image>().sprite = droneRaceUIManager.outOfScreenIcon;

            if (!IsBehind(target.position)) // если цель спереди
            {
                if (rect.Contains(realPos)) // и если цель в окне экрана
                {
                    droneRaceUIManager.pathArrow.GetComponent<Image>().sprite = droneRaceUIManager.pointerIcon;
                }
            }
            else // если цель cзади
            {
                realPos = -realPos;
                outPos = new Vector3(realPos.x, 0, 0); // позиция иконки - снизу
                if (mainCamera.transform.position.y < target.position.y)
                {
                    direction = -1;
                    outPos.y = Screen.height; // позиция иконки - сверху				
                }
            }
            // ограничиваем позицию областью экрана
            float offset = droneRaceUIManager.pathArrow.sizeDelta.x / 2;
            outPos.x = Mathf.Clamp(outPos.x, offset, Screen.width - offset);
            outPos.y = Mathf.Clamp(outPos.y, offset, Screen.height - offset);

            Vector3 pos = realPos - outPos; // направление к цели из PointerUI 

            RotatePointer(direction * pos);

            droneRaceUIManager.pathArrow.sizeDelta = new Vector2(_startPointerSize.x / 100 * interfaceScale, _startPointerSize.y / 100 * interfaceScale);
            droneRaceUIManager.pathArrow.anchoredPosition = outPos;
        }

        private bool IsBehind(Vector3 point) // true если point сзади камеры
        {
            Vector3 forward = mainCamera.transform.TransformDirection(Vector3.forward);
            Vector3 toOther = point - mainCamera.transform.position;
            if (Vector3.Dot(forward, toOther) < 0) return true;
            return false;
        }

        private void RotatePointer(Vector2 direction) // поворачивает PointerUI в направление direction
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            droneRaceUIManager.pathArrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void CheckStartGame()
        {
            if (timer.waitForStartGame >= 0)
            {
                droneRaceUIManager.timeToStartGameText.text = $"До начала осталось: {timer.waitForStartGame:f1}";
            }
            else
            {
                droneRaceUIManager.timeToStartGameText.gameObject.SetActive(false);
                droneRaceUIManager.descriptionPanel.SetActive(false);
                isGameStart = true;
            }
        }

        private void CheckEndGame()
        {
            if (timer.waitForEndGame >= 0)
            {
                float minutes = Mathf.FloorToInt(timer.waitForEndGame / 60);
                float seconds = Mathf.FloorToInt(timer.waitForEndGame % 60);
                droneRaceUIManager.timeToEndGameText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                if (_checkNode.passedNode == _checkNode.nodes.Count - 1)
                    droneRaceUIManager.matchResultText.text = "Вы победили!";

                droneRaceUIManager.matchResultPanel.SetActive(true);
                isGameStart = false;
                StartCoroutine(BackToMenu());
            }
        }

        public void CheckScore()
        {
            droneRaceUIManager.scoreText.text = $"Счет: {_checkNode.passedNode}";
        }

        private IEnumerator BackToMenu()
        {
            yield return new WaitForSeconds(3);
            droneRaceUIManager.backBtn.onClick.Invoke();
        }
    }
}
