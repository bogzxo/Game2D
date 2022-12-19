using Game2D.World.Generation;
using Game2D.World.Tiles;

namespace Game2D.World
{
    public class GameWorldGenerator
    {
        private float[,] _world;
        private readonly Noise _surfaceNoise = new Noise();
        private readonly Noise _cavernNoise = new Noise();
        public WorldGenerationParameters Parameters = WorldGenerationParameters.Default;

        public GameWorldGenerator()
        {
            _surfaceNoise.SetFrequency(Parameters.BaseSurfaceFrequency);
            _cavernNoise.SetFrequency(Parameters.BaseCavernFrequency);

            Regenerate();
        }

        public void Regenerate()
        {
            _world = new float[GameManager.Instance.GameWorld.Width * Chunk.Width, Chunk.Height];

            var heightMapSamples = new List<int>();

            for (int i = 0; i < GameManager.Instance.GameWorld.Width; i++)
                heightMapSamples.AddRange(GenerateHeightSamples(i, Chunk.Width, Chunk.Height - 1));

            for (int x = 0; x < _world.GetLength(0); x++)
            {
                for (int y = heightMapSamples[x]; y < Chunk.Height; y++)
                {
                    double yScaled = (Chunk.Height - 1 - y) / Chunk.Height + 0.1f;

                    double sample = 0.7;

                    //for (int i = 1; i < 10; i++)
                    sample += Math.Clamp(_cavernNoise.GetSimplex(x, y, 0), -0.2f, 0.2f);


                    _world[x, y] = (float)sample;
                }
            }

            _world = ApplyTerrainFilters(_world);
        }

        public float[,] ApplyTerrainFilters(in float[,] noiseMap)
        {
            var erodedNoiseMap = noiseMap;

            for (int i = 0; i < 10 - Parameters.ErosionPasses; i++)
            {
                for (int x = 1; x < noiseMap.GetLength(0) - 1; x++)
                {
                    for (int y = 1; y < noiseMap.GetLength(1) - 1; y++)
                    {
                        // Apply erosion by setting the noise value at this point to the average of its neighbors
                        erodedNoiseMap[x, y] = (erodedNoiseMap[x - 1, y] + erodedNoiseMap[x + 1, y] + erodedNoiseMap[x, y - 1] + erodedNoiseMap[x, y + 1] + Parameters.ErosionBias / 10.0f) / (4 + (0.1f - Parameters.ErosionBias / 10.0f));
                    }
                }
            }

            return erodedNoiseMap;
        }

        private int[] GenerateHeightSamples(int pos, int width, int maxHeight)
        {
            int[] samples = new int[width];

            for (int x = 0; x < width; x++)
            {
                double sample = 0.1;

                for (int i = 0; i < 6; i++)
                    sample += Math.Abs((sample + _surfaceNoise.GetSimplex(pos * width + x, i))) / 6;


                sample *= maxHeight * (2 / 3.0f);
                sample -= (1 / 8.0f) * maxHeight;
                sample = Math.Clamp(sample, 0, maxHeight);
                samples[x] = (int)sample;
            }

            return samples;
        }

        public void Generate(Chunk chunk)
        {
            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    Tile t = Tile.None;
                    if (_world[x + chunk.Position * Chunk.Width, y] >= Parameters.TileGenerationThreshold)
                        t = Tile.IdToTile(TileId.Dirt);

                    chunk.Tiles[x, Chunk.Height - 1 - y] = t;

                }
            }
        }
    }
}
