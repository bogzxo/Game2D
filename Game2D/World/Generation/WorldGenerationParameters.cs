namespace Game2D.World.Generation
{
    public struct WorldGenerationParameters
    {
        public static readonly WorldGenerationParameters Default = new WorldGenerationParameters
        {
            BaseSurfaceFrequency = 0.07f,
            BaseCavernFrequency = 0.1f
        };
        public float BaseSurfaceFrequency { get; set; }
        public float BaseCavernFrequency { get; set; }
    }
}
