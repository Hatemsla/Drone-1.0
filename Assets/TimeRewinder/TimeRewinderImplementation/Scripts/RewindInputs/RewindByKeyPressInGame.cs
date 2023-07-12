using System;
using Builder;
using Drone;
using Unity.VisualScripting;
using UnityEngine;

public class RewindByKeyPressInGame : MonoBehaviour
{
    private bool _isRewinding;
    private bool _isRewind;
    [SerializeField] float rewindIntensity = 0.02f; //Variable to change rewind speed
    [SerializeField] AudioSource rewindSound;
    private float _rewindValue;

    private void OnEnable()
    {
        InputManager.Instance.RewindTimeEvent += OnRewind;
    }

    private void OnDisable()
    {
        InputManager.Instance.RewindTimeEvent -= OnRewind;
    }

    private void OnRewind(bool value) => _isRewind = value;

    private void FixedUpdate()
    {
        if (_isRewind)
        {
            _rewindValue += rewindIntensity;

            if (!_isRewinding)
            {
                RewindManager.Instance.StartRewindTimeBySeconds(_rewindValue);
                // rewindSound.Play();
            }
            else
            {
                if (RewindManager.Instance.HowManySecondsAvailableForRewind > _rewindValue)
                    RewindManager.Instance.SetTimeSecondsInRewind(_rewindValue);
            }

            _isRewinding = true;
        }
        else
        {
            if (_isRewinding)
            {
                RewindManager.Instance.StopRewindTimeBySeconds();
                // rewindSound.Stop();
                _rewindValue = 0;
                _isRewinding = false;
            }
        }
    }
}