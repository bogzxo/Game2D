using Bogz.Logging;
using Game2D.Data;
using Game2D.OpenGL;
using Game2D.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Game2D.World
{
    public class GameWorldRenderer
    {
        private GameWorld world;
        private Texture texture;
        private Shader shader, flatShader;
        private Matrix4 model;

        private VertexBufferObject vbo;
        private uint[] indices;
        private Vertex[] vertices;
        private FrameBufferObject fbo0;

        public GameWorldRenderer(GameWorld world, string path)
        {
            this.world = world;
            texture = new Texture(new System.Drawing.Bitmap(path), false, false);

            shader = Shader.CreateShader((ShaderType.VertexShader, "Assets/Shader/tileMapRenderer.vert"), (ShaderType.FragmentShader, "Assets/Shader/tileMapRenderer.frag"));

            flatShader = Shader.CreateShader((ShaderType.VertexShader, "Assets/Shader/tileMapRenderer.vert"), (ShaderType.FragmentShader, "Assets/Shader/shadows.frag"));

            model = Matrix4.Identity;

            fbo0 = new FrameBufferObject(GameManager.Instance.Size.X, GameManager.Instance.Size.Y);

            vbo = new VertexBufferObject();
            indices = new uint[] {
                0, 1, 2,
                0, 2, 3
            };

            vertices = new Vertex[] {
                new Vertex(new Vector3(-1,-1,0), new Vector2(0,0)),
                new Vertex(new Vector3(1,-1,0), new Vector2(1,0)),
                new Vertex(new Vector3(1,1,0), new Vector2(1,1)),
                new Vertex(new Vector3(-1,1,0), new Vector2(0,1))
            };

            vbo.PushVertexAttribPointer(0, 3, VertexAttribPointerType.Float, Vertex.SizeInBytes, 0);
            vbo.PushVertexAttribPointer(1, 2, VertexAttribPointerType.Float, Vertex.SizeInBytes, Vector3.SizeInBytes);

            vbo.BufferData(vertices, Vertex.SizeInBytes);
            vbo.BufferIndices(indices);
        }

        public void BeginDraw(float dt)
        {
            fbo0.Use();
            GL.DrawBuffers(4, new[] { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3 });
            GL.ClearColor(new Color4(0, 0, 0, 0));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Viewport(0, 0, fbo0.Width, fbo0.Height);

            shader.UseShader();
            var mvp = Matrix4.Transpose(GameManager.Instance.Camera.GetProjectionMatrix()) * Matrix4.Transpose(GameManager.Instance.Camera.GetViewMatrix()) * Matrix4.Transpose(model);
            shader.Matrix4("mvp", ref mvp);


            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTextureUnit(0, texture.GLTexture);

            shader.Uniform1("inputTexture", 0);

            foreach (var chunk in world.Chunks)
                chunk.Draw(dt);

            shader.End();
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            fbo0.End();
        }

        public void EndDraw()
        {
            GL.Viewport(0, 0, GameManager.Instance.Size.X, GameManager.Instance.Size.Y);
            flatShader.UseShader();

            Vector2 player = new Vector2(0.5f);

            flatShader.Uniform2("t0", ref player);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTextureUnit(0, fbo0.ColorTexture);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTextureUnit(1, fbo0.NormalTexture);

            flatShader.Uniform1("inputTexture", 0);
            flatShader.Uniform1("shadowTexture", 1);

            var mat = Matrix4.Identity;

            flatShader.Matrix4("mvp", ref mat);


            vbo.Use();
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            vbo.End();
            flatShader.End();
        }
    }
}
