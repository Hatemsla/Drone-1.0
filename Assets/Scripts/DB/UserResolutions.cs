namespace DB
{
    public class UserResolutions
    {
        public int ResolutionId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameRate { get; set; }

        public UserResolutions()
        {
        }

        public UserResolutions(int resolutionId, int width, int height, int frameRate)
        {
            ResolutionId = resolutionId;
            Width = width;
            Height = height;
            FrameRate = frameRate;
        }
    }
}