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

        public GraphicsDevice GraphicsDevice { get; private set; }
        public Game CurrentGame { get; private set; }

        public Rectangle Bounds => GraphicsDevice.Viewport.Bounds;
        public Vector2 Center =>
            new(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        public bool IsActive => CurrentGame.IsActive;

        public void Initialize(Game game)
        {
            SetCurrentGame(game);
        }

        public void SetCurrentGame(Game game)
        {
            CurrentGame = game;
            GraphicsDevice = game.GraphicsDevice;
        }
    }
}
