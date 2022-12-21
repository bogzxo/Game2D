using OpenTK.Mathematics;

namespace Game2D.Rendering;

public struct GLLight
{
    public Vector3 Position;

    public const float Constant = 1.0f;
    public const float Linear = 0.09f;
    public const float Quadratic = 0.032f;

    public GLLight(Vector3 position)
    {
        Position = position;
    }
}