namespace DB
{
    public class UserData
    {
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }
        public float SecondsInGame { get; set; }

        public UserData()
        {
        }

        public UserData(string userLogin, string userPassword, float secondsInGame)
        {
            UserLogin = userLogin;
            UserPassword = userPassword;
            SecondsInGame = secondsInGame;
        }
    }
}