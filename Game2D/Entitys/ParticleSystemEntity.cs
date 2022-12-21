using Game2D.Entities.Components;
using Game2D.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game2D.Entities
{
    public class ParticleSystemEntity : IEntity
    {
        private struct ParticleInstanceData
        {
            public Vector2 Position { get; set; }
            public float Age { get; set; }
            public float LifeTime { get; set; }
            public Vector3 Color { get; set; }
            public static ParticleInstanceData None { get; } = new ParticleInstanceData();
            public static int SizeInBytes { get; } = Vector3.SizeInBytes + Vector2.SizeInBytes + sizeof(float) * 2;
        }

        private readonly Shader _shader;
        private float _timer = 0.5f;
        private readonly int _instanceVbo, _quadVao, _quadVbo;
        private readonly ParticleInstanceData[] _translations;

        public float MaximumAge { get; set; } = 1.0f;
        public float SpawnRate { get; set; } = 1.0f;
        public int Count { get; private set; }
        public bool IsSpawning { get; set; } = true;
        public Dictionary<Type, IEntityComponent> Components { get; set; }
        public EntityPhysicsComponent Physics { get; private set; }

        private Random rand = new Random();

        public ParticleSystemEntity(int count)
        {
            Count = count;
            Physics = new EntityPhysicsComponent(new Vector2(0, 0), Vector2.Zero, 1.0f);

            Components = new Dictionary<Type, IEntityComponent>()
            {
                { typeof(EntityPhysicsComponent), Physics }
            };

            _shader = Shader.CreateShader((ShaderType.VertexShader, "Assets/Shader/particle.vert"), (ShaderType.FragmentShader, "Assets/Shader/particle.frag"));
            _translations = new ParticleInstanceData[count];
            for (int i = 0; i < Count; i++)
            {
                Vector2 translation = new Vector2((float)rand.NextDouble() * 2 - 1, (float)rand.NextDouble() * 2 - 1) + Physics.Position;
                _translations[i] = new ParticleInstanceData
                {
                    Age = 0.0f,
                    LifeTime = 1.0f,
                    Position = translation
                };
            }

            // --------------------------------------
            _instanceVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, ParticleInstanceData.SizeInBytes * _translations.Length, _translations, BufferUsageHint.StaticDraw);
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
            _quadVao = GL.GenVertexArray();
            _quadVbo = GL.GenBuffer();
            GL.BindVertexArray(_quadVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _quadVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (2 * sizeof(float)));
            // also set instance data
            GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVbo); // this attribute comes from a different vertex buffer

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

        private float _tmr = 0.0f;

        public void Draw(float dt)
        {
            _tmr += dt;

            for (int i = 0; i < Count; i++)
            {
                if (_translations[i].LifeTime == 0 && IsSpawning)
                {
                    if (_tmr > SpawnRate)
                    {
                        _translations[i] = new ParticleInstanceData
                        {
                            Position = Physics.Position,
                            Age = 0.0f,
                            LifeTime = (float)rand.NextDouble() * MaximumAge,
                            Color = new Vector3((float)rand.NextDouble()) * 0.3f
                        };
                        _tmr = 0.0f;
                    }
                }
                else
                {
                    _translations[i].Age += dt * (float)rand.NextDouble();
                    _translations[i].Position += new Vector2(
                        (float)(rand.NextDouble() - 0.5f) / 400.0f,
                        (float)rand.NextDouble() / 5000.0f
                        );
                }

                if (_translations[i].Age > _translations[i].LifeTime)
                    _translations[i] = ParticleInstanceData.None;
            }

            _shader.UseShader();

            var mvp = Matrix4.Transpose(GameManager.Instance.Camera.GetProjectionMatrix()) * Matrix4.Transpose(GameManager.Instance.Camera.GetViewMatrix());
            _shader.Uniform1("timer", _timer);
            _shader.Matrix4("Matrix", ref mvp);

            GL.BindVertexArray(_quadVao);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 100);
            GL.BindVertexArray(0);

            _shader.End();
        }

        private float _updateTimer = 0.0f;

        public void Update(float dt)
        {
            _timer += dt;

            _updateTimer += dt;

            if (_updateTimer > 0.01f)
            {
                _updateTimer = 0;
                GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, ParticleInstanceData.SizeInBytes * _translations.Length, _translations, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            foreach (var item in Components)
                item.Value.Update(dt);
        }
    }
}