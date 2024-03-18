using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace zlib;

public class GridTileSet : Image
{
    public uint TileSize { get; }
    public uint Gap { get; set; }

    public GridTileSet(Texture2D texture, uint tileSize = 32, uint gap = 0)
        : base(texture)
    {
        TileSize = tileSize;
        Gap = gap;
    }

    public Rectangle GetSourceRectangle(uint i, uint j, uint width = 1, uint height = 1)
    {
        return new Rectangle(
            (int)(i * TileSize + i * Gap),
            (int)(j * TileSize + j * Gap),
            (int)(width * TileSize + (width - 1) * Gap),
            (int)(height * TileSize + (height - 1) * Gap)
        );
    }

    public void Draw(SpriteBatch spriteBatch, uint i, uint j, Vector2 position)
    {
        Rectangle sourceRectangle = GetSourceRectangle(i, j);
        spriteBatch.Draw(Texture, position, sourceRectangle, Color.White);
    }
}

public class AnimatedGridTileSet : GridTileSet, IAnimatable
{
    private Dictionary<string, (uint i, uint j)> frames = new();
    public IEnumerable<string> Frames => frames.Keys;

    public AnimatedGridTileSet(
        Texture2D texture,
        uint tileSize = 32,
        uint gap = 0,
        params (string frame, uint i, uint j)[] frames
    )
        : base(texture, tileSize, gap)
    {
        foreach ((string frame, uint i, uint j) in frames)
        {
            AddFrame(frame, i, j);
        }
    }

    public void AddFrame(string frame, uint i, uint j)
    {
        frames.Add(frame, (i, j));
    }

    public Rectangle GetSourceRectangle(
        string frame,
        uint i,
        uint j,
        uint width = 1,
        uint height = 1
    )
    {
        (uint fi, uint fj) = frames[frame];
        return base.GetSourceRectangle(fi + i, fj + j, width, height);
    }

    public Rectangle GetSourceRectangle(
        uint frame,
        uint i,
        uint j,
        uint width = 1,
        uint height = 1
    ) => GetSourceRectangle(Frames.ElementAt((int)frame), i, j, width, height);
}
