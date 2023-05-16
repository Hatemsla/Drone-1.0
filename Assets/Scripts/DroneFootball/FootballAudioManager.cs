using System;
using UnityEngine;
using UnityEngine.Audio;

namespace DroneFootball
{
    public class FootballAudioManager : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public AudioSource droneAudio;

        private void Start()
        {
            audioMixer.SetFloat("Music", 0);
            audioMixer.SetFloat("Effects", 0);
        }
    }
}