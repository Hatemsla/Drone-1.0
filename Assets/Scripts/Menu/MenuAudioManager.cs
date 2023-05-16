using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Menu
{
    public class MenuAudioManager : MonoBehaviour
    {
        public AudioMixer audioMixer;

        private void Start()
        {
            audioMixer.SetFloat("Music", 0);
            audioMixer.SetFloat("Effects", 0);
            audioMixer.SetFloat("UI", 0);
        }
    }
}