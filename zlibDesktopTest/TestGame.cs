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

    private Vector2? deltaFromMouse;

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
        deltaFromMouse = Position - InputManager.Instance.Mouse.ScreenPosition;
    }

    public void OnClickEnd()
    {
        // deltaFromMouse = null;
    }

    public void OnClickCanceled()
    {
        // deltaFromMouse = null;
    }

    public void Update(GameTime gameTime)
    {
        if (deltaFromMouse.HasValue)
        {
            Position = InputManager.Instance.Mouse.ScreenPosition + deltaFromMouse.Value;
            if (InputManager.Instance.Mouse.LeftButtonUp)
            {
                deltaFromMouse = null;
            }
        }
        else
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (
                Position.X < Radius
                || Position.X > WindowManager.Instance.GraphicsDevice.Viewport.Width - Radius
            )
            {
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
                Position = new Vector2(
                    Math.Clamp(
                        Position.X,
                        Radius,
                        WindowManager.Instance.GraphicsDevice.Viewport.Width - Radius
                    ),
                    Position.Y
                );
            }
            if (
                Position.Y < Radius
                || Position.Y > WindowManager.Instance.GraphicsDevice.Viewport.Height - Radius
            )
            {
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
                Position = new Vector2(
                    Position.X,
                    Math.Clamp(
                        Position.Y,
                        Radius,
                        WindowManager.Instance.GraphicsDevice.Viewport.Height - Radius
                    )
                );
            }
        }
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
        return Vector2.Distance(Position, position) <= Radius;
    }

    public float LayerDepthAtScreenPoint(Vector2 position)
    {
        return 0;
    }
}

public class TestGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Random random;
    private Ball ball;

    public TestGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
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
            ball.Position = WindowManager.Instance.Center;
            ball.Velocity = random.NextUnitVector() * 250;
        };

        random = new Random();
        ball = new Ball(
            50,
            WindowManager.Instance.Center,
            random.NextUnitVector() * 250,
            Color.Pink
        );
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

        ball.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        ball.Draw(_spriteBatch);
        _spriteBatch.End();

        _spriteBatch.Begin();
        UIManager.Instance.Draw(_spriteBatch);
        AssetManager
            .Instance.GetFont("ojuju")
            .Draw(
                _spriteBatch,
                "Hello, World!",
                WindowManager.Instance.Bounds,
                new FontProperties()
                {
                    TextFit = TextFit.None,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                }
            );
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
