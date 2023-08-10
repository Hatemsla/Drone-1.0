namespace Drone.DB
{
    public class UserColors
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }

        public UserColors()
        {
        }

        public UserColors(int colorId, string colorName)
        {
            ColorId = colorId;
            ColorName = colorName;
        } 
    }
}