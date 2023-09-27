namespace Drone.Builder
{
    [System.Flags]
    public enum ObjectsType
    {
        Floor      = 1 << 0,
        Wall       = 1 << 1,
        Slant      = 1 << 2,
        Gate       = 1 << 3,
        Drone      = 1 << 4,
        Lamp       = 1 << 5,
        Hint       = 1 << 6,
        Draw       = 1 << 7,
        Enemy      = 1 << 8,
        PitStop    = 1 << 9,
        Text3D     = 1 << 10,
        Enviroment = 1 << 11,
        Pipe       = 1 << 12,
    }
}