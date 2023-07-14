using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer video;
    public VideoClip studioIntro;
    public VideoClip gameIntro;
    
    private void Start()
    {
        StartCoroutine(WaitForNextVideo());
    }

    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame) SceneManager.LoadScene(1);
    }

    private IEnumerator WaitForLevel()
    {
        yield return new WaitForSeconds((float)gameIntro.length);
        video.clip = studioIntro;
        SceneManager.LoadScene(1);
    }

    private IEnumerator WaitForNextVideo()
    {
        yield return new WaitForSeconds((float)studioIntro.length);
        video.clip = gameIntro;
        StartCoroutine(WaitForLevel());
    }
}