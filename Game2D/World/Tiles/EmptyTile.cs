using Game2D.World.Tiles;
using OpenTK.Mathematics;

public class EmptyTile : Tile
{
    public override TileId Id { get; protected set; } = TileId.None;
    public override TileTextureId TextureID { get; protected set; } = TileTextureId.None;

    public override void Update(Vector2i pos, float dt)
    {
    }
}