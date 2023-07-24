using System;
using TMPro;
using UnityEngine;

namespace Builder
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
        [HideInInspector] public bool isLampTurn = true;
        [HideInInspector] public TMP_Text hintText;
        [HideInInspector] public int color_index;
        [HideInInspector] public bool ishacked;
        [HideInInspector] public int n1;
        [HideInInspector] public int n2;
        [HideInInspector] public int n3;
        [HideInInspector] public float buttonDelay;

        public abstract void SetActive(bool active);
    }
}