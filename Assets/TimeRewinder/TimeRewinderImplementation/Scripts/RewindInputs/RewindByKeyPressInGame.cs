using Builder;
using UnityEngine;

public class RewindByKeyPressInGame : MonoBehaviour
{
    private bool _isRewinding = false;
    [SerializeField] float rewindIntensity = 0.02f; //Variable to change rewind speed
    [SerializeField] AudioSource rewindSound;
    private float _rewindValue;

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space) && BuilderManager.Instance.isMove)
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