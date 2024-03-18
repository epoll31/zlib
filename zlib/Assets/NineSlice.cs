using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace zlib;

public enum NineSliceSection
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Center,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}

public class NineSlice : Image
{
    public int Left { get; }
    public int Top { get; }
    public int Right { get; }
    public int Bottom { get; }

    public Dictionary<NineSliceSection, Rectangle> Sections { get; }

    public NineSlice(Texture2D texture, int margin)
        : this(texture, margin, margin) { }

    public NineSlice(Texture2D texture, int horizontalMargin, int verticalMargin)
        : this(texture, horizontalMargin, verticalMargin, horizontalMargin, verticalMargin) { }

    public NineSlice(Texture2D texture, int left, int top, int right, int bottom)
        : base(texture)
    {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
        Sections = new Dictionary<NineSliceSection, Rectangle>
        {
            { NineSliceSection.TopLeft, new Rectangle(0, 0, left, top) },
            { NineSliceSection.Top, new Rectangle(left, 0, Texture.Width - left - right, top) },
            { NineSliceSection.TopRight, new Rectangle(Texture.Width - right, 0, right, top) },
            { NineSliceSection.Left, new Rectangle(0, top, left, Texture.Height - top - bottom) },
            {
                NineSliceSection.Center,
                new Rectangle(
                    left,
                    top,
                    Texture.Width - left - right,
                    Texture.Height - top - bottom
                )
            },
            {
                NineSliceSection.Right,
                new Rectangle(Texture.Width - right, top, right, Texture.Height - top - bottom)
            },
            {
                NineSliceSection.BottomLeft,
                new Rectangle(0, Texture.Height - bottom, left, bottom)
            },
            {
                NineSliceSection.Bottom,
                new Rectangle(left, Texture.Height - bottom, Texture.Width - left - right, bottom)
            },
            {
                NineSliceSection.BottomRight,
                new Rectangle(Texture.Width - right, Texture.Height - bottom, right, bottom)
            }
        };
    }

    private Dictionary<NineSliceSection, Rectangle> GetSplitBounds(
        Rectangle bounds,
        float edgeScale = 1
    )
    {
        return GetSplitBounds(bounds, edgeScale, edgeScale, edgeScale, edgeScale);
    }

    private Dictionary<NineSliceSection, Rectangle> GetSplitBounds(
        Rectangle bounds,
        float scaleTop,
        float scaleBottom,
        float scaleLeft,
        float scaleRight
    )
    {
        int x0 = bounds.X;
        int x1 = bounds.X + (int)(Left * scaleLeft);
        int x2 = bounds.X + bounds.Width - (int)(Right * scaleRight);
        int x3 = bounds.X + bounds.Width;

        int y0 = bounds.Y;
        int y1 = bounds.Y + (int)(Top * scaleTop);
        int y2 = bounds.Y + bounds.Height - (int)(Bottom * scaleBottom);
        int y3 = bounds.Y + bounds.Height;

        // int x0 = bounds.X;
        // int x1 = bounds.X + left;
        // int x2 = bounds.X + bounds.Width - right;
        // int x3 = bounds.X + bounds.Width;

        // int y0 = bounds.Y;
        // int y1 = bounds.Y + top;
        // int y2 = bounds.Y + bounds.Height - bottom;
        // int y3 = bounds.Y + bounds.Height;

        return new Dictionary<NineSliceSection, Rectangle>
        {
            { NineSliceSection.TopLeft, new Rectangle(x0, y0, x1 - x0, y1 - y0) },
            { NineSliceSection.Top, new Rectangle(x1, y0, x2 - x1, y1 - y0) },
            { NineSliceSection.TopRight, new Rectangle(x2, y0, x3 - x2, y1 - y0) },
            { NineSliceSection.Left, new Rectangle(x0, y1, x1 - x0, y2 - y1) },
            { NineSliceSection.Center, new Rectangle(x1, y1, x2 - x1, y2 - y1) },
            { NineSliceSection.Right, new Rectangle(x2, y1, x3 - x2, y2 - y1) },
            { NineSliceSection.BottomLeft, new Rectangle(x0, y2, x1 - x0, y3 - y2) },
            { NineSliceSection.Bottom, new Rectangle(x1, y2, x2 - x1, y3 - y2) },
            { NineSliceSection.BottomRight, new Rectangle(x2, y2, x3 - x2, y3 - y2) }
        };
    }

    public void Draw(
        SpriteBatch spriteBatch,
        Rectangle destination,
        Color color,
        float layerDepth,
        float edgeScale = 1
    )
    {
        Dictionary<NineSliceSection, Rectangle> splitBounds = GetSplitBounds(
            destination,
            edgeScale
        );
        foreach (var section in Sections.Keys)
        {
            spriteBatch.Draw(
                Texture,
                splitBounds[section],
                Sections[section],
                color,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                layerDepth
            );
        }
    }
}
