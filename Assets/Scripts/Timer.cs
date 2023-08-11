using System;
using Drone.Builder;
using UnityEngine;

namespace Drone
{
    public class Timer : MonoBehaviour
    {
        public float timeForStartGame;
        public float timeForEndGame;

        public float waitForStartGame;
        public float waitForEndGame;

        public float currentTime;

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

        private void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }

        private void ResetGameTime()
        {
            currentTime = 0;
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
            if (waitForEndGame >= 0 && waitForStartGame <= 0 && !RewindManager.Instance.IsBeingRewinded)
            {
                waitForEndGame -= Time.deltaTime;
            }
        }

        private void WaitForStartGame()
        {
            if (waitForStartGame >= 0)
            {
                waitForStartGame -= Time.deltaTime;
            }
        }
    }
}