using Microsoft.Xna.Framework.Graphics;

namespace zlib.UI;

public class RootElement : Element
{
    public RootElement(params Element[] children)
        : base(children)
    {
        (this as IClickable).UnregisterClickable();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
