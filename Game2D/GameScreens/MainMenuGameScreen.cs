using Game2D.OpenGL;
using Game2D.Rendering;
using Game2D.World;
using Game2D.World.Generation;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game2D.GameScreens
{
    public class MainMenuGameScreen : IGameScreen
    {
        private RenderRectangle fullscreenRenderRect;
        private Shader clouds;

        public void Initialize()
        {
            GL.ClearColor(Color4.Black);

            LoadData();

            fullscreenRenderRect = new RenderRectangle();
        }

        private void LoadData()
        {
            WorldGenerationParameters.LoadFromFile("config/worldgen.json");
            clouds = GameManager.Instance.AssetManager.RegisterShader("background_clouds", "Assets/Shader/clouds.frag", "Assets/Shader/basic.vert");
            GameManager.Instance.AssetManager.RegisterFont("roboto128", "Assets/Fonts/Roboto-Medium.ttf", 128);

            GameManager.Instance.GameWorld = new GameWorld();
        }

        private bool DrawButton(string label, Vector2 pos, Vector2 size)
        {
            ImGui.SetCursorPosX(pos.X);
            ImGui.SetCursorPosY(pos.Y);

            return ImGui.Button(label, new System.Numerics.Vector2(size.X, size.Y));
        }

        private void DrawText(string label, Vector2 pos)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, System.Numerics.Vector4.UnitW);

            ImGui.PushFont(GameManager.Instance.AssetManager.GetFont("roboto128"));

            var textSize = ImGui.CalcTextSize(label);

            ImGui.SetCursorPosX(pos.X - textSize.X / 2.0f);
            ImGui.SetCursorPosY(pos.Y - textSize.Y / 2.0f);

            ImGui.Text(label);
            ImGui.PopFont();

            ImGui.PopStyleColor();
        }

        private float iTime = 0, transitionTimer;
        private bool transition, optionsToggle;

        public void Draw(float deltaTime)
        {
            iTime += deltaTime;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            clouds.UseShader();

            var mat = Matrix4.Identity;
            var ires = new Vector2(GameManager.Instance.Size.X, GameManager.Instance.Size.Y);

            clouds.Matrix4("mvp", ref mat);
            clouds.Uniform1("iTime", iTime);
            clouds.Uniform2("iResolution", ref ires);
            clouds.Uniform1("pixels", 256.0f);
            clouds.Uniform1("brightness", 1 - transitionTimer);

            fullscreenRenderRect.Draw(deltaTime);
            clouds.End();

            ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);
            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);

            if (!transition && ImGui.Begin("test", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar))
            {
                transition = DrawButton("Play!", new Vector2(GameManager.Instance.Size.X / 2.0f, GameManager.Instance.Size.Y / 2.0f), new Vector2(100, 50));
                if (DrawButton("Options", new Vector2(GameManager.Instance.Size.X / 2.0f, GameManager.Instance.Size.Y / 2.0f + 75.0f), new Vector2(100, 50)))
                    optionsToggle = true;

                DrawText("Game2D", new Vector2(GameManager.Instance.Size.X / 2.0f, GameManager.Instance.Size.Y / 5));
                ImGui.End();
            }

            transitionTimer += transition ? deltaTime : 0.0f;

            if (transitionTimer > 1)
                GameManager.Instance.GameScreenManager.SetGameScreen(typeof(GamePlayGameScreen));

            if (optionsToggle && !transition)
            {
                ImGui.Begin("Options", ImGuiWindowFlags.AlwaysAutoResize);

                ImGui.BeginTabBar("Options");
                if (ImGui.BeginTabItem("Graphics"))
                {
                    DrawGraphicsTab();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("World Generation"))
                {
                    DrawWorldGenerationTab(deltaTime);
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();

                ImGui.End();
            }
        }

        private float updateTimer = 0.0f;
        private float[] samples;

        private void DrawWorldGenerationTab(float dt)
        {
            updateTimer += dt;
            ImGui.InputInt("Seed: ", ref WorldGenerationParameters.CurrentParameters.Seed);
            ImGui.SliderFloat($"Genration Threshold:", ref WorldGenerationParameters.CurrentParameters.TileGenerationThreshold, 0.0f, 1.0f);
            ImGui.SliderFloat($"Erosion Bias:", ref WorldGenerationParameters.CurrentParameters.ErosionBias, 0.0f, 1.0f);
            ImGui.SliderInt($"Erosion Passes:", ref WorldGenerationParameters.CurrentParameters.ErosionPasses, 1, 10);

            ImGui.SliderFloat($"Surface Frequency:", ref WorldGenerationParameters.CurrentParameters.BaseSurfaceFrequency, 0.0f, 1.0f);
            ImGui.SliderFloat($"Cavern Frequency:", ref WorldGenerationParameters.CurrentParameters.BaseCavernFrequency, 0.0f, 1.0f);

            if (updateTimer > 0.5f)
            {
                updateTimer = 0.0f;
                var heightMapSamples = new List<float>();
                GameManager.Instance.GameWorld.GameWorldGenerator.Regenerate();
                for (int i = 0; i < GameManager.Instance.GameWorld.Width; i++)
                    heightMapSamples.AddRange(GameManager.Instance.GameWorld.GameWorldGenerator.GenerateHeightSamplesPreview(i, Chunk.Width, Chunk.Height - 1));

                samples = heightMapSamples.ToArray();
            }
            if (samples != null)
                ImGui.PlotLines("Heightmap", ref samples[0], samples.Length);
        }

        private void DrawGraphicsTab()
        {
            ImGui.Text("WIP");
        }

        public void Update(float deltaTime)
        {
        }
    }
}