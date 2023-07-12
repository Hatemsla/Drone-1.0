using System;
using Drone;
using DroneFootball;
using Menu;
using Npgsql;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DB
{
    public class DBManager : MonoBehaviour
    {
        public MenuManager menuManager;

        private readonly string _connectionString =
            "Host=127.0.0.1;Port=5432;Username=postgres;Password=Bobik123654;Database=drones";

        public UserColors BotsColor = new UserColors();
        public UserColors PlayerColor = new UserColors();
        public UserData UserData = new UserData();
        public UserDifficultlyLevels UserDifficultlyLevels = new UserDifficultlyLevels();
        public UserResolutions UserResolutions = new UserResolutions();
        public UserSettings UserSettings = new UserSettings();
        public UserStatisticFootball UserStatisticFootball = new UserStatisticFootball();
        public UserStatisticRace UserStatisticRace = new UserStatisticRace();

        private void Start()
        {
            menuManager = GetComponent<MenuManager>();
            DontDestroyOnLoad(this);
        }

        public void SaveUserRaceStatistic()
        {
            if (IsStatisticsExist(true))
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var updateString =
                        $"update statistics_race set wins_count = {UserStatisticRace.WinsCount}, loses_count = {UserStatisticRace.LosesCount}, " +
                        $"games_count = {UserStatisticRace.GamesCount}, seconds_in_game = {Convert.ToInt32(UserStatisticRace.SecondsInGame)} " +
                        $"where statistic_race_id = {UserStatisticRace.StatisticRaceId}";
                    var cmd = new NpgsqlCommand(updateString, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var insertString =
                        $"insert into statistics_race values({UserStatisticRace.StatisticRaceId}, {UserStatisticRace.WinsCount}, {UserStatisticRace.LosesCount}, " +
                        $"{UserStatisticRace.GamesCount}, {Convert.ToInt32(UserStatisticRace.SecondsInGame)})";
                    var cmd = new NpgsqlCommand(insertString, conn);
                    cmd.ExecuteNonQuery();
                }

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var updateString =
                        $"update users set statistic_race_id = {UserStatisticRace.StatisticRaceId} where login = '{UserData.UserLogin}'";
                    var cmd = new NpgsqlCommand(updateString, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveUserFootballStatistic()
        {
            if (IsStatisticsExist(false))
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var updateString =
                        $"update statistics_football set wins_count = {UserStatisticFootball.WinsCount}, loses_count = {UserStatisticFootball.LosesCount}, " +
                        $"games_count = {UserStatisticFootball.GamesCount}, seconds_in_game = {Convert.ToInt32(UserStatisticFootball.SecondsInGame)} " +
                        $"where statistic_football_id = {UserStatisticFootball.StatisticFootballId}";
                    var cmd = new NpgsqlCommand(updateString, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var insertString =
                        $"insert into statistics_football values({UserStatisticFootball.StatisticFootballId}, {UserStatisticFootball.WinsCount}, {UserStatisticFootball.LosesCount}, " +
                        $"{UserStatisticFootball.GamesCount}, {Convert.ToInt32(UserStatisticFootball.SecondsInGame)})";
                    var cmd = new NpgsqlCommand(insertString, conn);
                    cmd.ExecuteNonQuery();
                }

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var updateString =
                        $"update users set statistic_football_id = {UserStatisticFootball.StatisticFootballId} where login = '{UserData.UserLogin}'";
                    var cmd = new NpgsqlCommand(updateString, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveUserResolution()
        {
            UserResolutions.Width = menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].width;
            UserResolutions.Height = menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].height;
            UserResolutions.FrameRate = (int)menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].refreshRateRatio.value;
            if (IsResolutionExist(UserResolutions.Width, UserResolutions.Height, UserResolutions.FrameRate))
            {
                var resId = 1;
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var selectId =
                        $"select resolution_id from resolutions where width = {UserResolutions.Width} and height = {UserResolutions.Height} and " +
                        $"refresh_rate = {UserResolutions.FrameRate}";
                    var cmd = new NpgsqlCommand(selectId, conn);
                    var dr = cmd.ExecuteReader();
                    while (dr.Read()) resId = dr.GetInt32(0);
                }

                UserSettings.ResolutionId = resId;
            }
            else
            {
                var newId = SelectNewId("resolution_id", "resolutions");
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var selectId =
                        $"insert into resolutions values({newId}, {UserResolutions.Width}, {UserResolutions.Height}, {UserResolutions.FrameRate})";
                    var cmd = new NpgsqlCommand(selectId, conn);
                    cmd.ExecuteNonQuery();
                }

                UserSettings.ResolutionId = newId;
            }
        }

        public void SaveUserSettings()
        {
            // PlayerColor =
            //     new UserColors(
            //         SelectIdWhere("color_id", "colors", "color",
            //             $"{GameManager.Instance.playerColorPreview.color.ToHexString()}"),
            //         GameManager.Instance.playerColorPreview.color.ToHexString());
            // BotsColor = new UserColors(
            //     SelectIdWhere("color_id", "colors", "color",
            //         $"{menuManager.botsColorPreview.color.ToHexString()}"),
            //     menuManager.botsColorPreview.color.ToHexString());

            UserSettings.IsFullscreen = menuManager.menuUIManager.isFullscreenToggle.isOn;
            UserSettings.SoundLevel = GameManager.Instance.gameData.currentEffectsVolume;
            UserSettings.YawRotationSensitivity = GameManager.Instance.gameData.currentYawSensitivity;
            UserSettings.BotsColorId = BotsColor.ColorId;
            UserSettings.PlayerColorId = PlayerColor.ColorId;
            UserSettings.DifficultId = GameManager.Instance.gameData.currentDifficultIndex + 1;
            if (IsSettingsExist())
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var updateString =
                        $"update settings set is_fullscreen = {UserSettings.IsFullscreen}, sound_level = {UserSettings.SoundLevel.ToString().Replace(',', '.')}, " +
                        $"rotation_sensitivity = {UserSettings.YawRotationSensitivity.ToString().Replace(',', '.')}, resolution_id = {UserSettings.ResolutionId}, " +
                        $"bots_color_id = {UserSettings.BotsColorId}, player_color_id = {UserSettings.PlayerColorId}, difficult_id = {UserSettings.DifficultId} " +
                        $"where settings_id = {UserSettings.SettingsId}";
                    var cmd = new NpgsqlCommand(updateString, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var insertString =
                        $"insert into settings values ({UserSettings.SettingsId}, {UserSettings.IsFullscreen}, {UserSettings.SoundLevel.ToString().Replace(',', '.')}, " +
                        $"{UserSettings.YawRotationSensitivity.ToString().Replace(',', '.')}, {UserSettings.ResolutionId}, {UserSettings.BotsColorId}, " +
                        $"{UserSettings.PlayerColorId}, {UserSettings.DifficultId})";
                    var cmd = new NpgsqlCommand(insertString, conn);
                    cmd.ExecuteNonQuery();
                }

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var updateString =
                        $"update users set settings_id = {UserSettings.SettingsId} where login = '{UserData.UserLogin}'";
                    var cmd = new NpgsqlCommand(updateString, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveUserData()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var saveString =
                    $"update users set seconds_in_game = {(int) UserData.SecondsInGame} where login = '{UserData.UserLogin}'";
                var cmd = new NpgsqlCommand(saveString, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void Registration()
        {
            UserDifficultlyLevels = new UserDifficultlyLevels(GameManager.Instance.gameData.currentDifficultIndex + 1,
                menuManager.menuUIManager.difficultDropdown.options[GameManager.Instance.gameData.currentDifficultIndex].text);
            if (IsResolutionExist(menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].width,
                menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].height,
                (int)menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].refreshRateRatio.value))
            {
                var resId = SelectIdWhere("resolution_id", "resolutions", "width", "height", "refresh_rate",
                    menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].width.ToString(),
                    menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].height.ToString(),
                    menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].refreshRateRatio.value.ToString());
                UserResolutions = new UserResolutions(resId,
                    menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].width,
                    menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].height,
                    (int)menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].refreshRateRatio.value);
            }
            else
            {
                var resId = SelectNewId("resolution_id", "resolutions");
                UserResolutions = new UserResolutions(resId,
                    menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].width,
                    menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].height,
                    (int)menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex].refreshRateRatio.value);
            }

            // PlayerColor =
            //     new UserColors(
            //         SelectIdWhere("color_id", "colors", "color",
            //             $"{menuManager.playerColorPreview.color.ToHexString()}"),
            //         menuManager.playerColorPreview.color.ToHexString());
            // BotsColor = new UserColors(
            //     SelectIdWhere("color_id", "colors", "color",
            //         $"{menuManager.botsColorPreview.color.ToHexString()}"),
            //     menuManager.botsColorPreview.color.ToHexString());
            UserSettings = new UserSettings(SelectNewId("settings_id", "settings"),
                menuManager.menuUIManager.isFullscreenToggle.isOn,
                GameManager.Instance.gameData.currentEffectsVolume,
                GameManager.Instance.gameData.currentYawSensitivity,
                UserResolutions.ResolutionId,
                BotsColor.ColorId,
                PlayerColor.ColorId,
                UserDifficultlyLevels.DifficultId);
            UserStatisticFootball =
                new UserStatisticFootball(SelectNewId("statistic_football_id", "statistics_football"), 0, 0, 0, 0);
            UserStatisticRace = new UserStatisticRace(SelectNewId("statistic_race_id", "statistics_race"), 0, 0, 0, 0);
            UserData = new UserData(menuManager.menuUIManager.regLoginInput.text,
                menuManager.menuUIManager.regPasswordInput.text, 0, UserSettings.SettingsId,
                UserStatisticRace.StatisticRaceId, UserStatisticFootball.StatisticFootballId);

            if (!IsUserExist(true))
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var reg = $"insert into users values('{UserData.UserLogin}', '{UserData.UserPassword}')";
                    var cmd = new NpgsqlCommand(reg, conn);
                    cmd.ExecuteNonQuery();
                    menuManager.OpenMenu("Start");
                }

                menuManager.menuUIManager.regLoginInput.text = string.Empty;
                menuManager.menuUIManager.regPasswordInput.text = string.Empty;
            }
        }

        public void Login()
        {
            UserData = new UserData(menuManager.menuUIManager.logLoginInput.text,
                menuManager.menuUIManager.logPasswordInput.text);

            if (IsUserExist(false))
            {
                var userData = LoadUserData(UserData.UserLogin, UserData.UserPassword);
                UserData = new UserData(userData[0], userData[1], Convert.ToSingle(userData[2]),
                    Convert.ToInt32(userData[3]), Convert.ToInt32(userData[4]), Convert.ToInt32(userData[5]));

                var userSettings = LoadUserSettings(UserData.SettingsId.ToString());
                UserSettings = new UserSettings(Convert.ToInt32(userSettings[0]), Convert.ToBoolean(userSettings[1]),
                    Convert.ToSingle(userSettings[2]), Convert.ToSingle(userSettings[3]),
                    Convert.ToInt32(userSettings[4]), Convert.ToInt32(userSettings[5]),
                    Convert.ToInt32(userSettings[6]), Convert.ToInt32(userSettings[7]));

                var userResolution = LoadUserResolution(UserSettings.ResolutionId.ToString());
                UserResolutions = new UserResolutions(Convert.ToInt32(userResolution[0]),
                    Convert.ToInt32(userResolution[1]), Convert.ToInt32(userResolution[2]),
                    Convert.ToInt32(userResolution[3]));

                var playerColor = LoadUserColor(UserSettings.PlayerColorId.ToString());
                PlayerColor = new UserColors(Convert.ToInt32(playerColor[0]), playerColor[1]);
                var botsColor = LoadUserColor(UserSettings.BotsColorId.ToString());
                BotsColor = new UserColors(Convert.ToInt32(botsColor[0]), botsColor[1]);

                var userDifficult = LoadUserDifficult(UserSettings.DifficultId.ToString());
                UserDifficultlyLevels = new UserDifficultlyLevels(Convert.ToInt32(userDifficult[0]), userDifficult[1]);

                var userFootballStatistic = LoadUserFootballStatistic(UserData.StatisticFootballId.ToString());
                UserStatisticFootball = new UserStatisticFootball(Convert.ToInt32(userFootballStatistic[0]),
                    Convert.ToInt32(userFootballStatistic[1]), Convert.ToInt32(userFootballStatistic[2]),
                    Convert.ToInt32(userFootballStatistic[3]), Convert.ToSingle(userFootballStatistic[4]));

                var userRaceStatistic = LoadUserRaceStatistic(UserData.StatisticRaceId.ToString());
                UserStatisticRace = new UserStatisticRace(Convert.ToInt32(userRaceStatistic[0]),
                    Convert.ToInt32(userRaceStatistic[1]), Convert.ToInt32(userRaceStatistic[2]),
                    Convert.ToInt32(userRaceStatistic[3]), Convert.ToSingle(userRaceStatistic[4]));

                menuManager.OpenMenu("Start");
                ApplySettings();
                menuManager.menuUIManager.logLoginInput.text = string.Empty;
                menuManager.menuUIManager.logPasswordInput.text = string.Empty;
            }
        }

        private void ApplySettings()
        {
            menuManager.menuUIManager.volumeEffectsSlider.value = UserSettings.SoundLevel;
            menuManager.menuUIManager.yawSensitivitySlider.value = UserSettings.YawRotationSensitivity - 1;
            menuManager.menuUIManager.isFullscreenToggle.isOn = UserSettings.IsFullscreen;
            menuManager.menuUIManager.difficultDropdown.value = UserSettings.DifficultId - 1;
            // var botsImage = menuManager.botsColorPreview.GetComponent<Image>();
            // var playerImage = menuManager.playerColorPreview.GetComponent<Image>();

            byte[] botsRgba =
            {
                Convert.ToByte(BotsColor.ColorName.Substring(0, 2), 16),
                Convert.ToByte(BotsColor.ColorName.Substring(2, 2), 16),
                Convert.ToByte(BotsColor.ColorName.Substring(4, 2), 16),
                Convert.ToByte(BotsColor.ColorName.Substring(6, 2), 16)
            };

            byte[] playerRgba =
            {
                Convert.ToByte(PlayerColor.ColorName.Substring(0, 2), 16),
                Convert.ToByte(PlayerColor.ColorName.Substring(2, 2), 16),
                Convert.ToByte(PlayerColor.ColorName.Substring(4, 2), 16),
                Convert.ToByte(PlayerColor.ColorName.Substring(6, 2), 16)
            };

            // botsImage.color = new Color32(botsRgba[0], botsRgba[1], botsRgba[2], botsRgba[3]);
            // playerImage.color = new Color32(playerRgba[0], playerRgba[1], playerRgba[2], playerRgba[3]);

            for (var i = 0; i < menuManager.resolutions.Length; i++)
                if (menuManager.resolutions[i].width == UserResolutions.Width &&
                    menuManager.resolutions[i].height == UserResolutions.Height &&
                    menuManager.resolutions[i].refreshRateRatio.value == UserResolutions.FrameRate)
                    GameManager.Instance.gameData.currentResolutionIndex = i;

            menuManager.menuUIManager.resolutionDropdown.value = GameManager.Instance.gameData.currentResolutionIndex;
            var resolution = menuManager.resolutions[GameManager.Instance.gameData.currentResolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, UserSettings.IsFullscreen);
        }

        private string[] LoadUserRaceStatistic(string raceStatisticId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userResolution =
                    $"select * from statistics_race where statistic_race_id = {raceStatisticId}";
                var cmd = new NpgsqlCommand(userResolution, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetInt32(1)} {dr.GetInt32(2)} {dr.GetInt32(3)} {dr.GetInt32(4)}";
            }

            return result.Split(" ");
        }

        private string[] LoadUserFootballStatistic(string footballStatisticId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userResolution =
                    $"select * from statistics_football where statistic_football_id = {footballStatisticId}";
                var cmd = new NpgsqlCommand(userResolution, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetInt32(1)} {dr.GetInt32(2)} {dr.GetInt32(3)} {dr.GetInt32(4)}";
            }

            return result.Split(" ");
        }

        private string[] LoadUserDifficult(string difficultId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userResolution = $"select * from difficulty_levels where difficult_id = {difficultId}";
                var cmd = new NpgsqlCommand(userResolution, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetString(1)}";
            }

            return result.Split(" ");
        }

        private string[] LoadUserColor(string colorId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userResolution = $"select * from colors where color_id = {colorId}";
                var cmd = new NpgsqlCommand(userResolution, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetString(1)}";
            }

            return result.Split(" ");
        }

        private string[] LoadUserResolution(string resolutionId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userResolution = $"select * from resolutions where resolution_id = {resolutionId}";
                var cmd = new NpgsqlCommand(userResolution, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetInt32(1)} {dr.GetInt32(2)} {dr.GetInt32(3)}";
            }

            return result.Split(" ");
        }

        private string[] LoadUserSettings(string settingsId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userSettings = $"select * from settings where settings_id = {settingsId}";
                var cmd = new NpgsqlCommand(userSettings, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetBoolean(1)} {dr.GetFloat(2)} {dr.GetFloat(3)} {dr.GetInt32(4)} {dr.GetInt32(5)} " +
                        $"{dr.GetInt32(6)} {dr.GetInt32(7)}";
            }

            return result.Split(" ");
        }

        private string[] LoadUserData(string login, string password)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userData = $"select * from users where login = '{login}' and password = '{password}'";
                var cmd = new NpgsqlCommand(userData, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetString(0)} {dr.GetString(1)} {dr.GetInt32(2)} {dr.GetInt32(3)} {dr.GetInt32(4)} {dr.GetInt32(5)}";
            }

            return result.Split(" ");
        }

        private bool IsResolutionExist(int width, int height, int frameRate)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var checkResExist =
                    $"select exists (select * from resolutions where width = {width} and height = {height} and refresh_rate = {frameRate})";
                var cmd = new NpgsqlCommand(checkResExist, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    return dr.GetBoolean(0);
                return false;
            }
        }

        private bool IsUserExist(bool isRegistration)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var checkUserId = "";
                if (isRegistration)
                    checkUserId =
                        $"select exists (select * from users where login = '{UserData.UserLogin}')";
                else
                    checkUserId =
                        $"select exists (select * from users where login = '{UserData.UserLogin}' and password = '{UserData.UserPassword}')";

                var cmd =
                    new NpgsqlCommand(checkUserId, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) return dr.GetBoolean(0);
                return false;
            }
        }

        private bool IsStatisticsExist(bool isRace)
        {
            if (isRace)
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var checkRaceId =
                        $"select exists(select * from users where login = '{UserData.UserLogin}' and statistic_race_id is not null)";
                    var cmd =
                        new NpgsqlCommand(checkRaceId, conn);
                    var dr = cmd.ExecuteReader();
                    while (dr.Read()) return dr.GetBoolean(0);
                    return false;
                }

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var checkFootballId =
                    $"select exists(select * from users where login = '{UserData.UserLogin}' and statistic_football_id is not null)";
                var cmd =
                    new NpgsqlCommand(checkFootballId, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) return dr.GetBoolean(0);
                return false;
            }
        }

        private bool IsSettingsExist()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var checkSettingsId =
                    $"select exists(select * from users where login = '{UserData.UserLogin}' and settings_id is not null)";
                var cmd =
                    new NpgsqlCommand(checkSettingsId, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) return dr.GetBoolean(0);
                return false;
            }
        }

        private int SelectSecondsInGame()
        {
            var secondsInGame = "0";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand($"select seconds_in_game from users where login = '{UserData.UserLogin}'",
                    conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    secondsInGame = dr.GetInt32(0).ToString();
            }

            return Convert.ToInt32(secondsInGame);
        }

        private int SelectNewId(string idName, string table)
        {
            var newId = 1;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand($"select max({idName}) from {table}", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    try
                    {
                        newId = dr.GetInt32(0) + 1;
                    }
                    catch
                    {
                        newId = 1;
                    }
            }

            return newId;
        }

        private int SelectIdWhere(string idName, string table, string whereName, string where)
        {
            var id = 1;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand($"select {idName} from {table} where {whereName} = '{where}'", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) id = dr.GetInt32(0);
            }

            return id;
        }

        private int SelectIdWhere(string idName, string table, string whereName1, string whereName2, string whereName3,
            string where1, string where2, string where3)
        {
            var id = 1;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    $"select {idName} from {table} where {whereName1} = '{where1}' and {whereName2} = '{where2}' " +
                    $"and {whereName3} = '{where3}'",
                    conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) id = dr.GetInt32(0);
            }

            return id;
        }
    }
}