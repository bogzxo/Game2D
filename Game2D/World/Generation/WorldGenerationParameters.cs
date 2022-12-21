using Newtonsoft.Json;

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

        public static WorldGenerationParameters CurrentParameters;

        // this sucks but im lazy so cope
        public static void LoadFromFile(string file)
        {
        A:
            if (!File.Exists(file))
            {
                CurrentParameters = Default;
                SaveToFile(file);
                return;
            }

            try
            {
                CurrentParameters = JsonConvert.DeserializeObject<WorldGenerationParameters>(File.ReadAllText(file));
            }
            catch (Exception)
            {
                File.Delete(file);
                goto A;
            }
        }

        public static void SaveToFile(string fileName)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(CurrentParameters));
        }

        public float BaseSurfaceFrequency;
        public float BaseCavernFrequency;
        public float ErosionBias;
        public float TileGenerationThreshold;
        public int ErosionPasses;
        public int Seed;
    }
}
