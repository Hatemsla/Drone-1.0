using Menu;
using Npgsql;
using UnityEngine;

namespace DB
{
    public class DBManager : MonoBehaviour
    {
        private readonly string _connectString =
            "Host=localhost;Port=5432;Username=postgres;Password=Bobik123654;Database=drones";

        public MenuManager menuManager;

        private void Start()
        {
            menuManager = GetComponent<MenuManager>();
            DontDestroyOnLoad(this);
        }

        public void LoadSettings()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectString))
            {
                
            }
        }

        public void SaveSettings()
        {
            
        }

        public void Registration(string userLogin, string userPassword)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectString))
            {
                conn.Open();
                string checkPlayeId =
                    $"select exists (select * from users where login = '{userLogin}' and password = '{userPassword}')";
                NpgsqlCommand cmd = new NpgsqlCommand(checkPlayeId, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                bool isPlayerExist = false;
                while (dr.Read())
                {
                    if (dr.GetBoolean(0))
                    {
                        isPlayerExist = true;
                        break;
                    }
                }

                if (!isPlayerExist)
                {
                    string reg = $"insert into users values('{userLogin}', '{userPassword}')";
                    cmd = new NpgsqlCommand(reg, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Login()
        {
            
        }

        public bool CheckUserLoginExist()
        {
            return true;
        }

        public bool CheckUserExist()
        {
            return false;
        }
    }
}
