using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace zlib;

public class PathFinder : IEnumerable<Point>
{
    private Point current;
    public Point Current
    {
        get { return current; }
        set
        {
            // current = value ?? throw new ArgumentNullException(nameof(value));
            current = value;
            FindPath();

            // path = path.SkipWhile(t => !t.Overlaps(current)).ToArray();
            // if (path.Length == 0)
            // {
            //     FindPath();
            // }
        }
    }
    private Point target;

    public Point Target
    {
        get { return target; }
        set
        {
            // target = value ?? throw new ArgumentNullException(nameof(value));
            target = value;
            FindPath();
            // path = path.TakeWhile(t => !t.Overlaps(target)).ToArray();
            // if (path.Length == 0)
            // {
            //     FindPath();
            // }
        }
    }
    private Point[] obstacles;
    public Point[] Obstacles
    {
        get { return obstacles; }
        set
        {
            obstacles = value;
            FindPath();

            // if (path.Any(p => obstacles.Any(o => o.Overlaps(p))))
            // {
            //     FindPath();
            // }
            // else {
            //     // TODO: check for easier path?
            // }
        }
    }
    private PathFinderAlgorithm algorithm;
    public PathFinderAlgorithm Algorithm
    {
        get { return algorithm; }
        set
        {
            if (algorithm == value)
            {
                return;
            }

            algorithm = value;

            FindPath();
        }
    }
    private Point[] path;
    public Point this[int index] => path[index];
    public int Length => path.Length;

    public bool HasReachedTarget => path.Length > 0 && path[0].Equals(Target);
    public int Attempts { get; private set; }

    public PathFinder(
        Point current,
        Point target,
        Point[] obstacles = null,
        PathFinderAlgorithm algorithm = PathFinderAlgorithm.AStar,
        int attempts = 1000
    )
    {
        this.current = current;
        this.target = target;
        this.obstacles = obstacles ?? Array.Empty<Point>();
        this.algorithm = algorithm;
        this.Attempts = attempts;

        FindPath();
    }

    private void FindPath()
    {
        path = Algorithm switch
        {
            PathFinderAlgorithm.AStar
                => PathFinderAlgorithms.AStar(Current, Target, Obstacles, Attempts),
            _ => throw new Exception("Algorithm not implemented"),
        };
    }

    public IEnumerator<Point> GetEnumerator()
    {
        foreach (Point tile in path)
        {
            yield return tile;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public enum PathFinderAlgorithm
{
    AStar
}

public static class PathFinderAlgorithms
{
    private class Node
    {
        public Point tile;
        public Node parent;
        public int g;
        public int h;
        public int f;

        public Node(Point tile, Node parent, int g, int h)
        {
            this.tile = tile;
            this.parent = parent;
            this.g = g;
            this.h = h;
            this.f = g + h;
        }
    }

    private static int ManhattanDistance(Point start, Point end)
    {
        return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
    }

    private static Point[] GetPath(Node node)
    {
        List<Point> path = new();
        Node current = node;
        while (current.parent != null)
        {
            path.Add(current.tile);
            current = current.parent;
        }
        path.Reverse();
        return path.ToArray();
    }

    private static bool PathOverlaps(Node node, Point tile)
    {
        Node current = node;
        while (current.parent != null)
        {
            if (current.tile.Equals(tile))
            {
                return true;
            }
            current = current.parent;
        }
        return false;
    }

    public static Point[] AStar(Point start, Point end, Point[] obstacles, int maxAttempts = 10000)
    {
        // List<Tile> path = new();
        List<Node> openList = new();
        List<Node> closedList = new();

        Node startNode = new(start, null, 0, 0);
        openList.Add(startNode);
        int attempts;
        for (attempts = 0; attempts < maxAttempts && openList.Count > 0; attempts++)
        {
            int minF = openList.Min(n => n.f);
            Node node = openList.Find(n => n.f == minF);
            openList.Remove(item: node);

            for (int i = 0; i < 4; i++)
            {
                Direction direction = (Direction)i;
                Point adjacentTile = direction switch
                {
                    Direction.Up => new(node.tile.X, node.tile.Y - 1),
                    Direction.Right => new(node.tile.X + 1, node.tile.Y),
                    Direction.Down => new(node.tile.X, node.tile.Y + 1),
                    Direction.Left => new(node.tile.X - 1, node.tile.Y),
                    _ => throw new Exception("Direction not implemented"),
                };
                if (adjacentTile.Equals(end))
                {
                    return GetPath(new Node(adjacentTile, node, node.g + 1, 0));
                }
                Node ajdNode =
                    new(adjacentTile, node, node.g + 1, ManhattanDistance(adjacentTile, end));
                if (
                    obstacles.Any(t => t.Equals(adjacentTile))
                    || openList.Any(n => ajdNode.h == n.h && ajdNode.f < n.f)
                    || closedList.Any(n => ajdNode.h == n.h && ajdNode.f < n.f)
                    // || GameManager.Instance.
                    || PathOverlaps(node, adjacentTile)
                )
                {
                    continue;
                }
                openList.Add(ajdNode);
            }

            closedList.Add(node);
        }
        Console.WriteLine($"AStar attempts: {attempts}");
        return GetPath(closedList.Last());
        // return closedList.Select(n => n.tile).ToArray();
        // throw new Exception("No path found");
    }
}
