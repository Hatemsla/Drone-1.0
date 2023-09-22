namespace Drone
{
    public static class Idents
    {
        public static class Tags
        {
            public static class SaveLoadTags
            {
                // Save data names
                public const string ObjectName = "name";
                public const string Position = "position";
                public const string Rotation = "rotation";
                public const string Scale = "scale";
                public const string Layer = "layer";
                public const string YOffset = "yOffset";
                public const string MaxMouseDistance = "maxMouseDistance";
                public const string Damage = "damage";
                public const string WindMillRotateSpeed = "windMillRotateSpeed";
                public const string MagnetForce = "magnetForce";
                public const string PendulumMoveSpeed = "pendulumMoveSpeed";
                public const string LeftPendulumAngle = "leftPendulumAngle";
                public const string RightPendulumAngle = "rightPendulumAngle";
                public const string WindForce = "windForce";
                public const string BatteryEnergy = "batteryEnergy";
                public const string BoostSpeed = "boostSpeed";
                public const string HintText = "hintText";
                public const string ColorIndex = "colorIndex";
                public const string IsActive = "isActive";
                public const string Text3DText = "text3d";
                public const string PortalMap = "portalMapName";
                public const string TriggerMessageHint = "triggerMessageHint";
                public const string TriggerMessageSound = "triggerMessageSound";
                public const string HasPassword = "hasPassword";
                public const string PortPassword = "portPassword";
                public const string MagnetKillerRotateSpeed = "magnetRotateSpeed";
                public const string MagnetKillerDamage = "magnetKillerDamage";
                public const string MagnetKillerDamageInterval = "magnetKillerDamageInterval";
                public const string PreviousRotation = "previousXRotation";
                
                // Interactive objects names
                public const string Windmill = "Windmill";
                public const string Magnet = "Magnet";
                public const string Pendulum = "Pendulum";
                public const string Battery = "Battery";
                public const string Wind = "Wind";
                public const string Boost = "Boost";
                public const string Hint = "Hint";
                public const string Interactive = "Interactive";
                public const string Text3D = "Text3D";
                public const string Portal = "Portal";
                public const string TriggerMessage = "TriggerMessage";
                public const string Port = "Port";
                public const string MagnetKiller = "MagnetKiller";
            }
            
            public static class PromptText
            {
                public const string DefaultText = "Для доступа нажмите F";
                public const string NotEnoughText = "У вас недостаточно монет (не хаватает ";
                public const string PortalText = "Для телепортации нажимте F";
                public const string NoPortalText = "Портал не работает";
            }
        }
        
        public static class ActionMaps
        {
            public const string Ui = "UI";
            public const string Player = "Player";
            public const string Port = "Port";
            public const string PortPassword = "PortPassword";
            public const string Builder = "Builder";
            public const string Camera = "Camera";
        }
        
        public static class AudioMixer
        {
            public const string Music = "Music";
            public const string Effects = "Effects";
            public const string UI = "UI";
            public const string LoadMusic = "LoadMusic";
            public const string Master = "Master";
            public const string TimeRewind = "TimeRewind";
        }
        
        public static class HexColors
        {
            public const string YellowFaff00 = "FAFF00";
            public const string RedFf0000 = "FF0000";
            public const string Blue8Cf2F4 = "8CF2F4";
            public const string Gray8B8B8B = "8B8B8B";
            public const string White = "FFFFFF";
            public const string Blue1A5Fc1 = "1A5FC1";
            public const string Green52Ff39 = "52FF39";
            public const string Turquoise00Ebff = "00EBFF";
        }
        
        public static class Layers
        {
            public const string FloorConnection = "FloorConnection";
            public const string WallConnection = "WallConnection";
            public const string SlantConnection = "SlantConnection";
            public const string IgnoreRaycast = "Ignore Raycast";
            public const string Hint = "Hint";
            public const string Draw = "Draw";
            public const string Intangible = "Intangible";
            public const string Text3D = "Text3D";
            public const string TrackGround = "TrackGround";
            public const string Track = "Track";
        }
    }
}