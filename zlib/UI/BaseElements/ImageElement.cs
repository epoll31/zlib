using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib.UI;

public class ImageElement : Element
{
    public Image Image { get; set; }

    public ImageElement(params Element[] children)
        : base(children)
    {
        Image = AssetManager.Instance.GetImage("blank");
    }

    public ImageElement(Image image, params Element[] children)
        : base(children)
    {
        Image = image;
        Background = null;
        BackgroundColor = Color.White;
    }

    protected override void DrawBackground(SpriteBatch spriteBatch)
    {
        base.DrawBackground(spriteBatch);
        Image?.Draw(spriteBatch, Transform.Bounds, BackgroundColor, LayerDepth);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
