using Game2D.Data;
using Game2D.Entities;
using Game2D.OpenGL;
using Game2D.Rendering;
using Game2D.World;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.GameScreens
{
    public class GamePlayGameScreen : IGameScreen
    {
        private List<IEntity> entities;
        private PlayerEntity player;
        private GameWorld world;
        private GameWorldRenderer worldRenderer;
        private Shader backgroundShader, postProcessingShader;
        private VertexBufferObject vbo;
        private FrameBufferObject fbo;
        private uint[] indices;
        private Vertex[] vertices;

        float iTime = 0;

        public void Initialize()
        {
            player = new PlayerEntity();
            entities = new List<IEntity>
            {
                player
            };
            GameManager.Instance.Camera = new Camera(CameraType.Perspective, new Vector3(0, 0, 15), 16.0f / 9.0f);

            world = new GameWorld();
            worldRenderer = new GameWorldRenderer(world, "Assets/Map/tilesheet.png");

            //background = new Texture(1920, 1080);
            //backgroundSprite = new Sprite(background);
            //backgroundSprite.Position = new Vector2(0, 0);

            backgroundShader = Shader.CreateShader((ShaderType.VertexShader, "Assets/Shader/basic.vert"), (ShaderType.FragmentShader, "Assets/Shader/clouds.frag"));
            postProcessingShader = Shader.CreateShader((ShaderType.VertexShader, "Assets/Shader/basic.vert"), (ShaderType.FragmentShader, "Assets/Shader/post.frag"));

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

            fbo = new FrameBufferObject(1920, 1080);

            GL.ClearColor(Color4.Aqua);
        }
        public void Draw(float dt)
        {
            fbo.Use();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, GameManager.Instance.Size.X, GameManager.Instance.Size.Y);

            backgroundShader.UseShader();

            var mat = Matrix4.Identity;
            var ires = new Vector2(GameManager.Instance.Size.X, GameManager.Instance.Size.Y);

            backgroundShader.Matrix4("mvp", ref mat);
            backgroundShader.Uniform1("iTime", iTime);
            backgroundShader.Uniform2("iResolution", ref ires);

            vbo.Use();
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            vbo.End();
            backgroundShader.End();

            worldRenderer.BeginDraw(dt);
            fbo.Use();

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            worldRenderer.EndDraw();

            foreach (var item in entities)
                item.Draw(dt);

            fbo.End();
            
            postProcessingShader.UseShader();
            postProcessingShader.Matrix4("mvp", ref mat);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTextureUnit(0, fbo.ColorTexture);

            postProcessingShader.Uniform1("inputTexture", 0);

            vbo.Use();
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            vbo.End();
        }

        public void Update(float dt)
        {
            iTime += dt;

            world.Update(dt);

            foreach (var item in entities)
                item.Update(dt);
        }
    }
}
