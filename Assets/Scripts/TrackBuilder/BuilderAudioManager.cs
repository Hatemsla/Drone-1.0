using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;

namespace Drone.Builder
{
    public class BuilderAudioManager : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public VideoPlayer loadVideo;
        public AudioSource loadAudio;

        private void Start()
        {
            audioMixer.SetFloat("Music", 0);
            audioMixer.SetFloat("Effects", 0);
        }

        private void OnEnable()
        {
            LevelManager.StartLevelLoading += StartPlay;
            BuilderManager.Instance.LoadingCompleteEvent += EndPlay;
        }

        private void OnDestroy()
        {
            LevelManager.StartLevelLoading -= StartPlay;
            BuilderManager.Instance.LoadingCompleteEvent -= EndPlay;
        }

        private void StartPlay()
        {
            loadVideo.Play();
            loadAudio.Play();
        }

        private void EndPlay()
        {
            loadVideo.Stop();
            loadAudio.Stop();
        }
    }
}