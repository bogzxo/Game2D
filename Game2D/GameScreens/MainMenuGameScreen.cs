using Game2D.Data;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.GameScreens
{
    public class MainMenuGameScreen : IGameScreen
    {
        public void Initialize()
        {
            GL.ClearColor(Color4.MistyRose);

            GameManager.Instance.AssetManager.RegisterFont("roboto128", "Assets/Fonts/Roboto-Medium.ttf", 128);
        }

        bool DrawButton(string label, Vector2 pos, Vector2 size)
        {
            ImGui.SetCursorPosX(pos.X);
            ImGui.SetCursorPosY(pos.Y);

            return ImGui.Button(label, new System.Numerics.Vector2(size.X, size.Y));
        }
        void DrawText(string label, Vector2 pos)
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
        public void Draw(float deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);
            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);

            if (ImGui.Begin("test", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar))
            {
                if (DrawButton("Play!", new Vector2(GameManager.Instance.Size.X / 2.0f, GameManager.Instance.Size.Y / 2.0f), new Vector2(100, 50)))
                    GameManager.Instance.GameScreenManager.SetGameScreen(typeof(GamePlayGameScreen));

                DrawButton("Options", new Vector2(GameManager.Instance.Size.X / 2.0f, GameManager.Instance.Size.Y / 2.0f + 75.0f), new Vector2(100, 50));
                DrawText("Game2D", new Vector2(GameManager.Instance.Size.X / 2.0f, GameManager.Instance.Size.Y / 5));
                ImGui.End();
            }

        }

        public void Update(float deltaTime)
        {

        }
    }
}
