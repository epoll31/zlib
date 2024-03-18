using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib;

public enum VerticalAlignment
{
    Top,
    Center,
    Bottom,
}

public enum HorizontalAlignment
{
    Left,
    Center,
    Right,
}

/// <summary>
/// None: No text fitting
/// Fill: Text will scale to fit the bounds
/// FillWidth: Text will scale horizontally to fit the bounds
/// FillHeight: Text will scale vertically to fit the bounds
/// </summary>
public enum TextFit
{
    None,
    Fill,
    FillWidth,
    FillHeight,
}

public struct FontProperties
{
    // public Vector2 Position { get; set; } = Vector2.Zero;
    public Color Color { get; set; } = Color.White;
    public float FontSize { get; set; } = 32;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    public TextFit TextFit { get; set; } = TextFit.None;

    // private float Rotation { get; set; } = 0;
    // private SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    public float LayerDepth { get; set; } = 0;

    public FontProperties() { }
}

public class Font
{
    public SpriteFont SpriteFont { get; }
    public float TrueFontSize { get; }

    public Font(SpriteFont spriteFont, float trueFontSize)
    {
        SpriteFont = spriteFont;
        TrueFontSize = trueFontSize;
    }

    public Vector2 MeasureString(string text, float fontSize)
    {
        Vector2 size = SpriteFont.MeasureString(text);
        return size * (fontSize / TrueFontSize);
    }

