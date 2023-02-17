using System;
using Menu;
using Npgsql;
using UnityEngine;

namespace DB
{
    public class DBManager : MonoBehaviour
    {
        public MenuManager menuManager;
        public UserData UserData = new UserData();
        public UserSettings UserSettings = new UserSettings();
        
        private readonly string _connectionString =
            "Host=192.168.1.130;Port=5432;Username=postgres;Password=Bobik123654;Database=drones";

        

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

        public void SaveUserSettings()
        {
            if (IsSettingsExist())
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string updateString = $"update settings set is_fullscreen = {menuManager.menuUIManager.isFullscreenToggle.isOn}";
                }
            }
        }

        public void SaveUserData()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string saveString =
                    $"update users set seconds_in_game = {(int)UserData.SecondsInGame} where login = '{UserData.UserLogin}'";
                NpgsqlCommand cmd = new NpgsqlCommand(saveString, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void Registration()
        {
            UserData = new UserData(menuManager.menuUIManager.regLoginInput.text, menuManager.menuUIManager.regPasswordInput.text, 0);
            // UserSettings = new UserSettings(menuManager.menuUIManager.isFullscreenToggle.isOn,
            //     menuManager.menuUIManager.volumeSlider.value,
            //     menuManager.menuUIManager.yawSensitivitySlider.value + 1, );

            if (!CheckUserExist())
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string reg = $"insert into users values('{UserData.UserLogin}', '{UserData.UserPassword}')";
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
            UserData = new UserData(menuManager.menuUIManager.logLoginInput.text, menuManager.menuUIManager.logPasswordInput.text, 0);

            if (CheckUserExist())
            {
                menuManager.OpenMenu("Start");
                menuManager.menuUIManager.logLoginInput.text = String.Empty;
                menuManager.menuUIManager.logPasswordInput.text = String.Empty;
                UserData.SecondsInGame = SelectSecondsInGame();
            }
        }

        private bool CheckUserExist()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string checkUserId =
                    $"select exists (select * from users where login = '{UserData.UserLogin}' and password = '{UserData.UserPassword}')";
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

        private bool IsSettingsExist()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string checkSettingsId =
                    $"select exists(select * from users where login = '{UserData.UserLogin}' and settings_id is null)";
                NpgsqlCommand cmd =
                    new NpgsqlCommand(checkSettingsId, conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    return dr.GetBoolean(0);
                }
                return false;
            }
        }

        private int SelectSecondsInGame()
        {
            string secondsInGame = "0";
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand($"select seconds_in_game from users where login = '{UserData.UserLogin}'", conn);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                    secondsInGame = dr.GetInt32(0).ToString();
            }

            return Convert.ToInt32(secondsInGame);
        }
    }
}
