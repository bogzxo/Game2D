using Bogz.Logging;
using Bogz.Logging.Loggers;
using Game2D.Data;
using Game2D.Entities;
using Game2D.Rendering;
using Game2D.World;
using Game2D.World.Generation;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.ComponentModel;

namespace Game2D
{
    internal class GameManager : GameWindow
    {
        private static GameManager instance;
        private static bool inited;

        public static GameManager Instance
        {
            get => instance;
        }

        public ImGuiController ImGuiController { get; private set; }
        public InputManager InputManager { get; private set; }
        public GameScreenManager GameScreenManager { get; private set; }
        public AssetManager AssetManager { get; private set; }
        public PlayerEntity Player { get; set; }
        public Camera Camera { get; set; }
        public GameWorld GameWorld { get; set; }

        public GameManager(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            inited = true;
            instance = this;
            Logger.InitializeLogger(new BasicLogger("info.log"));
            InputManager = new InputManager(InputManagerConfiguration.Default);

            ImGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);
            AssetManager = new AssetManager();

            // make sure config dir exists
            if (!Directory.Exists("config"))
                Directory.CreateDirectory("config");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            WorldGenerationParameters.SaveToFile("config/worldgen.json");

            base.OnClosing(e);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.Texture2D);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            Title = "Game2D";

            GameScreenManager = new GameScreenManager();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            ImGuiController.Update(this, (float)args.Time);

            GameScreenManager.Draw((float)args.Time);

            ImGuiController.Render();
            ImGuiController.CheckGLError("End of frame");

            SwapBuffers();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            ImGuiController.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            ImGuiController.MouseScroll(e.Offset);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            ImGuiController.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            InputManager.Update();
            GameScreenManager.Update((float)args.Time);
        }
    }
}