using UnityEngine;

namespace Drone
{
    public class GameObjectInfo
    {
        public string ObjectName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public int Layer { get; set; }
        public bool IsActive { get; set; }
        public float YOffset { get; set; }
        public float MaxMouseDistance { get; set; }
        public float Damage { get; set; }
        public float WindMillRotateSpeed { get; set; }
        public float MagnetForce { get; set; }
        public float PendulumMoveSpeed { get; set; }
        public float LeftPendulumAngle { get; set; }
        public float RightPendulumAngle { get; set; }
        public float WindForce { get; set; }
        public float BatteryEnergy { get; set; }
        public float BoostSpeed { get; set; }
        public string HintText { get; set; }
        public int ColorIndex { get; set; }
        public string Text3d { get; set; }
        public string PortalMap { get; set; }

        public string SoundName { get; set; }
        public string TriggerMessageText { get; set; }
        public bool HasPassword { get; set; }
        public string PortPassword { get; set; }
        public float MagnetKillerRotateSpeed { get; set; }
        public float MagnetKillerDamage { get; set; }
        public float MagnetKillerDamageInterval { get; set; }
        public float PreviousXRotation { get; set; }
    }
}