﻿namespace Drone.Builder
{
    // Определяет направление соединение относительно стороны просмотра
    // Если смотреть с левой стороны на объект (Y вверх, Z налево), то направление определяется наппрвлением стрелки
    public enum ConnectionDirection
    {
        X,
        MX,
        Y,
        MY,
        Z,
        MZ
    }
}