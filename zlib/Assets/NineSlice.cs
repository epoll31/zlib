using System;
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
    public int Top { get; }
    public int Bottom { get; }
    public int Left { get; }
    public int Right { get; }
    public int Gap { get; }

    public Dictionary<NineSliceSection, Rectangle> SourceRectangles { get; }

    public NineSlice(Texture2D texture, int edges, int gap = 0)
        : this(texture, edges, edges, gap) { }

    public NineSlice(Texture2D texture, int verticalEdges, int horizontalEdges, int gap = 0)
        : this(texture, horizontalEdges, horizontalEdges, verticalEdges, verticalEdges, gap) { }

    public NineSlice(Texture2D texture, int top, int bottom, int left, int right, int gap = 0)
        : base(texture)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
        Gap = gap;

        int x0 = 0;
        int w1 = left;
        int x1 = left + gap;
        int w2 = texture.Width - left - right - gap * 2;
        int x2 = texture.Width - right - gap;
        int w3 = right;

        int y0 = 0;
        int h1 = top;
        int y1 = top + gap;
        int h2 = texture.Height - top - bottom - gap * 2;
        int y2 = texture.Height - bottom - gap;
        int h3 = bottom;

        SourceRectangles = new Dictionary<NineSliceSection, Rectangle>
        {
            { NineSliceSection.TopLeft, new Rectangle(x0, y0, w1, h1) },
            { NineSliceSection.Top, new Rectangle(x1, y0, w2, h1) },
            { NineSliceSection.TopRight, new Rectangle(x2, y0, w3, h1) },
            { NineSliceSection.Left, new Rectangle(x0, y1, w1, h2) },
            { NineSliceSection.Center, new Rectangle(x1, y1, w2, h2) },
            { NineSliceSection.Right, new Rectangle(x2, y1, w3, h2) },
            { NineSliceSection.BottomLeft, new Rectangle(x0, y2, w1, h3) },
            { NineSliceSection.Bottom, new Rectangle(x1, y2, w2, h3) },
            { NineSliceSection.BottomRight, new Rectangle(x2, y2, w3, h3) }
        };
    }

    public static Dictionary<NineSliceSection, Rectangle> SplitRectangle(
        Rectangle bounds,
        int edges
    ) => SplitRectangle(bounds, edges, edges);

    public static Dictionary<NineSliceSection, Rectangle> SplitRectangle(
        Rectangle bounds,
        int veticalEdges,
        int horizontalEdges
    ) => SplitRectangle(bounds, veticalEdges, veticalEdges, horizontalEdges, horizontalEdges);

    public static Dictionary<NineSliceSection, Rectangle> SplitRectangle(
        Rectangle bounds,
        int top,
        int bottom,
        int left,
        int right
    )
    {
        int x0 = bounds.Left;
        int w1 = left;
        int x1 = bounds.Left + left;
        int w2 = bounds.Right - left - right;
        int x2 = bounds.Right - right;
        int w3 = right;

        int y0 = bounds.Top;
        int h1 = top;
        int y1 = bounds.Top + top;
        int h2 = bounds.Bottom - top - bottom;
        int y2 = bounds.Bottom - bottom;
        int h3 = bottom;

        return new Dictionary<NineSliceSection, Rectangle>
        {
            { NineSliceSection.TopLeft, new Rectangle(x0, y0, w1, h1) },
            { NineSliceSection.Top, new Rectangle(x1, y0, w2, h1) },
            { NineSliceSection.TopRight, new Rectangle(x2, y0, w3, h1) },
            { NineSliceSection.Left, new Rectangle(x0, y1, w1, h2) },
            { NineSliceSection.Center, new Rectangle(x1, y1, w2, h2) },
            { NineSliceSection.Right, new Rectangle(x2, y1, w3, h2) },
            { NineSliceSection.BottomLeft, new Rectangle(x0, y2, w1, h3) },
            { NineSliceSection.Bottom, new Rectangle(x1, y2, w2, h3) },
            { NineSliceSection.BottomRight, new Rectangle(x2, y2, w3, h3) }
        };
    }
    

    public void Draw(
        SpriteBatch spriteBatch,
        Rectangle destination,
        Color color,
        float layerDepth,
        int edges
    ) => Draw(spriteBatch, destination, color, layerDepth, edges, edges);

    public void Draw(
        SpriteBatch spriteBatch,
        Rectangle destination,
        Color color,
        float layerDepth,
        int horizontalEdges,
        int verticalEdges
    ) =>
        Draw(
            spriteBatch,
            destination,
            color,
            layerDepth,
            verticalEdges,
            verticalEdges,
            horizontalEdges,
            horizontalEdges
        );

    public void Draw(
        SpriteBatch spriteBatch,
        Rectangle destination,
        Color color,
        float layerDepth,
        int top,
        int bottom,
        int left,
        int right
    )
    {
        Dictionary<NineSliceSection, Rectangle> splitBounds = SplitRectangle(
            destination,
            top,
            bottom,
            left,
            right
        );
        foreach (NineSliceSection section in SourceRectangles.Keys)
        {
            spriteBatch.Draw(
                Texture,
                splitBounds[section],
                SourceRectangles[section],
                color,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                layerDepth
            );
        }
    }
}
