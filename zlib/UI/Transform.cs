using System;
using Microsoft.Xna.Framework;

namespace zlib.UI;

public class Transform
{
    public Element Host { get; set; }
    public Position Position { get; set; } = Position.Relative;
    public int? Top { get; set; }
    public int? Right { get; set; }
    public int? Bottom { get; set; }
    public int? Left { get; set; }

    public int? Horizontals
    {
        set
        {
            Left = value;
            Right = value;
        }
    }
    public int? Verticals
    {
        set
        {
            Top = value;
            Bottom = value;
        }
    }
    public int? All
    {
        set
        {
            Left = value;
            Right = value;
            Top = value;
            Bottom = value;
        }
    }

    public int? Width { get; set; }
    public int? Height { get; set; }
    public Rectangle Bounds
    {
        get
        {
            Rectangle parentBounds = Position switch
            {
                Position.Relative
                    => Host.Parent?.Transform.Bounds
                        ?? WindowManager.Instance.GraphicsDevice.Viewport.Bounds,
                Position.Absolute => WindowManager.Instance.GraphicsDevice.Viewport.Bounds,
                _ => throw new InvalidOperationException("Invalid Position value.")
            };
            int x;
            int width;
            // find x and width
            if (Left.HasValue && Right.HasValue && Width.HasValue)
            {
                throw new InvalidOperationException(
                    "Conflicting constraints: Left, Right, and Width cannot all be set."
                );
            }
            else if (Left.HasValue && Right.HasValue)
            {
                x = parentBounds.X + Left.Value;
                width = parentBounds.Width - Left.Value - Right.Value;
            }
            else if (Left.HasValue && Width.HasValue)
            {
                x = parentBounds.X + Left.Value;
                width = Width.Value;
            }
            else if (Right.HasValue && Width.HasValue)
            {
                x = parentBounds.X + parentBounds.Width - Width.Value - Right.Value;
                width = Width.Value;
            }
            else if (Left.HasValue)
            {
                x = parentBounds.X + Left.Value;
                width = parentBounds.Width - Left.Value;
            }
            else if (Right.HasValue)
            {
                x = parentBounds.X;
                width = parentBounds.Width - Right.Value;
            }
            else if (Width.HasValue)
            {
                x = parentBounds.X + parentBounds.Width / 2 - Width.Value / 2;
                width = Width.Value;
            }
            else
            {
                x = parentBounds.X;
                width = parentBounds.Width;
            }

            int y;
            int height;
            // find y and height
            if (Top.HasValue && Bottom.HasValue && Height.HasValue)
            {
                throw new InvalidOperationException(
                    "Conflicting constraints: Top, Bottom, and Height cannot all be set."
                );
            }
            else if (Top.HasValue && Bottom.HasValue)
            {
                y = parentBounds.Y + Top.Value;
                height = parentBounds.Height - Top.Value - Bottom.Value;
            }
            else if (Top.HasValue && Height.HasValue)
            {
                y = parentBounds.Y + Top.Value;
                height = Height.Value;
            }
            else if (Bottom.HasValue && Height.HasValue)
            {
                y = parentBounds.Y + parentBounds.Height - Height.Value - Bottom.Value;
                height = Height.Value;
            }
            else if (Top.HasValue)
            {
                y = parentBounds.Y + Top.Value;
                height = parentBounds.Height - Top.Value;
            }
            else if (Bottom.HasValue)
            {
                y = parentBounds.Y;
                height = parentBounds.Height - Bottom.Value;
            }
            else if (Height.HasValue)
            {
                y = parentBounds.Y + parentBounds.Height / 2 - Height.Value / 2;
                height = Height.Value;
            }
            else
            {
                y = parentBounds.Y;
                height = parentBounds.Height;
            }

            // find bounds
            return new(x, y, width, height);
        }
    }

    public static Transform From(int value)
    {
        return new Transform() { All = value };
    }

    public static Transform From(int horizontals, int verticals)
    {
        return new Transform() { Horizontals = horizontals, Verticals = verticals };
    }

    public static Transform From(int top, int right, int bottom, int left)
    {
        return new Transform()
        {
            Top = top,
            Right = right,
            Bottom = bottom,
            Left = left
        };
    }
}
