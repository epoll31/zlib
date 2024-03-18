using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace zlib;

public class Camera
{
    public Matrix Transform { get; set; }

    private float minScale = 0.6f;
    private float maxScale = 3.5f;

    private float scale;

    private readonly float startScale;
    private readonly Vector2 startPosition;

    public Camera(float x, float y, float scale = 1)
    {
        startPosition = new Vector2(x, y);
        startScale = scale;
        this.scale = startScale;
        Transform = Matrix.CreateTranslation(x, y, 0) * Matrix.CreateScale(this.scale);
    }

    public void MoveBy(Vector2 delta)
    {
        Transform *= Matrix.CreateTranslation(delta.X, delta.Y, 0);
    }

    // public void MoveWorldToCenter(Point point)
    // {
    //     // TODO: Remove reference to GameManager
    //     Viewport screen = GameManager.Instance.GraphicsDevice.Viewport;
    //     Vector2 delta = screen.Bounds.Center.ToVector2() - WorldToScreen(point);
    //     MoveBy(delta);
    // }

    public void ZoomAt(float v, float x, float y)
    {
        float newScale = Math.Clamp(scale * v, minScale, maxScale);

        Transform *=
            Matrix.CreateTranslation(-x, -y, 0)
            * Matrix.CreateScale(newScale / scale)
            * Matrix.CreateTranslation(x, y, 0);

        scale = newScale;
        // Console.WriteLine($"scale: {scale}");
    }

    public void Reset()
    {
        scale = startScale;
        Transform =
            Matrix.CreateTranslation(startPosition.X, startPosition.Y, 0)
            * Matrix.CreateScale(scale);
    }

    public Point ScreenToWorld(Vector2 point)
    {
        return Vector2.Transform(new Vector2(point.X, point.Y), Matrix.Invert(Transform)).ToPoint();
    }

    public Vector2 WorldToScreen(Point point)
    {
        return Vector2.Transform(new Vector2(point.X, point.Y), Transform);
    }

    public void Update(GameTime gameTime)
    {
        // TODO: Remove references to InputManager
        // if (InputManager.Instance.IsActive)
        // {
        //     if (InputManager.Instance.Mouse.LeftButtonDown)
        //     {
        //         MoveBy(InputManager.Instance.Mouse.ScreenDelta);
        //     }
        //     ZoomAt(
        //         1 + InputManager.Instance.Mouse.ScrollWheelDelta / 10000f,
        //         InputManager.Instance.Mouse.ScreenPosition.X,
        //         InputManager.Instance.Mouse.ScreenPosition.Y
        //     );
        // }
        // ZoomAt(1 + (ms.ScrollWheelValue - lms.ScrollWheelValue) / 1000f, ms.X, ms.Y);
    }
}
