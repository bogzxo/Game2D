using Game2D;
using Game2D.World.Tiles;
using OpenTK.Mathematics;

public class DirtTile : Tile
{
    public override TileId Id { get; protected set; } = TileId.Dirt;
    public override TileTextureId TextureID { get; protected set; } = TileTextureId.Dirt;
    public bool HasGrass
    {
        get; set;
    } = false;

    public override void Update(Vector2i pos, float dt)
    {
        if (GameManager.Instance.GameWorld[pos.X, pos.Y + 1].Id == TileId.None)
        {
            HasGrass = true;
            TextureID = TileTextureId.GrassTop;
        }
    }
}