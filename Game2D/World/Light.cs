using OpenTK.Mathematics;

namespace Game2D.World
{
    public struct Light
    {
        public Vector3 Color { get; set; }
        public Vector2 Position { get; set; }
        public float Intensity { get; set; }
    }
}