using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using zlib.UI;

namespace zlib.UI;

public sealed class UIManager
{
    private static readonly Lazy<UIManager> lazy = new(() => new UIManager());
    public static UIManager Instance
    {
        get { return lazy.Value; }
    }

    private UIManager() { }

    public RootElement Root { get; private set; }

    public void Initialize()
    {
        Root = new RootElement();
    }

    public void Update(GameTime gameTime)
    {
        Root.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Root.Draw(spriteBatch);
    }
}