    public void Draw(
        SpriteBatch spriteBatch,
        string text,
        Rectangle destination,
        FontProperties props
    )
    {
        Vector2 trueSize = SpriteFont.MeasureString(text);
        // Vector2 origin =
        //     new(
        //         props.HorizontalAlignment switch
        //         {
        //             HorizontalAlignment.Left => 0,
        //             HorizontalAlignment.Center => trueSize.X / 2,
        //             HorizontalAlignment.Right => trueSize.X,
        //             _ => 0,
        //         },
        //         props.VerticalAlignment switch
        //         {
        //             VerticalAlignment.Top => 0,
        //             VerticalAlignment.Center => trueSize.Y / 2,
        //             VerticalAlignment.Bottom => trueSize.Y,
        //             _ => 0,
        //         }
        //     );

        //determine fill scales without using props.FontSize
        // float scale = props.TextFit switch
        // {
        //     TextFit.None => props.FontSize / TrueFontSize,
        //     TextFit.Fill => Math.Min(destination.Width / trueSize.X, destination.Height / trueSize.Y),
        //     TextFit.FillWidth => destination.Width / trueSize.X,
        //     TextFit.FillHeight => destination.Height / trueSize.Y,
        //     _ => throw new Exception("Invalid TextFit"),
        // };
        // Vector2 origin = props.TextFit switch {
        //     TextFit.None => new Vector2() {
        //         X = props.HorizontalAlignment switch
        //         {
        //             HorizontalAlignment.Left => 0,
        //             HorizontalAlignment.Center => trueSize.X / 2,
        //             HorizontalAlignment.Right => trueSize.X,
        //             _ => 0,
        //         },
        //         Y = props.VerticalAlignment switch
        //         {
        //             VerticalAlignment.Top => 0,
        //             VerticalAlignment.Center => trueSize.Y / 2,
        //             VerticalAlignment.Bottom => trueSize.Y,
        //             _ => 0,
        //         }
        //     } * scale,
        //     TextFit.Fill => Vector2.Zero,
        //     TextFit.FillWidth => Vector2.Zero,
        //     TextFit.FillHeight => Vector2.Zero,
        //     _ => throw new Exception("Invalid TextFit"),
        // };

        float scale;
        Vector2 origin;
        Vector2 position;

        switch (props.TextFit)
        {
            case TextFit.None:
                scale = props.FontSize / TrueFontSize;
                origin = new Vector2(
                    props.HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => 0,
                        HorizontalAlignment.Center => trueSize.X / 2,
                        HorizontalAlignment.Right => trueSize.X,
                        _ => 0,
                    },
                    props.VerticalAlignment switch
                    {
                        VerticalAlignment.Top => 0,
                        VerticalAlignment.Center => trueSize.Y / 2,
                        VerticalAlignment.Bottom => trueSize.Y,
                        _ => 0,
                    }
                );
                position = new Vector2(
                    props.HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => destination.Left,
                        HorizontalAlignment.Center => destination.Center.X,
                        HorizontalAlignment.Right => destination.Right,
                        _ => 0,
                    },
                    props.VerticalAlignment switch
                    {
                        VerticalAlignment.Top => destination.Top,
                        VerticalAlignment.Center => destination.Center.Y,
                        VerticalAlignment.Bottom => destination.Bottom,
                        _ => 0,
                    }
                );
                break;
            case TextFit.Fill:
                scale = Math.Min(destination.Width / trueSize.X, destination.Height / trueSize.Y);
                origin = new Vector2(
                    props.HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => 0,
                        HorizontalAlignment.Center => trueSize.X / 2,
                        HorizontalAlignment.Right => trueSize.X,
                        _ => 0,
                    },
                    props.VerticalAlignment switch
                    {
                        VerticalAlignment.Top => 0,
                        VerticalAlignment.Center => trueSize.Y / 2,
                        VerticalAlignment.Bottom => trueSize.Y,
                        _ => 0,
                    }
                );
                position = new Vector2(
                    props.HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => destination.Left,
                        HorizontalAlignment.Center => destination.Center.X,
                        HorizontalAlignment.Right => destination.Right,
                        _ => 0,
                    },
                    props.VerticalAlignment switch
                    {
                        VerticalAlignment.Top => destination.Top,
                        VerticalAlignment.Center => destination.Center.Y,
                        VerticalAlignment.Bottom => destination.Bottom,
                        _ => 0,
                    }
                );
                break;
            case TextFit.FillWidth:
                scale = destination.Width / trueSize.X;
                origin = new Vector2(
                    props.HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => 0,
                        HorizontalAlignment.Center => trueSize.X / 2,
                        HorizontalAlignment.Right => trueSize.X,
                        _ => 0,
                    },
                    props.VerticalAlignment switch
                    {
                        VerticalAlignment.Top => 0,
                        VerticalAlignment.Center => trueSize.Y / 2,
                        VerticalAlignment.Bottom => trueSize.Y,
                        _ => 0,
                    }
                );
                position = new Vector2(
                    props.HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => destination.Left,
                        HorizontalAlignment.Center => destination.Center.X,
                        HorizontalAlignment.Right => destination.Right,
                        _ => 0,
                    },
                    props.VerticalAlignment switch
                    {
                        VerticalAlignment.Top => destination.Top,
                        VerticalAlignment.Center => destination.Center.Y,
                        VerticalAlignment.Bottom => destination.Bottom,
                        _ => 0,
                    }
                );
                break;
            case TextFit.FillHeight:
                scale = destination.Height / trueSize.Y;
                origin = new Vector2(
                    props.HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => 0,
                        HorizontalAlignment.Center => trueSize.X / 2,
                        HorizontalAlignment.Right => trueSize.X,
                        _ => 0,
                    },
                    props.VerticalAlignment switch
                    {
                        VerticalAlignment.Top => 0,
                        VerticalAlignment.Center => trueSize.Y / 2,
                        VerticalAlignment.Bottom => trueSize.Y,
                        _ => 0,
                    }
                );
                position = new Vector2(
                    props.HorizontalAlignment switch
                    {
                        HorizontalAlignment.Left => destination.Left,
                        HorizontalAlignment.Center => destination.Center.X,
                        HorizontalAlignment.Right => destination.Right,
                        _ => 0,
                    },
                    props.VerticalAlignment switch
                    {
                        VerticalAlignment.Top => destination.Top,
                        VerticalAlignment.Center => destination.Center.Y,
                        VerticalAlignment.Bottom => destination.Bottom,
                        _ => 0,
                    }
                );
                break;
            default:
                throw new Exception("Invalid TextFit");
        }

        spriteBatch.DrawString(
            SpriteFont,
            text,
            position,
            props.Color,
            0,
            origin,
            scale,
            SpriteEffects.None,
            props.LayerDepth
        );
    }
}
