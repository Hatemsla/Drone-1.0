using System;
using Drone.Builder;
using UnityEngine;

namespace Drone
{
    public abstract class Timer : MonoBehaviour
    {
        public float timeForStartGame;
        public float timeForEndGame;

        public float waitForStartGame;
        public float waitForEndGame;

        public float currentTime;

        public abstract void ResetTimeScale();
        public abstract void WaitForStartGame();

    }
}