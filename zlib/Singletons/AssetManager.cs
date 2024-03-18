using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace zlib;

public static class AssetGenerator
{
    public static Texture2D Rectangle(
        GraphicsDevice graphicsDevice,
        int width,
        int height,
        Color color
    )
    {
        Texture2D texture = new(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; ++i)
        {
            data[i] = color;
        }
        texture.SetData(data);
        return texture;
    }

    public static Texture2D Rectangle(
        GraphicsDevice graphicsDevice,
        int width,
        int height,
        int borderWidth,
        Color borderColor,
        Color innerColor
    )
    {
        Texture2D texture = new(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (
                    x < borderWidth
                    || x >= width - borderWidth
                    || y < borderWidth
                    || y >= height - borderWidth
                )
                {
                    data[y * width + x] = borderColor;
                }
                else
                {
                    data[y * width + x] = innerColor;
                }
            }
        }
        texture.SetData(data);
        return texture;
    }

    public static Texture2D Circle(
        GraphicsDevice graphicsDevice,
        int diameter,
        Color circleColor,
        Color backgroundColor = default
    )
    {
        Texture2D texture = new(graphicsDevice, diameter, diameter);
        Color[] data = new Color[diameter * diameter];
        float radius = diameter / 2;
        float radiusSq = radius * radius;
        for (int y = 0; y < diameter; ++y)
        {
            for (int x = 0; x < diameter; ++x)
            {
                float x0 = x - radius;
                float y0 = y - radius;
                if ((x0 * x0) + (y0 * y0) <= radiusSq)
                {
                    data[y * diameter + x] = circleColor;
                }
                else
                {
                    data[y * diameter + x] = backgroundColor;
                }
            }
        }
        texture.SetData(data);
        return texture;
    }

    public static Texture2D Ellipse(
        GraphicsDevice graphicsDevice,
        int width,
        int height,
        Color ellipseColor,
        Color backgroundColor = default
    )
    {
        Texture2D texture = new(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        float radiusX = width / 2;
        float radiusY = height / 2;
        float radiusXsq = radiusX * radiusX;
        float radiusYsq = radiusY * radiusY;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                float x0 = x - radiusX;
                float y0 = y - radiusY;
                if ((x0 * x0) / radiusXsq + (y0 * y0) / radiusYsq <= 1)
                {
                    data[y * width + x] = ellipseColor;
                }
                else
                {
                    data[y * width + x] = backgroundColor;
                }
            }
        }
        texture.SetData(data);
        return texture;
    }
}

public sealed class AssetManager
{
    private static readonly Lazy<AssetManager> lazy = new(() => new AssetManager());
    public static AssetManager Instance
    {
        get { return lazy.Value; }
    }

    private AssetManager() { }

    private ContentManager content;
    private readonly Dictionary<string, Image> Images = new();
    private readonly Dictionary<string, Font> Fonts = new();

    public void Initialize(ContentManager content)
    {
        this.content = content;

        AddImage(
            "blank",
            new Image(
                AssetGenerator.Rectangle(WindowManager.Instance.GraphicsDevice, 3, 3, Color.White)
            )
        );

        AddImage(
            "border",
            new NineSlice(
                AssetGenerator.Rectangle(
                    WindowManager.Instance.GraphicsDevice,
                    50,
                    50,
                    1,
                    Color.White,
                    Color.Transparent
                ),
                1
            )
        );
    }

    public T Load<T>(string path)
    {
        return content.Load<T>(path);
    }

    public void AddImage(string name, string path)
    {
        Images.Add(name, new Image(content.Load<Texture2D>(path)));
    }

    public void AddImage(string name, Image image)
    {
        Images.Add(name, image);
    }

    public void AddFont(string name, Font font)
    {
        Fonts.Add(name, font);
    }

    public Image GetImage(string name) => GetImage<Image>(name);

    public T GetImage<T>(string name)
        where T : Image
    {
        return Images[name] as T;
    }

    public Font GetFont(string name)
    {
        return Fonts[name];
    }
}
