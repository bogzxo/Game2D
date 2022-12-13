using Game2D;
using Game2D.World.Tiles;
using OpenTK.Mathematics;

public abstract class Tile
{
    public abstract TileTextureId TextureID { get; protected set; }
    public abstract TileId Id { get; protected set; }
    public abstract void Update(Vector2i pos, float dt);

    public static Tile IdToTile(TileId id)
    {
        switch (id)
        {
            case TileId.Dirt:
                return new DirtTile();
            default:
                return new EmptyTile();
        }
    }
    public static Tile None { get; private set; } = new EmptyTile();
}
