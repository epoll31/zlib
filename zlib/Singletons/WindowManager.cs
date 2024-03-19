using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib
{
    public sealed class WindowManager
    {
        private static readonly Lazy<WindowManager> lazy = new(() => new WindowManager());
        public static WindowManager Instance
        {
            get { return lazy.Value; }
        }

        private WindowManager() { }

        public Game CurrentGame { get; private set; }
        public GraphicsDevice GraphicsDevice => CurrentGame.GraphicsDevice;
        public Camera MainCamera { get; private set; }

        public Rectangle Bounds => GraphicsDevice.Viewport.Bounds;

        public bool IsActive => CurrentGame.IsActive;

        public void Initialize(Game game)
        {
            SetCurrentGame(game);
        }

        public void SetCurrentGame(Game game)
        {
            CurrentGame = game;
            MainCamera = new Camera();
        }

        public void Update(GameTime gameTime)
        {
            MainCamera.Update(gameTime);
        }
    }
}
