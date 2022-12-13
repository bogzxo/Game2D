using Bogz.Logging;
using Game2D.World.Tiles;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace Game2D.World
{
    public class GameWorld
    {
        #region Public Members
        public int Width { get; private set; } = 10;
        public Chunk[] Chunks { get; private set; }
        #endregion

        #region Internal Members
        #endregion


        public List<Light> Lights { get; private set; } = new List<Light>();
        public Tile this[int x, int y]
        {
            get
            {
                int chunkX = x / Chunk.Width;
                int chunkY = y / Chunk.Height;

                int fx = x % Chunk.Width;
                int fy = y % Chunk.Height;

                if (chunkX > Width - 1 || x < 0 || y < 0 || chunkY < 0 || chunkY > 0 || fy > Chunk.Height - 1 || fx > Chunk.Width - 1) return Tile.None;

                return Chunks[chunkX].Tiles[fx, fy];
            }
        }

        public GameWorld()
        {
            GameManager.Instance.GameWorld = this;

            Stopwatch sw = Stopwatch.StartNew();

            Chunks = new Chunk[Width];

            for (int i = 0; i < Chunks.Length; i++)
                Chunks[i] = new Chunk(i);

            Parallel.For(0, Width, (i) => Chunks[i].GenerateChunk());

            for (int i = 0; i < Chunks.Length; i++)
                Chunks[i].UploadVertexBuffer();

            sw.Stop();

            // SINGLE THREADED
            // 10 Chunks: 5s
            // 100 Chunks: 22s

            // MULTI THREADED
            // 10 Chunks 25ms
            // 100 Chunks 33ms

            // fuck

            Logger.Instance.Log(LogLevel.Error, $"Total Chunk Generation Time {sw.ElapsedMilliseconds}ms");
        }
        float timer = 0.0f;
        public void Update(float dt)
        {
            //timer += dt;
            //if (timer > 0.1f)
            //{
            //    timer = 0.0f;

            //    for (int i = 0; i < Chunks.Length; i++)
            //        Chunks[i].GenerateMesh();
            //}



        }
    }
}