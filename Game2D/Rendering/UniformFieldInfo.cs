using OpenTK.Graphics.OpenGL4;

namespace Game2D.Rendering
{
    public partial class Shader
    {
        public struct UniformFieldInfo
        {
            public int Location;
            public string Name;
            public int Size;
            public ActiveUniformType Type;
        }
    }
}