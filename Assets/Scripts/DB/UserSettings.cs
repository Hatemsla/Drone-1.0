namespace DB
{
    public class UserSettings
    {
        public int SettingsId { get; set; }
        public bool IsFullscreen { get; set; }
        public float SoundLevel { get; set; }
        public float YawRotationSensitivity { get; set; }
        public int ResolutionId { get; set; }
        public int BotsColorId { get; set; }
        public int PlayerColorId { get; set; }

        public UserSettings()
        {
        }

        public UserSettings(int settingsId, bool isFullscreen, float soundLevel, float yawRotationSensitivity, int resolutionId,
            int botsColorId, int playerColorId)
        {
            SettingsId = settingsId;
            IsFullscreen = isFullscreen;
            SoundLevel = soundLevel;
            YawRotationSensitivity = yawRotationSensitivity;
            ResolutionId = resolutionId;
            BotsColorId = botsColorId;
            PlayerColorId = playerColorId;
        }
    }
}