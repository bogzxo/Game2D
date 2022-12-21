using Bogz.Logging;
using Game2D.Data;
using Game2D.OpenGL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game2D.World
{
    public class Chunk
    {
        #region Public Members

        public static int Width { get; } = 32;
        public static int Height { get; } = 32;
        public static Vector2 Size { get => new Vector2(Width, Height); }
        public int Position { get; set; }
        public Tile[,] Tiles { get; set; }

        #endregion Public Members

        #region Private Members

        private VertexBufferObject vbo;
        private Vertex[] verticesArray;
        private uint[] indicesArray;
        private const int tilesetSize = 10;
        private const float texOffset = 1.0f / tilesetSize;

        #endregion Private Members

        public Chunk(int position)
        {
            Tiles = new Tile[Width, Height];
            Position = position;

            vbo = new VertexBufferObject();
            vbo.PushVertexAttribPointer(0, 3, VertexAttribPointerType.Float, Vertex.SizeInBytes, 0);
            vbo.PushVertexAttribPointer(1, 2, VertexAttribPointerType.Float, Vertex.SizeInBytes, Vector3.SizeInBytes);
        }

        public void GenerateChunk(GameWorldGenerator gameWorldGenerator)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Tiles[x, y] = Tile.None;

            gameWorldGenerator.Generate(this);

            generateMesh();
        }

        private void generateMesh()
        {
            List<Vertex> vertices = new();
            List<uint> indices = new();

            uint indiciesCount = 0;

            void updateIndicies()
            {
                indices.AddRange(new uint[] {
                            indiciesCount + 0, indiciesCount + 1, indiciesCount + 2,
                            indiciesCount + 0, indiciesCount + 2, indiciesCount + 3
                        });

                indiciesCount += 4;
            }

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

                        vertices.AddRange(new[] {
                            new Vertex(new Vector3(position.X + 0, position.Y + 0, 0), texCoords + new Vector2(0, texOffset)),
                            new Vertex(new Vector3(position.X + 1, position.Y + 0, 0), texCoords + new Vector2(texOffset, texOffset)),
                            new Vertex(new Vector3(position.X + 1, position.Y + 1, 0), texCoords + new Vector2(texOffset, 0)),
                            new Vertex(new Vector3(position.X + 0, position.Y + 1, 0), texCoords + new Vector2(0, 0))
                        });

                        updateIndicies();
                    }
                }
            }

            this.verticesArray = vertices.ToArray();
            this.indicesArray = indices.ToArray();

            Logger.Instance.Log(LogLevel.Success, $"Generated chunk with {vertices.Count} vertices.");
        }

        public void UploadVertexBuffer()
        {
            vbo.BufferData(this.verticesArray, Vertex.SizeInBytes);
            vbo.BufferIndices(this.indicesArray);
        }

        public void Draw(float dt)
        {
            vbo.Use();
            GL.DrawElements(PrimitiveType.Triangles, indicesArray.Length, DrawElementsType.UnsignedInt, 0);
            vbo.End();
        }

        public Tile GetTile(int x, int y)
        {
            if (x > Width - 1 || x < 0 || y > Height - 1 || y < 0) return Tile.None;
            return Tiles[x, y];
        }

        ~Chunk()
        {
            vbo.Dispose();
        }
    }
}