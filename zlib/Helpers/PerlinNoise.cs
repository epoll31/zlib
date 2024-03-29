using System;
using Microsoft.Xna.Framework;

namespace zlib;

public static class PerlinNoise
{
    private static Random random = new();

    public static float[,] GetEmptyArray(int width, int height)
    {
        float[,] array = new float[width, height];

        return array;
    }

    public static float[,] GenerateWhiteNoise(int width, int height)
    {
        float[,] noise = GetEmptyArray(width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                noise[i, j] = (float)random.NextDouble();
            }
        }

        return noise;
    }

    public static float[,] GenerateSmoothNoise(float[,] baseNoise, int octave)
    {
        int width = baseNoise.GetLength(0);
        int height = baseNoise.GetLength(1);

        float[,] smoothNoise = GetEmptyArray(width, height);

        int samplePeriod = 1 << octave; // calculates 2 ^ k
        float sampleFrequency = 1.0f / samplePeriod;

        for (int i = 0; i < width; i++)
        {
            //calculate the horizontal sampling indices
            int sample_i0 = (i / samplePeriod) * samplePeriod;
            int sample_i1 = (int)((sample_i0 + samplePeriod) % width); //wrap around
            float horizontal_blend = (i - sample_i0) * sampleFrequency;

            for (int j = 0; j < height; j++)
            {
                //calculate the vertical sampling indices
                int sample_j0 = (j / samplePeriod) * samplePeriod;
                int sample_j1 = (int)((sample_j0 + samplePeriod) % height); //wrap around
                float vertical_blend = (j - sample_j0) * sampleFrequency;

                //blend the top two corners
                float top = MathHelper.Lerp(
                    baseNoise[sample_i0, sample_j0],
                    baseNoise[sample_i1, sample_j0],
                    horizontal_blend
                );

                //blend the bottom two corners
                float bottom = MathHelper.Lerp(
                    baseNoise[sample_i0, sample_j1],
                    baseNoise[sample_i1, sample_j1],
                    horizontal_blend
                );

                //final blend
                smoothNoise[i, j] = MathHelper.Lerp(top, bottom, vertical_blend);
            }
        }

        return smoothNoise;
    }

    public static float[,] GeneratePerlinNoise(float[,] baseNoise, int octaveCount)
    {
        int width = baseNoise.GetLength(0);
        int height = baseNoise.GetLength(1);

        float[][,] smoothNoise = new float[octaveCount][,]; //an array of 2D arrays containing

        float persistance = 0.5f;

        //generate smooth noise
        for (int i = 0; i < octaveCount; i++)
        {
            smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
        }

        float[,] perlinNoise = GetEmptyArray(width, height);
        float amplitude = 1.0f;
        float totalAmplitude = 0.0f;

        //blend noise together
        for (int octave = octaveCount - 1; octave > 0; octave--)
        {
            amplitude *= persistance;
            totalAmplitude += amplitude;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i, j] += smoothNoise[octave][i, j] * amplitude;
                }
            }
        }

        //normalisation
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                perlinNoise[i, j] /= totalAmplitude;
            }
        }

        return perlinNoise;
    }
}
