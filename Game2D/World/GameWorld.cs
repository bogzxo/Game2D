using Bogz.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Game2D.World
{
    public class GameWorld
    {
        #region Public Members

        public int Width { get; private set; } = 10;
        public Chunk[] Chunks { get; private set; }
        public GameWorldGenerator GameWorldGenerator { get; }
        public ConcurrentBag<IEntity> Entities { get; private set; }

        #endregion Public Members

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
            GameWorldGenerator = new GameWorldGenerator();
            Chunks = new Chunk[Width];
            for (int i = 0; i < Chunks.Length; i++)
                Chunks[i] = new Chunk(i);

            Entities = new ConcurrentBag<IEntity>();

            GenerateWorld();

            // SINGLE THREADED
            // 10 Chunks: 5s
            // 100 Chunks: 22s

            // MULTI THREADED
            // 10 Chunks 25ms
            // 100 Chunks 33ms

            // fuck
        }

        public void GenerateWorld()
        {
            GameWorldGenerator.Regenerate();

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < Chunks.Length; i++)
                Chunks[i].GenerateChunk(GameWorldGenerator);

            for (int i = 0; i < Chunks.Length; i++)
                Chunks[i].UploadVertexBuffer();

            sw.Stop();
            Logger.Instance.Log(LogLevel.Error, $"Total Chunk Generation Time {sw.ElapsedMilliseconds}ms");
        }

        private float timer = 0.0f;

        public void Update(float dt)
        {
            foreach (var item in Entities)
                item.Update(dt);
        }

        public void AddEntity(in IEntity entity)
        {
            if (Entities.Contains(entity)) return;
            Entities.Add(entity);
        }
    }
}