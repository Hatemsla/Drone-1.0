using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Drone.DroneRace
{
    public class RaceAudioManager : MonoBehaviour
    {
        [SerializeField] private GameData gameData;
        [SerializeField] private AudioMixer audioMixer;

        private void Start()
        {
            audioMixer.SetFloat(Idents.AudioMixer.Music, gameData.currentMusicsVolume * 80 - 80);
            audioMixer.SetFloat(Idents.AudioMixer.Effects, gameData.currentEffectsVolume * 80 - 80);
        }
    }
}