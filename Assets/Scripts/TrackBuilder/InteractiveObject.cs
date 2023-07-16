using TMPro;
using UnityEngine;

namespace Builder
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        [HideInInspector] public float windMillRotateSpeed;
        [HideInInspector] public float magnetForce;
        [HideInInspector] public float pendulumMoveSpeed;
        [HideInInspector] public float leftPendulumAngle;
        [HideInInspector] public float rightPendulumAngle;
        [HideInInspector] public float windForce;
        [HideInInspector] public bool isFreezing = true;
        [HideInInspector] public float batteryEnergy;
        [HideInInspector] public float boostSpeed = 2;
        [HideInInspector] public bool isLampTurn = true;
        [HideInInspector] public TMP_Text hintText;
    }
}