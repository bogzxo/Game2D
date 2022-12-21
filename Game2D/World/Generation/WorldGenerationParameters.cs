namespace Game2D.World.Generation
{
    public struct WorldGenerationParameters
    {
        public static readonly WorldGenerationParameters Default = new WorldGenerationParameters
        {
            BaseSurfaceFrequency = 0.04f,
            BaseCavernFrequency = 0.04f,
            ErosionBias = 0.1f,
            ErosionPasses = 9,
            TileGenerationThreshold = 0.50f,
            Seed = 0
        };
        public float BaseSurfaceFrequency;
        public float BaseCavernFrequency;
        public float ErosionBias;
        public float TileGenerationThreshold;
        public int ErosionPasses;
        public int Seed;
    }
}
