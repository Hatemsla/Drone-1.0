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
            audioMixer.SetFloat(Idents.AudioMixer.Music, 0);
            audioMixer.SetFloat(Idents.AudioMixer.Effects, 0);
            audioMixer.SetFloat(Idents.AudioMixer.UI, 0);
        }
        
        public void ChangeEffectsVolume()
        {
            var gameData = MenuManager.Instance.gameData;
            var menuUIManager = MenuManager.Instance.menuUIManager;
            audioMixer.SetFloat(Idents.AudioMixer.Effects, menuUIManager.volumeEffectsSlider.value * 80 - 80);
            audioMixer.SetFloat(Idents.AudioMixer.TimeRewind, menuUIManager.volumeEffectsSlider.value * 80 - 80);
            gameData.currentEffectsVolume = menuUIManager.volumeEffectsSlider.value;
            menuUIManager.effectsVolumeValue.text = (gameData.currentEffectsVolume * 100).ToString("0");
        }
        
        public void ChangeMusicsVolume()
        {
            var gameData = MenuManager.Instance.gameData;
            var menuUIManager = MenuManager.Instance.menuUIManager;
            audioMixer.SetFloat(Idents.AudioMixer.Music, menuUIManager.volumeMusicsSlider.value * 80 - 80);
            gameData.currentMusicsVolume = menuUIManager.volumeMusicsSlider.value;
            menuUIManager.musicsVolumeValue.text = (gameData.currentMusicsVolume * 100).ToString("0");
        }
    }
}