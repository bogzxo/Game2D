using Game2D.OpenGL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game2D.Rendering.UI
{
    public class UISprite
    {
        private static RenderRectangle renderRect;
        public Shader Shader { get; set; }
        public Texture Texture { get; set; }

        public Vector2 Scale = Vector2.One;
        public Vector2 Position = Vector2.Zero;

        private Matrix4 mat;

        public UISprite(Shader shader, Texture texture)
        {
            renderRect ??= new RenderRectangle();

            Shader = shader;
            Texture = texture;
        }

        public void Update(float dt)
        {
            mat = Matrix4.Identity
              * Matrix4.Transpose(Matrix4.CreateTranslation(Position.X, Position.Y, 0)) * Matrix4.Transpose(Matrix4.CreateScale(new Vector3(Scale.X * (Texture.Width / Texture.Height) / 2.0f, Scale.Y, 1)));
        }

        public void Draw(float dt)
        {
            Shader.UseShader();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTextureUnit(0, Texture.GLTexture);

            Shader.UseShader();
            Shader.Uniform1("inputTexture", 0);
            Shader.Matrix4("mvp", ref mat);

            renderRect.Draw(dt);

            Shader.End();
        }
    }
}
