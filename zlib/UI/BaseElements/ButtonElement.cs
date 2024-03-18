using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib.UI;

public class ButtonClickedEventArgs : EventArgs
{
    // public ButtonElement Button { get; set; }
}

public class ButtonElement : ImageElement, IClickable
{
    public Color HoverColor { get; set; } = Color.LightGray;
    public Color ClickColor { get; set; } = Color.PaleGreen;

    public event EventHandler<ButtonClickedEventArgs> OnClickedEvent;

    public ButtonElement(Image image, params Element[] children)
        : base(image, children) { }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }

    public void OnHover()
    {
        BackgroundColor = HoverColor;
    }

    public void OnHoverEnd()
    {
        BackgroundColor = Color.White;
    }

    public void OnClick()
    {
        BackgroundColor = ClickColor;
    }

    public void OnClickEnd()
    {
        OnClickedEvent?.Invoke(this, new ButtonClickedEventArgs { });
        BackgroundColor = Color.White;
    }
}
