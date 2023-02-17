namespace DB
{
    public class UserDifficultlyLevels
    {
        public int DifficultId { get; set; }
        public string DifficultName { get; set; }

        public UserDifficultlyLevels()
        {
        }

        public UserDifficultlyLevels(int difficultId, string difficultName)
        {
            DifficultId = difficultId;
            DifficultName = difficultName;
        }
    }
}