using Bogz.Logging;
using Game2D.Rendering;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Data
{
    public struct AssetManagerFontAsset
    {
        public int Size { get; set; }
        public ImFontPtr Pointer { get; set; }

        public AssetManagerFontAsset(int size, ImFontPtr ptr)
        {
            Size = size;
            Pointer = ptr;
        }
    }
    public struct AssetManagerShaderAsset
    {
        public Shader Shader { get; set; }

        public AssetManagerShaderAsset(string fragPath, string vertPath)
        {
            Shader = Shader.CreateShader((ShaderType.FragmentShader, fragPath), (ShaderType.VertexShader, vertPath));
        }
    }
    public class AssetManager : IDisposable
    {
        private bool disposedValue;

        public Dictionary<string, AssetManagerFontAsset> Fonts { get; set; }
        public Dictionary<string, AssetManagerShaderAsset> Shaders { get; set; }

        public AssetManager()
        {
            Fonts = new Dictionary<string, AssetManagerFontAsset>();
            Shaders = new Dictionary<string, AssetManagerShaderAsset>();

            Logger.Instance.Log(LogLevel.Info, "Asset Manager Loaded!");
        }

        public ImFontPtr GetFont(in string name) => Fonts[name].Pointer;
        public Shader GetShader(in string name) => Shaders[name].Shader;


        public void RegisterFont(string name, string path, int size)
        {
            var io = ImGui.GetIO();

            Fonts.Add(name, new AssetManagerFontAsset(size, io.Fonts.AddFontFromFileTTF("Assets/Fonts/Roboto-Medium.ttf", size)));

            if (io.Fonts.Build())
            {
                GameManager.Instance.ImGuiController.RecreateFontDeviceTexture();

                Logger.Instance.Log(LogLevel.Success, $"Font({name}) Loaded Successfully!");
            }
            else
                Logger.Instance.Log(LogLevel.Error, $"Font({name}) Failed to load!");
        }

        public Shader RegisterShader(in string name, in string fragPath, in string vertPath)
        {
            Shaders.Add(name, new AssetManagerShaderAsset(fragPath, vertPath));
            Logger.Instance.Log(LogLevel.Success, $"Shader({name}) Loaded Successfully!");

            return Shaders[name].Shader;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                foreach (var item in Fonts)
                    item.Value.Pointer.Destroy();

                foreach (var item in Shaders)
                    item.Value.Shader.Dispose();

                if (disposing)
                {
                    Fonts.Clear();
                    Fonts = null;

                    Shaders.Clear();
                    Shaders = null;
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AssetManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
