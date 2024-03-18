using System;
using System.Numerics;

namespace zlib;

public static partial class StaticHelpers
{
    // public static float Lerp(float x0, float x1, float alpha)
    // {
    //     return x0 * (1 - alpha) + alpha * x1;
    // }

    /// <summary>
    /// Returns a random double between 0 and 1.
    /// if power is 1, it is the same as random.NextDouble(). The higher the power, the more likely the value will be close to 0
    /// </summary>
    /// <param name="random"></param>
    /// <param name="power"></param>
    /// <returns></returns>
    public static double NextDouble(this Random random, int power)
    {
        double value = 1;
        for (int i = 0; i < power; i++)
        {
            value *= random.NextDouble();
        }
        return value;
    }

    public static Vector2 NextUnitVector(this Random random)
    {
        double angle = random.NextDouble() * Math.PI * 2;
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }
}
