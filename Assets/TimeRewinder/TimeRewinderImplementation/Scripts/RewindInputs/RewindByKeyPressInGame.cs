using System;
using Builder;
using Drone;
using Unity.VisualScripting;
using UnityEngine;

public class RewindByKeyPressInGame : MonoBehaviour
{
    private bool _isRewinding;
    private bool _isRewind;
    [SerializeField] private float maxTimeRewind = 10f;
    [SerializeField] float rewindIntensity = 0.02f; //Variable to change rewind speed
    [SerializeField] private AudioSource rewindSound;
    [SerializeField] private DroneRpgController droneRpgController;
    private float _rewindValue;
    private float _timeRewind;

    private void Start()
    {
        _timeRewind = maxTimeRewind;
    }

    private void OnEnable()
    {
        InputManager.Instance.RewindTimeEvent += OnRewind;
        BuilderManager.Instance.TestLevelEvent += FindDroneRpg;
    }

    private void FindDroneRpg()
    {
        if (!droneRpgController)
            droneRpgController = FindObjectOfType<DroneRpgController>();
    }

    private void OnDisable()
    {
        InputManager.Instance.RewindTimeEvent -= OnRewind;
        BuilderManager.Instance.TestLevelEvent -= FindDroneRpg;
    }

    private void OnRewind(bool value)
    {
        if (droneRpgController.SkillsCount[Skills.TimeRewind] > 0 && maxTimeRewind > 0)
        {
            _isRewind = value;
        }
    }

    private void FixedUpdate()
    {
        if (_isRewind && _timeRewind > 0)
        {
            _rewindValue += rewindIntensity;
            _timeRewind -= Time.deltaTime;

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
                
                droneRpgController.UpdateSkillValue(Skills.TimeRewind, droneRpgController.SkillsCount[Skills.TimeRewind] - 1);
                if (droneRpgController.SkillsCount[Skills.TimeRewind] > 0)
                    _timeRewind = maxTimeRewind;
            }
        }
    }
}