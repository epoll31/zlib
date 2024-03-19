using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using zlib;
using zlib.UI;

namespace zlibDesktopTest;

public class Ball : IClickable
{
    public int Radius { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Color Color { get; set; }

    public Image Image { get; private set; }

    public bool Interactable => true;

    private bool dragging = false;

    public Rectangle Bounds { get; } = new(-00, -00, 400, 300);

    public Ball(int radius, Vector2 position, Vector2 velocity, Color color)
    {
        Radius = radius;
        Position = position;
        Velocity = velocity;
        Color = color;

        Image = AssetManager.Instance.GetImage("circle");

        (this as IClickable).RegisterClickable();
    }

    public void OnClickStart()
    {
        dragging = true;
    }

    public void Update(GameTime gameTime)
    {
        if (dragging)
        {
            Position = WindowManager.Instance.MainCamera.ScreenToWorld(
                InputManager.Instance.Mouse.ScreenPosition
            );
            if (InputManager.Instance.Mouse.LeftButtonUp)
            {
                dragging = false;
            }
        }
        else
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Position.X < Bounds.Left + Radius || Position.X > Bounds.Right - Radius)
            {
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            }
            if (Position.Y < Bounds.Top + Radius || Position.Y > Bounds.Bottom - Radius)
            {
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
            }
        }
        Position = new Vector2(
            Math.Clamp(Position.X, Bounds.Left + Radius, Bounds.Right - Radius),
            Math.Clamp(Position.Y, Bounds.Top + Radius, Bounds.Bottom - Radius)
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            Image.Texture,
            Position,
            null,
            Color,
            0,
            new Vector2(Radius),
            Radius * 2f / Image.Texture.Width,
            SpriteEffects.None,
            0
        );
    }

    public bool ContainsScreenPoint(Vector2 position)
    {
        return Vector2.Distance(Position, WindowManager.Instance.MainCamera.ScreenToWorld(position))
            <= Radius;
    }

    public float LayerDepthAtScreenPoint(Vector2 position)
    {
        return 0;
    }
}

public class BouncingBallGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Random random;
    private Ball ball;
    private bool panning;

    public BouncingBallGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        WindowManager.Instance.Initialize(this);
        InputManager.Instance.Initialize();

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        AssetManager.Instance.Initialize(Content);
        AssetManager.Instance.AddImage(
            "circle",
            new Image(AssetGenerator.Circle(GraphicsDevice, 100, Color.White))
        );
        AssetManager.Instance.AddFont(
            "ojuju",
            new Font(AssetManager.Instance.Load<SpriteFont>("ojuju"), 100)
        );

        UIManager.Instance.Initialize();

        UIManager.Instance.Root.AddChild(
            new ButtonElement(
                AssetManager.Instance.GetImage("blank"),
                new LabelElement(AssetManager.Instance.GetFont("ojuju"), "Reset")
                {
                    Transform = Transform.From(2),
                    FontProperties = new FontProperties()
                    {
                        TextFit = TextFit.Fill,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Color = Color.Black,
                    },
                    Interactable = false
                }
            )
            {
                ID = "reset_button",
                BackgroundColor = Color.White,
                BorderColor = Color.Pink,
                BorderWidth = 5,
                Transform = new Transform()
                {
                    Left = 10,
                    Top = 10,
                    Width = 100,
                    Height = 50,
                },
            }
        );
        UIManager.Instance.Root.GetElementByID<ButtonElement>("reset_button").OnClickedEvent += (
            sender,
            args
        ) =>
        {
            ball.Position = WindowManager.Instance.Bounds.Center.ToVector2();
            ball.Velocity = random.NextUnitVector() * 250;
        };

        random = new Random();
        ball = new Ball(50, Vector2.Zero, random.NextUnitVector() * 250, Color.Pink);
        WindowManager.Instance.MainCamera.Position += (
            WindowManager.Instance.Bounds.Center.ToVector2() - ball.Bounds.Center.ToVector2()
        );

        InputManager.Instance.OnClickStartMiss += (sender, args) =>
        {
            panning = true;
        };
        InputManager.Instance.OnClickEndMiss += (sender, args) =>
        {
            panning = false;
        };
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        InputManager.Instance.Update(gameTime);
        UIManager.Instance.Update(gameTime);
        WindowManager.Instance.Update(gameTime);
        if (panning)
        {
            WindowManager.Instance.MainCamera.Position += (InputManager.Instance.Mouse.ScreenDelta);
        }
        if (InputManager.Instance.Mouse.RightButtonClicked)
        {
            WindowManager.Instance.MainCamera.Position = (
                WindowManager.Instance.Bounds.Center.ToVector2()
                - Vector2.Transform(
                    ball.Bounds.Center.ToVector2(),
                    WindowManager.Instance.MainCamera.ScaleMatrix
                )
            );
        }
        if (InputManager.Instance.Mouse.ScrollWheelDelta != 0)
        {
            WindowManager.Instance.MainCamera.ScaleOrigin = InputManager
                .Instance
                .Mouse
                .ScreenPosition;
            WindowManager.Instance.MainCamera.Scale *= (
                1 + InputManager.Instance.Mouse.ScrollWheelDelta / 1000f
            );
        }

        ball.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(
            transformMatrix: WindowManager.Instance.MainCamera.Transform,
            samplerState: SamplerState.PointClamp
        );

        ball.Draw(_spriteBatch);
        AssetManager
            .Instance.GetImage<NineSlice>("border")
            .Draw(_spriteBatch, destination: ball.Bounds, Color.White, 0, 10);
        _spriteBatch.End();

        _spriteBatch.Begin();

        Image circle = AssetManager.Instance.GetImage("circle");
        _spriteBatch.Draw(
            circle.Texture,
            WindowManager.Instance.MainCamera.Position,
            null,
            Color.ForestGreen * 0.5f,
            0,
            circle.Texture.Bounds.Center.ToVector2(),
            10f / circle.Texture.Width,
            SpriteEffects.None,
            0
        );

        UIManager.Instance.Draw(_spriteBatch);
        AssetManager
            .Instance.GetFont("ojuju")
            .Draw(
                _spriteBatch,
                "zlib!",
                WindowManager.Instance.Bounds,
                new FontProperties()
                {
                    TextFit = TextFit.None,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                }
            );
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
