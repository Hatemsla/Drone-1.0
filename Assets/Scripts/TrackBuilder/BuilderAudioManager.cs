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

        public void StartPlay()
        {
            loadVideo.Play();
            loadAudio.Play();
        }

        public void EndPlay()
        {
            loadVideo.Stop();
            loadAudio.Stop();
        }
    }
}