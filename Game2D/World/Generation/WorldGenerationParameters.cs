namespace Game2D.World.Generation
{
    public struct WorldGenerationParameters
    {
        public static readonly WorldGenerationParameters Default = new WorldGenerationParameters
        {
            BaseSurfaceFrequency = 0.07f,
            BaseCavernFrequency = 0.1f,
            ErosionBias = 0.1f,
            ErosionPasses = 5,
            TileGenerationThreshold = 0.55f
        };
        public float BaseSurfaceFrequency;
        public float BaseCavernFrequency;
        public float ErosionBias;
        public float TileGenerationThreshold;
        public int ErosionPasses;
    }
}
