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

        private float _previousMusicValue;
        private float _previousEffectsValue;

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

        public void StartTimeRewind()
        {
            audioMixer.GetFloat(Idents.AudioMixer.Effects, out _previousEffectsValue);
            audioMixer.GetFloat(Idents.AudioMixer.Music, out _previousMusicValue);
            
            audioMixer.SetFloat(Idents.AudioMixer.Effects, -80);
            audioMixer.SetFloat(Idents.AudioMixer.Music, -80);
        }

        public void StopTimeRewind()
        {
            audioMixer.SetFloat(Idents.AudioMixer.Effects, _previousEffectsValue);
            audioMixer.SetFloat(Idents.AudioMixer.Music, _previousMusicValue);
        }
    }
}