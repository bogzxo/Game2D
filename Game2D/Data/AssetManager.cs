using Bogz.Logging;
using ImGuiNET;
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
    public class AssetManager : IDisposable
    {
        private bool disposedValue;

        public Dictionary<string, AssetManagerFontAsset> Fonts { get; set; }

        public AssetManager()
        {
            Fonts = new Dictionary<string, AssetManagerFontAsset>();

            Logger.Instance.Log(LogLevel.Info, "Asset Manager Loaded!");
        }

        public ImFontPtr GetFont(in string name) => Fonts[name].Pointer;

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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                foreach (var item in Fonts)
                    item.Value.Pointer.Destroy();

                if (disposing)
                {
                    Fonts.Clear();
                    Fonts = null;
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
