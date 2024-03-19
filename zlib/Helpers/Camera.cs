using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace zlib;

public static class VectorExtensions
{
    public static Vector3 Extend(this Vector2 vector, float z)
    {
        return new(vector.X, vector.Y, z);
    }
}

public class Camera
{
    // public Matrix Transform { get; set; }

    private float minScale = 0.6f;
    private float maxScale = 3.5f;

    private float scale;
    public Vector2 ScaleOrigin { get; set; }
    public float Scale
    {
        get => scale;
        set
        {
            float newScale = Math.Clamp(value, minScale, maxScale);

            ScaleMatrix *= Matrix.CreateScale(newScale / scale);
            TranslationMatrix *= Matrix.CreateTranslation(
                (ScaleOrigin.X - Position.X) * (1 - newScale / scale),
                (ScaleOrigin.Y - Position.Y) * (1 - newScale / scale),
                0
            );

            scale = newScale;
        }
    }

    public Vector2 Position
    {
        get => new(Transform.Translation.X, Transform.Translation.Y);
        set { TranslationMatrix = Matrix.CreateTranslation(value.Extend(0)); }
    }

    public Matrix ScaleMatrix { get; private set; }
    public Matrix TranslationMatrix { get; private set; }
    public Matrix Transform => ScaleMatrix * TranslationMatrix;

    public Camera(Vector2 position = new Vector2(), float scale = 1)
    {
        this.scale = Math.Clamp(scale, minScale, maxScale);

        ScaleMatrix = Matrix.CreateScale(scale);
        TranslationMatrix = Matrix.CreateTranslation(position.Extend(0));
    }

    public Vector2 ScreenToWorld(Vector2 screen)
    {
        return Vector2.Transform(screen, Matrix.Invert(Transform));
    }

    public Vector2 WorldToScreen(Vector2 world)
    {
        return Vector2.Transform(world, Transform);
    }

    public void Update(GameTime gameTime) { }
}
