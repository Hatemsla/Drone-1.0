using System;
using TMPro;
using UnityEngine;

namespace Drone.Builder
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        public bool isActive = true;
        [HideInInspector] public float windMillRotateSpeed;
        [HideInInspector] public float magnetForce;
        [HideInInspector] public float pendulumMoveSpeed;
        [HideInInspector] public float leftPendulumAngle;
        [HideInInspector] public float rightPendulumAngle;
        [HideInInspector] public float windForce;
        [HideInInspector] public float batteryEnergy;
        [HideInInspector] public float boostSpeed;
        [HideInInspector] public float TimeDelay;

        [HideInInspector] public bool isLampTurn = true;
        [HideInInspector] public TMP_Text hintText;
        [HideInInspector] public string text3D;
        [HideInInspector] public int color_index;
        [HideInInspector] public bool ishacked;
        [HideInInspector] public float buttonDelay;
        [HideInInspector] public bool hasPassword;

        [HideInInspector] public string password;


        private protected bool isActiv;
        private protected bool previousIsActiv;


        public abstract void SetActive(bool active);
        public abstract void SetColorIndex(int color);

        private protected bool CheckColorActivChange(ColorOption option)
        {
            if (option == ColorOption.Белый)
            {
                isActiv = BuilderManager.Instance.isActivWhite;
            }
            else if (option == ColorOption.Красный)
            {
                isActiv = BuilderManager.Instance.isActivRed;
            }
            else if (option == ColorOption.Зелёный)
            {
                isActiv = BuilderManager.Instance.isActivGreen;
            }
            else if (option == ColorOption.Жёлтый)
            {
                isActiv = BuilderManager.Instance.isActivYellow;
            }
            else if (option == ColorOption.Синий)
            {
                isActiv = BuilderManager.Instance.isActivBlue;
            }
            if (previousIsActiv != isActiv)
            {
                previousIsActiv = isActiv;
                return true;
            }
            else
            {
                return false;
            }
        }

        private protected static Color GetColorFromOption(ColorOption option)
        {
            switch (option)
            {
                case ColorOption.Белый:
                    return Color.white;
                case ColorOption.Красный:
                    return Color.red;
                case ColorOption.Зелёный:
                    return Color.green;
                case ColorOption.Жёлтый:
                    return Color.yellow;
                case ColorOption.Синий:
                    return Color.blue;
                default:
                    return Color.white;
            }
        }
    }
}