namespace Builder
{
    [System.Flags]
    public enum ObjectsType
    {
        Floor = 1,
        Wall = 2,
        Slant = 4,
        Gate = 8,
        Drone = 16,
        Lamp = 32,
        Hint = 64,
        Draw = 128,
        Enemy = 256,
        Other = 512,
    }
}