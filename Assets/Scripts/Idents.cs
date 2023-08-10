namespace Drone
{
    public static class Idents
    {
        public static class Tags
        {
            public static class SaveLoadTags
            {
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
                public const string Text3DText = "Text";

                public const string Windmill = "Windmill";
                public const string Magnet = "Magnet";
                public const string Pendulum = "Pendulum";
                public const string Battery = "Battery";
                public const string Wind = "Wind";
                public const string Boost = "Boost";
                public const string Hint = "Hint";
                public const string Interactive = "Interactive";
                public const string Text3D = "Text3D";
            }
            
            public static class PromptText
            {
                public const string DefaultText = "Для доступа нажмите F";
                public const string NotEnoughText = "У вас недостаточно монет (не хаватает ";
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
    }
}