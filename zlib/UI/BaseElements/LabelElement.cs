using Microsoft.Xna.Framework.Graphics;

namespace zlib.UI;

public class LabelElement : Element
{
    public string Text { get; set; } = "";
    public Font Font { get; set; }
    public FontProperties FontProperties { get; set; } = new();

    public LabelElement(Font font, string text = "label")
    {
        Font = font;
        Text = text;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        FontProperties newFP = FontProperties;
        newFP.LayerDepth = LayerDepth;
        Font.Draw(spriteBatch, Text, Transform.Bounds, newFP);
    }
}
