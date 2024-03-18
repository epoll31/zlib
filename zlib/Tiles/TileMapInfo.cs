using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace zlib.Tiles;

static class TileMapGenerator
{
    private static Random random = new();

    public static Tile[] GenerateRandomBlob(
        uint map_size,
        double blob_size,
        bool clean = true,
        int AFOs = 4
    )
    {
        (double, double, double)[] amplitudeFrequencyOffsets = new (double, double, double)[AFOs];

        double minAmplitude = blob_size * 0.25 / AFOs;
        double maxAmplitude = blob_size * 0.5 / AFOs;

        double minFrequency = 1;
        double maxFrequency = 8;

        double minOffset = 0;
        double maxOffset = Math.PI * 2;

        for (int i = 0; i < AFOs; i++)
        {
            amplitudeFrequencyOffsets[i] = (
                random.NextDouble() * (maxAmplitude - minAmplitude) + minAmplitude,
                random.NextDouble() * (maxFrequency - minFrequency) + minFrequency,
                random.NextDouble() * (maxOffset - minOffset)
            );
        }

        return GenerateBlob(map_size, blob_size, clean, amplitudeFrequencyOffsets);
    }

    private static Point[] GetNeighbors(Point p)
    {
        return new Point[]
        {
            new(p.X - 1, p.Y - 1),
            new(p.X, p.Y - 1),
            new(p.X + 1, p.Y - 1),
            new(p.X - 1, p.Y),
            new(p.X + 1, p.Y),
            new(p.X - 1, p.Y + 1),
            new(p.X, p.Y + 1),
            new(p.X + 1, p.Y + 1)
        };
    }

    public static Tile[] GenerateBlob(
        uint map_size,
        double blob_size,
        bool clean = true,
        params (double, double, double)[] amplitudeFrequencyOffsets
    )
    {
        // Tile[,] blob = new Tile[size * 2, size * 2];
        List<Tile> blob = new();
        Dictionary<Point, List<Tile>> tileNeighbors = new();
        double max_dist = map_size * blob_size;

        for (int i = -(int)map_size; i <= map_size; i++)
        {
            for (int j = -(int)map_size; j <= map_size; j++)
            {
                double angle = Math.Atan2(i, j);
                double dist = Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2)); // / maxDist;

                double value = 1;
                foreach (
                    (double amplitude, double frequency, double offset) in amplitudeFrequencyOffsets
                )
                {
                    value += amplitude * Math.Sin(frequency * angle + offset);
                }

                if (dist * value < max_dist)
                {
                    Tile t = new(new Point(i, j));
                    blob.Add(t);

                    foreach (Point neighbor in GetNeighbors(new Point(i, j)))
                    {
                        if (!tileNeighbors.ContainsKey(neighbor))
                        {
                            tileNeighbors[neighbor] = new List<Tile>();
                        }
                        tileNeighbors[neighbor].Add(t);
                    }
                }
            }
        }
        foreach (Tile t in blob)
        {
            t.SetNeighbors(tileNeighbors[t.Position].ToArray());
        }

        // if (clean)
        // {
        //     for (int i = 0; i < blob.Count; i++)
        //     {
        //         if (blob[i].Neighbors.Length < 8)
        //         {
        //             Console.WriteLine(blob[i].Neighbors.Length);
        //         }
        //         if (blob[i].Neighbors.Length <= 6)
        //         {
        //             blob.RemoveAt(i);
        //             i--;
        //         }
        //     }
        // }
        return blob.ToArray();
    }
}

public class TileMapInfo : IEnumerable<Tile>
{
    public Tile this[int x, int y] => this[new Point(x, y)];
    public Tile this[Point point]
    {
        get
        {
            if (tiles.ContainsKey(point))
            {
                return tiles[point];
            }
            return null;
        }
    }
    private Dictionary<Point, Tile> tiles { get; }
    public Rectangle FullBounds { get; }
    public Rectangle TilesBounds { get; }

    public TileMapInfo(int tileSize, int mapSize)
    {
        Tile[] blob = TileMapGenerator.GenerateRandomBlob((uint)(mapSize / 2), 0.75);

        int tilesXMin = int.MaxValue;
        int tilesXMax = int.MinValue;
        int tilesYMin = int.MaxValue;
        int tilesYMax = int.MinValue;

        int fullBoundsXMin = int.MaxValue;
        int fullBoundsXMax = int.MinValue;
        int fullBoundsYMin = int.MaxValue;
        int fullBoundsYMax = int.MinValue;

        tiles = new Dictionary<Point, Tile>();
        foreach (Tile t in blob)
        {
            Rectangle curr =
                new(t.Position.X * tileSize, t.Position.Y * tileSize, tileSize, tileSize);

            if (t.Position.X < tilesXMin)
            {
                tilesXMin = t.Position.X;
            }
            if (t.Position.X > tilesXMax)
            {
                tilesXMax = t.Position.X + tileSize;
            }
            if (t.Position.Y < tilesYMin)
            {
                tilesYMin = t.Position.Y;
            }
            if (t.Position.Y > tilesYMax)
            {
                tilesYMax = t.Position.Y + tileSize;
            }

            if (curr.X < fullBoundsXMin)
            {
                fullBoundsXMin = curr.X;
            }
            if (curr.X + tileSize > fullBoundsXMax)
            {
                fullBoundsXMax = curr.X + tileSize;
            }
            if (curr.Y < fullBoundsYMin)
            {
                fullBoundsYMin = curr.Y;
            }
            if (curr.Y + tileSize > fullBoundsYMax)
            {
                fullBoundsYMax = curr.Y + tileSize;
            }

            t.SetBounds(curr);

            tiles[t.Position] = t;
        }

        TilesBounds = new(tilesXMin, tilesYMin, tilesXMax - tilesXMin, tilesYMax - tilesYMin);
        FullBounds = new(
            fullBoundsXMin,
            fullBoundsYMin,
            fullBoundsXMax - fullBoundsXMin,
            fullBoundsYMax - fullBoundsYMin
        );
    }

    public Tile GetRandomTile(Random random)
    {
        return tiles.Values.ElementAt(random.Next(tiles.Count));
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        return tiles.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
