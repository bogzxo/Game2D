using Bogz.Logging;
using Game2D.Rendering;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

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
    public struct AssetManagerTextureAsset
    {
        public Texture Texture { get; set; }

        public AssetManagerTextureAsset(string path)
        {
            Texture = new Texture(path);
        }
    }

    public enum AssetType
    {
        Texture,
        Shader,
        Font
    }
    public class AssetManager : IDisposable
    {
        private bool disposedValue;

        public Dictionary<string, AssetManagerFontAsset> Fonts { get; set; }
        public Dictionary<string, AssetManagerShaderAsset> Shaders { get; set; }
        public Dictionary<string, AssetManagerTextureAsset> Textures { get; set; }

        public AssetManager()
        {
            Fonts = new Dictionary<string, AssetManagerFontAsset>();
            Shaders = new Dictionary<string, AssetManagerShaderAsset>();
            Textures = new Dictionary<string, AssetManagerTextureAsset>();

            Logger.Instance.Log(LogLevel.Info, "Asset Manager Loaded!");
        }

        public ImFontPtr GetFont(in string name) => Fonts[name].Pointer;
        public Shader GetShader(in string name) => Shaders[name].Shader;
        public Texture GetTexture(in string name) => Textures[name].Texture;

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

        public Shader RegisterShader(in string name, in string vertPath, in string fragPath)
        {
            Shaders.Add(name, new AssetManagerShaderAsset(fragPath, vertPath));
            Logger.Instance.Log(LogLevel.Success, $"Shader({name}) Loaded Successfully!");

            return Shaders[name].Shader;
        }

        public Texture RegisterTexture(in string name, in string path)
        {
            Textures.Add(name, new AssetManagerTextureAsset(path));
            Logger.Instance.Log(LogLevel.Success, $"Texture({name}) Loaded Successfully!");

            return Textures[name].Texture;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                foreach (var item in Fonts)
                    item.Value.Pointer.Destroy();

                foreach (var item in Shaders)
                    item.Value.Shader.Dispose();

                foreach (var item in Textures)
                    item.Value.Texture.Dispose();

                if (disposing)
                {
                    Fonts.Clear();
                    Fonts = null;

                    Shaders.Clear();
                    Shaders = null;

                    Textures.Clear();
                    Textures = null;
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

        internal bool HasAsset(AssetType type, string name)
        {
            return type switch
            {
                AssetType.Texture => Textures.ContainsKey(name),
                AssetType.Shader => Shaders.ContainsKey(name),
                AssetType.Font => Fonts.ContainsKey(name),
                _ => false,
            };
        }
    }
}