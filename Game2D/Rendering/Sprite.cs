using Bogz.Logging;
using Game2D.Data;
using Game2D.OpenGL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace Game2D.Rendering
{
    public class Sprite : IEntity, IDisposable
    {
        #region Private Members

        private readonly VertexBufferObject _vbo;
        private readonly uint[] _indices;
        private readonly Vertex[] _vertices;
        private Shader _shader;
        private Matrix4 _model;

        #endregion Private Members

        #region Public Members

        public bool IsFlipped { get; set; }
        public Texture Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; private set; }
        public RectangleF TextureRectangle { get; set; }

        #endregion Public Members

        public Dictionary<Type, IEntityComponent> Components { get; set; } = new Dictionary<Type, IEntityComponent>();

        public Sprite(Texture tex, Vector2 size, Shader? sprShader = null)
        {
            Size = size;

            if (sprShader == null)
            {
                Logger.Instance.Log(LogLevel.Info, "Sprite constructed without any shader, using default shader...");
                _shader = Shader.CreateShader((ShaderType.VertexShader, "Assets/Shader/basic.vert"), (ShaderType.FragmentShader, "Assets/Shader/basic.frag"));
            }
            else _shader = sprShader;

            Logger.Instance.Log(LogLevel.Info, $"Sprite constructed using shader({_shader.Program})");

            Texture = tex;
            _vbo = new VertexBufferObject();

            _indices = new uint[] {
                0, 1, 2,
                0, 2, 3
            };

            _vertices = new Vertex[] {
                new Vertex(new Vector3(-size.X, -size.Y,0), new Vector2(0,1)),
                new Vertex(new Vector3(size.X, -size.Y,0), new Vector2(1,1)),
                new Vertex(new Vector3(size.X, size.Y,0), new Vector2(1,0)),
                new Vertex(new Vector3(-size.X, size.Y,0), new Vector2(0,0))
            };

            _vbo.PushVertexAttribPointer(0, 3, VertexAttribPointerType.Float, Vertex.SizeInBytes, 0);
            _vbo.PushVertexAttribPointer(1, 2, VertexAttribPointerType.Float, Vertex.SizeInBytes, Vector3.SizeInBytes);

            _vbo.BufferData(_vertices, Vertex.SizeInBytes);
            _vbo.BufferIndices(_indices);

            _model = Matrix4.Identity;
        }

        ~Sprite()
        {
            Dispose(false);
        }

        public void Update(float dt)
        {
            _model = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
        }

        public void UpdateTextureRectangle(bool usePlayerCamera = true)
        {
            var imageSize = new Vector2(TextureRectangle.Width, TextureRectangle.Height);
            var texturePosition = new Vector2(TextureRectangle.X, TextureRectangle.Y);
            _shader.UseShader();

            _shader.Uniform2("imageSize", ref imageSize);
            _shader.Uniform2("texturePosition", ref texturePosition);
            var mvp = usePlayerCamera ? Matrix4.Transpose(GameManager.Instance.Camera.GetProjectionMatrix()) * Matrix4.Transpose(GameManager.Instance.Camera.GetViewMatrix()) * Matrix4.Transpose(_model) : Matrix4.Identity;

            _shader.Matrix4("mvp", ref mvp);
            _shader.Uniform1("flipped", IsFlipped ? 1 : 0);
            _shader.End();
        }

        public void Draw(float dt)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTextureUnit(0, Texture.GLTexture);

            _shader.UseShader();
            _shader.Uniform1("inputTexture", 0);

            _vbo.Use();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            _vbo.End();
            _shader.End();
        }

        private void ReleaseUnmanagedResources()
        {
            _vbo.Dispose();
            _shader.Dispose();
            Texture.Dispose();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                // Release Managed Resources
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}