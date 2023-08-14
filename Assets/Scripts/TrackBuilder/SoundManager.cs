using System;
using System.Collections;
using ObjectsPool;
using UnityEngine;

namespace Drone.Builder
{
    public sealed class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [Header("Pool settings")]
        [SerializeField] private int soundsCount = 10;
        [SerializeField] private AudioSource sound;
        
        [Header("Sounds")]
        public AudioClip collisionClip;
        public AudioClip explosionClip;
        public AudioClip lampOffClip;

        private PoolBase<AudioSource> _soundsPool;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _soundsPool = new PoolBase<AudioSource>(PreloadSound, GetSound, ReturnSound,
                soundsCount);
        }

        private AudioSource PreloadSound()
        {
            var newSound = Instantiate(sound, Vector3.zero, Quaternion.identity, transform);
            return newSound;
        }

        private void GetSound(AudioSource obj)
        {
            obj.gameObject.SetActive(true);
            // obj.Play();
        }

        private void ReturnSound(AudioSource obj)
        {
            obj.gameObject.SetActive(false);
            obj.Stop();
        }

        public IEnumerator Return(AudioSource obj, float time)
        {
            yield return new WaitForSeconds(time);
            
            if(_soundsPool.HasItem(obj))
                _soundsPool.Return(obj);
        }

        public void GetSound(Vector3 position, AudioClip clip)
        {
            var newSound = _soundsPool.Get();
            newSound.clip = clip;
            newSound.transform.position = position;
            newSound.Play();
            StartCoroutine(Return(newSound, clip.length));
        }
        
        public void GetSound(Vector3 position, AudioClip clip, float force)
        {
            var newSound = _soundsPool.Get();
            newSound.clip = clip;
            newSound.volume = Mathf.Clamp(force, 0f, 1f);
            newSound.transform.position = position;
            newSound.Play();
            StartCoroutine(Return(newSound, clip.length));
        }
    }
}