using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DroneFootball
{
    public class AsyncLoad : MonoBehaviour
    {
        public float loadingProgress;
        public Slider loadSlider;
        private AsyncOperation _asyncOperation;

        public void LoadScene(int buildIndex, Slider slider)
        {
            StartCoroutine(LoadSceneAsync(buildIndex, slider));
        }

        private IEnumerator LoadSceneAsync(int buildIndex, Slider slider)
        {
            yield return new WaitForSeconds(0.5f);
            slider.value = 0;
            _asyncOperation = SceneManager.LoadSceneAsync(buildIndex);
            float progress = 0;

            while (!_asyncOperation.isDone)
            {
                progress = _asyncOperation.progress;
                slider.value = progress;
                if (progress >= 0.9f)
                {
                    slider.value = 1;
                    _asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}