namespace DB
{
    public class UserData
    {
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }

        public UserData(string userLogin, string userPassword)
        {
            UserLogin = userLogin;
            UserPassword = userPassword;
        }
    }
}