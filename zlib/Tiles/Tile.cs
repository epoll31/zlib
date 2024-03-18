using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace zlib.Tiles;

public class Tile
{
    public Point Position { get; }
    public Rectangle Bounds { get; private set; }
    public Tile[] Neighbors { get; private set; }

    private Dictionary<int, Tile[]> nearbyCache { get; }

    private Dictionary<Direction, Tile> adjacentCache { get; }

    public Tile(Point position)
    {
        adjacentCache = new Dictionary<Direction, Tile>();
        nearbyCache = new Dictionary<int, Tile[]>();
        Position = position;
    }

    public void SetNeighbors(Tile[] neighbors)
    {
        Neighbors = neighbors;
        foreach (Tile t in Neighbors)
        {
            if (t.Position.X == Position.X)
            {
                if (t.Position.Y == Position.Y - 1)
                {
                    adjacentCache[Direction.Up] = t;
                }
                else if (t.Position.Y == Position.Y + 1)
                {
                    adjacentCache[Direction.Down] = t;
                }
            }
            else if (t.Position.Y == Position.Y)
            {
                if (t.Position.X == Position.X - 1)
                {
                    adjacentCache[Direction.Left] = t;
                }
                else if (t.Position.X == Position.X + 1)
                {
                    adjacentCache[Direction.Right] = t;
                }
            }
        }
    }

    public void SetBounds(Rectangle bounds)
    {
        Bounds = bounds;
    }

    public Tile[] GetNearby(int radius)
    {
        if (nearbyCache.ContainsKey(radius))
        {
            return nearbyCache[radius];
        }

        Dictionary<Point, Tile> nearby = new();
        Dictionary<Point, Tile> tooFar = new();
        Queue<Tile> queue = new();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();
            if (nearby.ContainsKey(current.Position) || tooFar.ContainsKey(current.Position))
            {
                continue;
            }

            if (
                Math.Sqrt(
                    Math.Pow(current.Position.X - Position.X, 2)
                        + Math.Pow(current.Position.Y - Position.Y, 2)
                ) <= radius
            // && GameManager.Instance.Ground.PlayableTile(current)
            )
            {
                nearby[current.Position] = current;
            }
            else
            {
                tooFar[current.Position] = current;
            }

            foreach (Tile neighbor in current.Neighbors)
            {
                queue.Enqueue(neighbor);
            }
        }

        Tile[] result = nearby.Values.ToArray();
        nearbyCache[radius] = result;
        return result;
    }

    public Tile GetAdjacent(Direction direction)
    {
        if (adjacentCache.ContainsKey(direction))
        {
            return adjacentCache[direction];
        }
        return null;
    }

    public override bool Equals(object obj)
    {
        return obj is Tile tile && Equals(tile);
    }

    public bool Equals(Tile other)
    {
        if (other == null)
        {
            return false;
        }
        return Position.Equals(other.Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}
