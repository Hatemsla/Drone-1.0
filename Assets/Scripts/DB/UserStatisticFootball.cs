namespace Drone.DB
{
    public class UserStatisticFootball
    {
        public int StatisticFootballId { get; set; } 
        public int WinsCount { get; set; } 
        public int LosesCount { get; set; } 
        public int GamesCount { get; set; } 
        public float SecondsInGame { get; set; }

        public UserStatisticFootball()
        {
        }

        public UserStatisticFootball(int statisticFootballId, int winsCount, int losesCount, int gamesCount,
            float secondsInGame)
        {
            StatisticFootballId = statisticFootballId;
            WinsCount = winsCount;
            LosesCount = losesCount;
            GamesCount = gamesCount;
            SecondsInGame = secondsInGame;
        }
    }
}