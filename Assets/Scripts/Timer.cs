using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeForStartGame;
    public float timeForEndGame;

    public float waitForStartGame;
    public float waitForEndGame;

    private void Start()
    {
        waitForEndGame = timeForEndGame;
        waitForStartGame = timeForStartGame;
    }

    private void Update()
    {
        WaitForEndGame();
        WaitForStartGame();
    }

    public void WaitForEndGame()
    {
        if (waitForEndGame >= 0 && waitForStartGame <= 0)
        {
            waitForEndGame -= Time.deltaTime;
        }
    }

    public void WaitForStartGame()
    {
        if (waitForStartGame >= 0)
        {
            waitForStartGame -= Time.deltaTime;
        }
    }
}
