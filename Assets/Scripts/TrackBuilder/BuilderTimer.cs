using UnityEngine;

namespace Drone.Builder
{
    public sealed class BuilderTimer : Timer
    {
        private void Start()
        {
            waitForEndGame = timeForEndGame;
            waitForStartGame = timeForStartGame;
        }

        private void OnEnable()
        {
            BuilderManager.Instance.StartGame += ResetGameTime;
            BuilderManager.Instance.TestLevelEvent += ResetTimeScale;
        }

        private void OnDestroy()
        {
            BuilderManager.Instance.StartGame -= ResetGameTime;
            BuilderManager.Instance.TestLevelEvent -= ResetTimeScale;
        }

        public override void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }

        private void ResetGameTime()
        {
            currentTime = 0;
            if (GameManager.Instance.gameData.isTeleportLevel)
                waitForEndGame = BuilderManager.Instance.droneBuilderController.GetComponent<DroneRpgController>()
                    .TimeForEndGame;
            else
                waitForEndGame = timeForEndGame;
        }

        private void Update()
        {
            WaitForEndGame();
            WaitForStartGame();
        
            if(!RewindManager.Instance.IsBeingRewinded)
                currentTime += Time.deltaTime;
        }

        private void WaitForEndGame()
        {
            if (waitForEndGame >= 0 && !RewindManager.Instance.IsBeingRewinded)
            {
                waitForEndGame -= Time.deltaTime;
            }
        }

        public override void WaitForStartGame()
        {
            if (waitForStartGame >= 0)
            {
                waitForStartGame -= Time.deltaTime;
            }
        }
    }
}