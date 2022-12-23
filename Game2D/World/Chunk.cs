using Bogz.Logging;
using Game2D.Data;
using Game2D.OpenGL;
using Game2D.World.Tiles;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game2D.World
{
    public class TileMesh : IDisposable
    {
        private bool disposedValue;
        private VertexBufferObject vbo;
        private uint indiciesCount = 0;

        public List<Vertex> Vertices { get; set; } = new();
        public List<uint> Indices { get; set; } = new();

        public TileMesh()
        {
            vbo = new VertexBufferObject();
            vbo.PushVertexAttribPointer(0, 3, VertexAttribPointerType.Float, Vertex.SizeInBytes, 0);
            vbo.PushVertexAttribPointer(1, 2, VertexAttribPointerType.Float, Vertex.SizeInBytes, Vector3.SizeInBytes);
        }
        void updateIndicies()
        {
            Indices.AddRange(new uint[] {
                            indiciesCount + 0, indiciesCount + 1, indiciesCount + 2,
                            indiciesCount + 0, indiciesCount + 2, indiciesCount + 3
                        });

            indiciesCount += 4;
        }
        public void AddVertices(Vertex[] verts)
        {
            Vertices.AddRange(verts);
            updateIndicies();
        }

        public void Clear()
        {
            Vertices.Clear();
            Indices.Clear();
        }

        public void FinalizeAndUpload()
        {
            vbo.BufferData(Vertices.ToArray(), Vertex.SizeInBytes);
            vbo.BufferIndices(Indices.ToArray());
        }

        public void Draw()
        {
            vbo.Use();
            GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
            vbo.End();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                vbo.Dispose();
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    public class Chunk
    {
        #region Public Members

        public static int Width { get; } = 32;
        public static int Height { get; } = 32;
        public static Vector2 Size { get => new Vector2(Width, Height); }
        public int Position { get; set; }
        public Tile[,] Tiles { get; set; }
        public TileMesh TileMesh { get; set; }
        public TileMesh VegetationMesh { get; set; }
        #endregion Public Members

        #region Private Members

        private const int tilesetSize = 10;
        private const float texOffset = 1.0f / tilesetSize;

        #endregion Private Members

        public Chunk(int position)
        {
            TileMesh = new TileMesh();
            VegetationMesh = new TileMesh();

            Tiles = new Tile[Width, Height];
            Position = position;
        }

        public void GenerateChunk(GameWorldGenerator gameWorldGenerator)
        {
            TileMesh.Clear();
            VegetationMesh.Clear();

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Tiles[x, y] = Tile.None;

            gameWorldGenerator.Generate(this);

            generateMesh();
        }

        private void generateMesh()
        {
            Random rand = new Random(5);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y].Id > 0)
                    {
                        Tiles[x, y].Update(new Vector2i(x + Position * Width, y), 0);

                        int textureId = (int)Tiles[x, y].TextureID;

                        Vector2 texCoords = new Vector2((int)(textureId % tilesetSize), (int)(textureId / tilesetSize)) * texOffset;
                        Vector3 position = new Vector3(x + Position * Width, y, 0);

                        switch (Tiles[x, y])
                        {
                            case DirtTile tile:
                                if (!tile.HasGrass || rand.NextDouble() < 0.5) break;

                                int vegTextureId = (int)TileTextureId.Vegetation + (rand.Next(3));
                                Vector2 vegTexCoords = new Vector2((int)(vegTextureId % tilesetSize), (int)(vegTextureId / tilesetSize)) * texOffset;

                                VegetationMesh.AddVertices(new[] {
                                    new Vertex(new Vector3(position.X + 0, position.Y + 0 + 0.9f, 0), vegTexCoords + new Vector2(0, texOffset)),
                                    new Vertex(new Vector3(position.X + 1, position.Y + 0 + 0.9f, 0), vegTexCoords + new Vector2(texOffset, texOffset)),
                                    new Vertex(new Vector3(position.X + 1, position.Y + 1 + 0.9f, 0), vegTexCoords + new Vector2(texOffset, 0)),
                                    new Vertex(new Vector3(position.X + 0, position.Y + 1 + 0.9f, 0), vegTexCoords + new Vector2(0, 0))
                                });
                                break;
                        }

                        TileMesh.AddVertices(new[] {
                            new Vertex(new Vector3(position.X + 0, position.Y + 0, 0), texCoords + new Vector2(0, texOffset)),
                            new Vertex(new Vector3(position.X + 1, position.Y + 0, 0), texCoords + new Vector2(texOffset, texOffset)),
                            new Vertex(new Vector3(position.X + 1, position.Y + 1, 0), texCoords + new Vector2(texOffset, 0)),
                            new Vertex(new Vector3(position.X + 0, position.Y + 1, 0), texCoords + new Vector2(0, 0))
                        });
                    }
                }
            }

            Logger.Instance.Log(LogLevel.Success, $"Generated chunk with {TileMesh.Vertices.Count} vertices.");
        }

        public void UploadVertexBuffer()
        {
            TileMesh.FinalizeAndUpload();
            VegetationMesh.FinalizeAndUpload();
        }


        public Tile GetTile(int x, int y)
        {
            if (x > Width - 1 || x < 0 || y > Height - 1 || y < 0) return Tile.None;
            return Tiles[x, y];
        }
    }
}