using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Drone.Menu
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
        
        public void ChangeEffectsVolume()
        {
            var gameData = MenuManager.Instance.gameData;
            var menuUIManager = MenuManager.Instance.menuUIManager;
            audioMixer.SetFloat("Effects", menuUIManager.volumeEffectsSlider.value * 80 - 80);
            gameData.currentEffectsVolume = menuUIManager.volumeEffectsSlider.value;
            menuUIManager.effectsVolumeValue.text = (gameData.currentEffectsVolume * 100).ToString("0");
        }
        
        public void ChangeMusicsVolume()
        {
            var gameData = MenuManager.Instance.gameData;
            var menuUIManager = MenuManager.Instance.menuUIManager;
            audioMixer.SetFloat("Music", menuUIManager.volumeMusicsSlider.value * 80 - 80);
            gameData.currentMusicsVolume = menuUIManager.volumeMusicsSlider.value;
            menuUIManager.musicsVolumeValue.text = (gameData.currentMusicsVolume * 100).ToString("0");
        }
    }
}