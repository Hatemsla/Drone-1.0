using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

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

    private void Update()
    {
        WaitForEndGame();
        WaitForStartGame();
        
        if(!RewindManager.Instance.IsBeingRewinded)
            currentTime += Time.deltaTime;
    }

    public void WaitForEndGame()
    {
        if (waitForEndGame >= 0 && waitForStartGame <= 0 && !RewindManager.Instance.IsBeingRewinded)
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
