using Game2D.World.Tiles;
using PostSharp.Aspects.Advices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.World.Generation
{
    public class WorldChunkGenerator
    {
        private readonly Noise _surfaceNoise = new Noise();
        private readonly Noise _cavernNoise = new Noise();

        public WorldChunkGenerator()
        {
            WorldGenerationParameters parameters = WorldGenerationParameters.Default;
            _surfaceNoise.SetFrequency(parameters.BaseSurfaceFrequency);
            _cavernNoise.SetFrequency(parameters.BaseCavernFrequency);
        }

        private int[] generateHeightSamples(Chunk chunk, int width, int maxHeight)
        {
            int[] samples = new int[width];

            for (int x = 0; x < width; x++)
            {
                double sample = 0;

                for (int i = 0; i < 10; i++)
                    sample = (sample + _surfaceNoise.GetSimplex(chunk.Position * width + x, i)) / 2.0;
                sample *= 30;

                samples[x] = (int)Math.Clamp((float)sample, 0.0f, maxHeight);
            }

            return samples;
        }
        public void Generate(Chunk chunk)
        {
            var heightSamples = generateHeightSamples(chunk, Chunk.Width, Chunk.Height - 1);

            for (int x = 0; x < Chunk.Width; x++)
            {
                int realY = 0;
                for (int y = heightSamples[x]; y < Chunk.Height; y++)
                {
                    double sample = 0.0;

                    for (int i = 0; i < 10; i++)
                        sample = (sample + _cavernNoise.GetSimplex(chunk.Position * Chunk.Width + x, y, i)) / 2.0;

                    if (sample > 0.25f) continue;

                    Tile t = Tile.None;

                    t = Tile.IdToTile(TileId.Dirt);

                    chunk.Tiles[x, Chunk.Height - 1 - y] = t;

                    realY++;
                }
            }
        }
    }
}
