using Bogz.Logging;
using Game2D.Entitys.Components;
using Game2D.OpenGL;
using Game2D.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Game2D.Entitys
{
    public class ParticleSystemEntity : IEntity
    {
        struct ParticleInstanceData
        {
            public Vector2 Position { get; set; }
            public float Age { get; set; }
            public float LifeTime { get; set; }
            public Vector3 Color { get; set; }
            public static ParticleInstanceData None { get; } = new ParticleInstanceData();
            public static int SizeInBytes { get; } = Vector3.SizeInBytes + Vector2.SizeInBytes + sizeof(float) * 2;
        }

        private Shader shader;
        private float timer = 0.5f;
        private int instanceVBO, quadVAO, quadVBO;
        private ParticleInstanceData[] translations;

        public float MaximumAge { get; set; } = 1.0f;
        public float SpawnRate { get; set; } = 1.0f;
        public int Count { get; private set; }
        public bool IsSpawning { get; set; } = true;
        public Dictionary<Type, IEntityComponent> Components { get; set; }
        public EntityPhysicsComponent Physics { get; private set; }

        Random rand = new Random();
        public ParticleSystemEntity(int count)
        {
            Count = count;
            Physics = new EntityPhysicsComponent(new Vector2(0, 0), Vector2.Zero, 1.0f);

            Components = new Dictionary<Type, IEntityComponent>()
            {
                { typeof(EntityPhysicsComponent), Physics }
            };

            shader = Shader.CreateShader((ShaderType.VertexShader, "Assets/Shader/particle.vert"), (ShaderType.FragmentShader, "Assets/Shader/particle.frag"));
            translations = new ParticleInstanceData[count];
            for (int i = 0; i < Count; i++)
            {
                Vector2 translation = new Vector2((float)rand.NextDouble() * 2 - 1, (float)rand.NextDouble() * 2 - 1) + Physics.Position;
                translations[i] = new ParticleInstanceData
                {
                    Age = 0.0f,
                    LifeTime = 1.0f,
                    Position = translation
                };
            }

            // --------------------------------------
            instanceVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ParticleInstanceData.SizeInBytes * translations.Length, translations, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // set up vertex data (and buffer(s)) and configure vertex attributes
            // ------------------------------------------------------------------
            float scale = 0.05f;
            float[] quadVertices = {
                // positions     // colors
                -scale, scale,  1.0f, 0.0f,
                 scale, scale,  1.0f, 0.0f,
                scale, -scale,  1.0f, 1.0f,

                scale,  -scale,  1.0f, 0.0f,
                -scale, -scale,  1.0f, 1.0f,
                -scale,  scale,  0.0f, 1.0f
            };
            quadVAO = GL.GenVertexArray();
            quadVBO = GL.GenBuffer();
            GL.BindVertexArray(quadVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (2 * sizeof(float)));
            // also set instance data
            GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVBO); // this attribute comes from a different vertex buffer

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, ParticleInstanceData.SizeInBytes, 0);

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, ParticleInstanceData.SizeInBytes, Vector2.SizeInBytes);

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, ParticleInstanceData.SizeInBytes, Vector2.SizeInBytes + sizeof(float));


            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, false, ParticleInstanceData.SizeInBytes, Vector2.SizeInBytes + sizeof(float) * 2);


            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.VertexAttribDivisor(2, 1); // tell OpenGL this is an instanced vertex attribute.

        }

        ~ParticleSystemEntity()
        {

        }

        float tmr = 0.0f;
        public void Draw(float dt)
        {
            tmr += dt;

            for (int i = 0; i < Count; i++)
            {
                if (translations[i].LifeTime == 0 && IsSpawning)
                {
                    if (tmr > SpawnRate)
                    {
                        translations[i] = new ParticleInstanceData
                        {
                            Position = Physics.Position,
                            Age = 0.0f,
                            LifeTime = (float)rand.NextDouble() * MaximumAge,
                            Color = new Vector3((float)rand.NextDouble()) * 0.3f
                        };
                        tmr = 0.0f;
                    }
                }
                else
                {
                    translations[i].Age += dt * (float)rand.NextDouble();
                    translations[i].Position += new Vector2(
                        (float)(rand.NextDouble() - 0.5f) / 400.0f,
                        (float)rand.NextDouble() / 5000.0f
                        );
                }

                if (translations[i].Age > translations[i].LifeTime)
                    translations[i] = ParticleInstanceData.None;
            }

            shader.UseShader();

            var mvp = Matrix4.Transpose(GameManager.Instance.Camera.GetProjectionMatrix()) * Matrix4.Transpose(GameManager.Instance.Camera.GetViewMatrix());
            shader.Uniform1("timer", timer);
            shader.Matrix4("Matrix", ref mvp);

            GL.BindVertexArray(quadVAO);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 100);
            GL.BindVertexArray(0);

            shader.End();

        }

        float updateTimer = 0.0f;
        public void Update(float dt)
        {
            timer += dt;

            updateTimer += dt;

            if (updateTimer > 0.01f)
            {
                updateTimer = 0;
                GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, ParticleInstanceData.SizeInBytes * translations.Length, translations, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }


            foreach (var item in Components)
                item.Value.Update(dt);
        }
    }
}
