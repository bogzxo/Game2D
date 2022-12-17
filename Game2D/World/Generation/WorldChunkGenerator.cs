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

        private static float[,] theWholeFuckingWorld;

        static WorldChunkGenerator()
        {
            theWholeFuckingWorld = new float[GameManager.Instance.GameWorld.Width * Chunk.Width, Chunk.Height];
        }

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

                sample = (sample + _surfaceNoise.GetSimplex(chunk.Position * width + x, 1.0f));
                sample *= 30;

                samples[x] = (int)Math.Clamp((float)sample, 0.0f, maxHeight);
            }

            return samples;
        }


        // Returns a new noise map with 2D erosion applied to it
        public float[,] ApplyTerrainFilters(float[,] noiseMap)
        {
            var erodedNoiseMap = new float[noiseMap.GetLength(0), noiseMap.GetLength(1)];

            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                {
                    int clampedX(int x) => Math.Clamp(x, 0, noiseMap.GetLength(0));
                    int clampedY(int y) => Math.Clamp(y, 0, noiseMap.GetLength(1));

                    // Apply erosion by setting the noise value at this point to the average of its neighbors
                    erodedNoiseMap[x, y] = (noiseMap[clampedX(x - 1), y] + noiseMap[clampedX(x + 1), y] + noiseMap[x, clampedY(y - 1)] + noiseMap[x, clampedY(y + 1)]) / 4;
                }
            }

            return erodedNoiseMap;
        }

        //public void Generate(Chunk chunk)
        //{
        //    float[,] noiseMap = new float[Chunk.Width, Chunk.Height];

        //    var heightSamples = generateHeightSamples(chunk, Chunk.Width, Chunk.Height - 1);

        //    for (int x = 0; x < Chunk.Width; x++)
        //    {
        //        for (int y = heightSamples[x]; y < Chunk.Height; y++)
        //        {
        //            //// the deeper down you go.
        //            //double yScaled = (Chunk.Height - y) * 0.5;

        //            //double sample = 0.0;

        //            //for (int i = 0; i < 10; i++)
        //            //    sample = (sample + _cavernNoise.GetSimplex(chunk.Position * Chunk.Width + x, yScaled, i)) / (2.0);
        //            //sample += (((float)Chunk.Height - y) / (float)Chunk.Height) * 10.0f;

        //            var sample = 1.0;
        //            noiseMap[x, y] = (float)sample;
        //        }
        //    }

        //    noiseMap = ApplyTerrainFilters(noiseMap);

        //    for (int x = 0; x < noiseMap.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < noiseMap.GetLength(1); y++)
        //        {
        //            Tile t = Tile.None;

        //            if (noiseMap[x, y] > 0.05f)
        //                t = Tile.IdToTile(TileId.Dirt);

        //            chunk.Tiles[x, Chunk.Height - 1 - y] = t;

        //        }
        //    }
        //}
    }
}
