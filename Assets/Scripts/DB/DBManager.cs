using System;
using Menu;
using Npgsql;
using UnityEngine;

namespace DB
{
    public class DBManager : MonoBehaviour
    {
        public string userLogin;
        public string userPassword;
        
        private readonly string _connectionString =
            "Host=localhost;Port=5432;Username=postgres;Password=Bobik123654;Database=drones";

        public MenuManager menuManager;

        private void Start()
        {
            menuManager = GetComponent<MenuManager>();
            DontDestroyOnLoad(this);
        }

        public void LoadSettings()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                
            }
        }

        public void SaveSettings()
        {
            
        }

        public void Registration()
        {
            userLogin = menuManager.menuUIManager.regLoginInput.text;
            userPassword = menuManager.menuUIManager.regPasswordInput.text;

            if (!CheckUserExist())
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string reg = $"insert into users values('{userLogin}', '{userPassword}')";
                    NpgsqlCommand cmd = new NpgsqlCommand(reg, conn);
                    cmd.ExecuteNonQuery();
                    menuManager.OpenMenu("Start");
                }

                menuManager.menuUIManager.regLoginInput.text = String.Empty;
                menuManager.menuUIManager.regPasswordInput.text = String.Empty;
            }
        }

        public void Login()
        {
            userLogin = menuManager.menuUIManager.logLoginInput.text;
            userPassword = menuManager.menuUIManager.logPasswordInput.text;

            if (CheckUserExist())
            {
                menuManager.OpenMenu("Start");
                menuManager.menuUIManager.logLoginInput.text = String.Empty;
                menuManager.menuUIManager.logPasswordInput.text = String.Empty;
            }
        }

        private bool CheckUserExist()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string checkUserId =
                    $"select exists (select * from users where login = '{userLogin}' and password = '{userPassword}')";
                NpgsqlCommand cmd =
                    new NpgsqlCommand(checkUserId, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    return dr.GetBoolean(0);
                }
                return false;
            }
        }
    }
}
