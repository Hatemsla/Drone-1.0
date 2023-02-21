namespace DB
{
    public class UserData
    {
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }
        public float SecondsInGame { get; set; }
        public int SettingsId { get; set; }
        public int StatisticRaceId { get; set; }
        public int StatisticFootballId { get; set; }

        public UserData()
        {
        }
        
        public UserData(string userLogin, string userPassword)
        {
            UserLogin = userLogin;
            UserPassword = userPassword;
        }
        
        public UserData(string userLogin, string userPassword, float secondsInGame, int settingsId)
        {
            UserLogin = userLogin;
            UserPassword = userPassword;
            SecondsInGame = secondsInGame;
            SettingsId = settingsId;
        }

        public UserData(string userLogin, string userPassword, float secondsInGame, int settingsId, int statisticRaceId, int statisticFootballId)
        {
            UserLogin = userLogin;
            UserPassword = userPassword;
            SecondsInGame = secondsInGame;
            SettingsId = settingsId;
            StatisticFootballId = statisticFootballId;
            StatisticRaceId = statisticRaceId;
        }
    }
}