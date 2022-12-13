using Bogz.Logging;
using Game2D.Data;
using Game2D.OpenGL;
using Game2D.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.GameScreens.Graphics
{
    public class Sprite : IEntity
    {
        #region Private Members
        private VertexBufferObject vbo;
        private uint[] indices;
        private Vertex[] vertices;
        private Shader shader;
        private Matrix4 model;
        #endregion

        #region Public Members
        public bool IsFlipped { get; set; }
        public Texture Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; private set; }
        public RectangleF TextureRectangle { get; set; }
        #endregion

        public Dictionary<Type, IEntityComponent> Components { get; set; } = new Dictionary<Type, IEntityComponent>();

        public Sprite(Texture tex, Vector2 size, Shader sprShader = null)
        {
            Size = size;

            if (sprShader == null)
            {
                Logger.Instance.Log(LogLevel.Info, "Sprite constructed without any shader, using default shader...");
                shader = Shader.CreateShader((ShaderType.VertexShader, "Assets/Shader/basic.vert"), (ShaderType.FragmentShader, "Assets/Shader/basic.frag"));
            }
            else shader = sprShader;

            Logger.Instance.Log(LogLevel.Info, $"Sprite constructed using shader({shader.Program})");

            Texture = tex;
            vbo = new VertexBufferObject();

            indices = new uint[] {
                0, 1, 2,
                0, 2, 3
            };

            vertices = new Vertex[] {
                new Vertex(new Vector3(-size.X, -size.Y,0), new Vector2(0,1)),
                new Vertex(new Vector3(size.X, -size.Y,0), new Vector2(1,1)),
                new Vertex(new Vector3(size.X, size.Y,0), new Vector2(1,0)),
                new Vertex(new Vector3(-size.X, size.Y,0), new Vector2(0,0))
            };

            vbo.PushVertexAttribPointer(0, 3, VertexAttribPointerType.Float, Vertex.SizeInBytes, 0);
            vbo.PushVertexAttribPointer(1, 2, VertexAttribPointerType.Float, Vertex.SizeInBytes, Vector3.SizeInBytes);

            vbo.BufferData(vertices, Vertex.SizeInBytes);
            vbo.BufferIndices(indices);

            model = Matrix4.Identity;
        }

        public void Update(float dt)
        {
            model = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
        }

        public void Draw(float dt)
        {
            var mvp = Matrix4.Transpose(GameManager.Instance.Camera.GetProjectionMatrix()) * Matrix4.Transpose(GameManager.Instance.Camera.GetViewMatrix()) * Matrix4.Transpose(model);
            var imageSize = new Vector2(TextureRectangle.Width, TextureRectangle.Height);
            var texturePosition = new Vector2(TextureRectangle.X, TextureRectangle.Y);

            shader.UseShader();
            shader.Matrix4("mvp", ref mvp);
            shader.Uniform2("imageSize", ref imageSize);
            shader.Uniform2("texturePosition", ref texturePosition);

            shader.Uniform1("flipped", IsFlipped ? 1 : 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTextureUnit(0, Texture.GLTexture);

            shader.Uniform1("inputTexture", 0);

            vbo.Use();
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            vbo.End();
            shader.End();
        }
    }
}
