using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace zlib;

public class MouseHandler
{
    private MouseState ms;
    private MouseState lms;

    public Vector2 ScreenPosition { get; private set; }
    public Point WorldPosition { get; private set; }

    public Vector2 ScreenDelta { get; private set; }
    public Point WorldDelta { get; private set; }

    public int ScrollWheelValue { get; private set; }
    public int ScrollWheelDelta { get; private set; }

    public bool ScrollWheelChanged { get; private set; }

    public bool LeftButtonUp { get; private set; }
    public bool LeftButtonDown { get; private set; }
    public bool LeftButtonClickStart { get; private set; }
    public bool LeftButtonClickEnd { get; private set; }

    public bool RightButtonClicked { get; private set; }
    public bool RightButtonDown { get; private set; }

    public MouseHandler() { }

    public void Update()
    {
        lms = ms;
        ms = Mouse.GetState();

        ScreenPosition = new(ms.X, ms.Y);
        ScreenDelta = new(ms.X - lms.X, ms.Y - lms.Y);

        ScrollWheelValue = ms.ScrollWheelValue;
        ScrollWheelDelta = ms.ScrollWheelValue - lms.ScrollWheelValue;
        ScrollWheelChanged = ScrollWheelDelta != 0;

        LeftButtonUp = ms.LeftButton == ButtonState.Released;
        LeftButtonDown = ms.LeftButton == ButtonState.Pressed;
        LeftButtonClickStart =
            ms.LeftButton == ButtonState.Pressed && lms.LeftButton == ButtonState.Released;
        LeftButtonClickEnd =
            ms.LeftButton == ButtonState.Released && lms.LeftButton == ButtonState.Pressed;

        RightButtonClicked =
            ms.RightButton == ButtonState.Pressed && lms.RightButton == ButtonState.Released;
        RightButtonDown = ms.RightButton == ButtonState.Pressed;
    }
}
