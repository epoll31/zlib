using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib;

public class Image
{
    public Texture2D Texture { get; private set; }

    public Image(Texture2D texture)
    {
        Texture = texture;
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle bounds, Color color, float layerDepth)
    {
        spriteBatch.Draw(
            Texture,
            bounds,
            null,
            color,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            layerDepth
        );
    }
}
