namespace Game2D.World
{
    public class WorldLayer
    {
        public int Width { get; private set; }
        public Chunk[] Chunks { get; private set; }

        public WorldLayer(int width)
        {
            Width = width;

            Chunks = new Chunk[Width];
            for (int i = 0; i < width; i++)
                Chunks[i] = new Chunk(i);
        }
    }
}