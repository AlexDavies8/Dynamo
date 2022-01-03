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
        public static float GetPosition(this PositionType type, float position, float size)
        {
            if (type == PositionType.Pixels) return position;
            if (type == PositionType.Fractional) return position * size;
            return position * size * 0.01f; // Percentage
        }

        public static int GetPixelPosition(this PositionType type, float position, int size)
        {
            if (type == PositionType.Pixels) return (int)position;
            if (type == PositionType.Fractional) return (int)(position * size);
            return (int)(position * size / 0.01f); // Percentage
        }
    }
}
