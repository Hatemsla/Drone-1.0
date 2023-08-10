using System.Collections;
using Drone.Menu;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Drone
{
    public class AsyncLoad : MonoBehaviour
    {
        public VideoPlayer loadVideo;
        public AudioSource loadAudio;
        public AudioMixer audioMixer;
        private AsyncOperation _asyncOperation;

        public void LoadScene(int buildIndex)
        {
            StartCoroutine(LoadSceneAsync(buildIndex));
        }

        private IEnumerator LoadSceneAsync(int buildIndex)
        {
            audioMixer.SetFloat("Effects", -80);
            audioMixer.SetFloat("Music", -80);
            loadVideo.Play();
            loadAudio.Play();
            yield return new WaitForSeconds(0.5f);
            _asyncOperation = SceneManager.LoadSceneAsync(buildIndex);

            while (!_asyncOperation.isDone)
            {
                var progress = _asyncOperation.progress;
                if (progress >= 0.9f)
                {
                    _asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}