using OpenTK.Mathematics;

namespace Game2D.Data;

public struct Vertex
{
    public static int SizeInBytes = Vector3.SizeInBytes + Vector2.SizeInBytes;

    public Vertex(Vector3 position, Vector2 texCoord)
    {
        Position = position;
        TexCoord = texCoord;
    }

    public Vector3 Position { get; set; }
    public Vector2 TexCoord { get; set; }
}