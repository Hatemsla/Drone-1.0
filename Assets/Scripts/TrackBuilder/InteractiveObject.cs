using System;
using TMPro;
using UnityEngine;

namespace Drone.Builder
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        public bool isActive = true;
        public int colorIndex;

        private bool _isActiveB;
        private bool _previousIsActive;

        public abstract void SetActive(bool active);
        public abstract void SetColorIndex(int color);

        private protected bool CheckColorActiveChange(ColorOption option)
        {
            _isActiveB = option switch
            {
                ColorOption.Белый => BuilderManager.Instance.isActivWhite,
                ColorOption.Красный => BuilderManager.Instance.isActivRed,
                ColorOption.Зелёный => BuilderManager.Instance.isActivGreen,
                ColorOption.Жёлтый => BuilderManager.Instance.isActivYellow,
                ColorOption.Синий => BuilderManager.Instance.isActivBlue,
                _ => _isActiveB
            };

            if (_previousIsActive != _isActiveB)
            {
                _previousIsActive = _isActiveB;
                return true;
            }

            return false;
        }

        private protected static Color GetColorFromOption(ColorOption option)
        {
            return option switch
            {
                ColorOption.Белый => Color.white,
                ColorOption.Красный => Color.red,
                ColorOption.Зелёный => Color.green,
                ColorOption.Жёлтый => Color.yellow,
                ColorOption.Синий => Color.blue,
                _ => Color.white
            };
        }
    }
}