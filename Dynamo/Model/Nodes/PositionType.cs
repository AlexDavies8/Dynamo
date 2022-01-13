using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.Model.Nodes
{
    public enum PositionType
    {
        Pixels,
        Fractional,
        Percentage
    }

    public static class PositionTypeExtensions
    {
        public const int MaxPixelSize = 1024 * 8;

        public static float GetPosition(this PositionType type, float position, float size)
        {
            if (type == PositionType.Pixels) return position;
            if (type == PositionType.Fractional) return position * size;
            return position * size * 0.01f; // Percentage
        }

        public static int GetPixelPosition(this PositionType type, float position, int size)
        {
            if (type == PositionType.Pixels) return (int)Math.Min(position, MaxPixelSize);
            if (type == PositionType.Fractional) return (int)Math.Min(position * size, MaxPixelSize);
            return (int)Math.Min(position * size / 0.01f, MaxPixelSize); // Percentage
        }
    }
}
