using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game2D.OpenGL;

public class GLShader : IDisposable
{
    public struct UniformFieldInfo
    {
        public int Location;
        public string Name;
        public int Size;
        public ActiveUniformType Type;
    }

    public int Program { get; private set; }
    private readonly Dictionary<string, int> UniformToLocation = new Dictionary<string, int>();
    private bool Initialized = false;

    private readonly (ShaderType Type, string Path)[] Files;

    public static GLShader CreateShader(params (ShaderType Type, string Path)[] shaders)
    {
        return new GLShader(shaders);
    }

    private GLShader(params (ShaderType Type, string Path)[] shaders)
    {
        Files = shaders;
        Program = CreateProgram(Files);
    }

    public virtual void UseShader()
    {
        GL.UseProgram(Program);
    }

    public virtual void End()
    {
        GL.UseProgram(0);
    }

    public void DisposeManaged()
    {
        if (Initialized)
        {
            GL.DeleteProgram(Program);
            Initialized = false;
        }
    }

    public void Uniform3(string name, ref Vector3 value) => GL.Uniform3(GetUniformLocation(name), ref value);

    public void Uniform2(string name, ref Vector2 value) => GL.Uniform2(GetUniformLocation(name), ref value);

    public void Uniform1(string name, float value) => GL.Uniform1(GetUniformLocation(name), value);

    public void Uniform1(string name, int value) => GL.Uniform1(GetUniformLocation(name), value);

    public void Matrix4(string name, ref Matrix4 value) => GL.ProgramUniformMatrix4(Program, GetUniformLocation(name), false, ref value);

    public UniformFieldInfo[] GetUniforms()
    {
        GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out int UnifromCount);

        UniformFieldInfo[] Uniforms = new UniformFieldInfo[UnifromCount];

        for (int i = 0; i < UnifromCount; i++)
        {
            string Name = GL.GetActiveUniform(Program, i, out int Size, out ActiveUniformType Type);

            UniformFieldInfo FieldInfo;
            FieldInfo.Location = GetUniformLocation(Name);
            FieldInfo.Name = Name;
            FieldInfo.Size = Size;
            FieldInfo.Type = Type;

            Uniforms[i] = FieldInfo;
        }

        return Uniforms;
    }

    public int GetUniformLocation(string uniform)
    {
        if (UniformToLocation.TryGetValue(uniform, out int location) == false)
        {
            location = GL.GetUniformLocation(Program, uniform);
            UniformToLocation.Add(uniform, location);

            if (location == -1)
            {
                //Logger.Instance.Log(LogLevel.Warning, $"The uniform '{uniform}' does not exist in the shader!");
            }
        }

        return location;
    }

    private int CreateProgram(params (ShaderType Type, string source)[] shaderPaths)
    {
        Util.CreateProgram(out int Program);

        int[] Shaders = new int[shaderPaths.Length];
        for (int i = 0; i < shaderPaths.Length; i++)
        {
            Shaders[i] = CompileShader(shaderPaths[i].Type, shaderPaths[i].source);
        }

        foreach (var shader in Shaders)
            GL.AttachShader(Program, shader);

        GL.LinkProgram(Program);

        GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out int Success);
        if (Success == 0)
        {
            string Info = GL.GetProgramInfoLog(Program);
            //Logger.Instance.Log(LogLevel.Info, $"GL.LinkProgram had info log:\n{Info}");
            // Check for compilation errors
            GL.GetShader(Program, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(Program);
                //Logger.Instance.Log(LogLevel.Error, $"Error occurred whilst compiling Shader({Program}).\n\n{infoLog}");
            }
        }

        foreach (var Shader in Shaders)
        {
            GL.DetachShader(Program, Shader);
            GL.DeleteShader(Shader);
        }

        Initialized = true;

        return Program;
    }

    private int CompileShader(ShaderType type, string source)
    {
        Util.CreateShader(type, out int Shader);
        GL.ShaderSource(Shader, source);
        GL.CompileShader(Shader);

        GL.GetShader(Shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string Info = GL.GetProgramInfoLog(Shader);
            //Logger.Instance.Log(LogLevel.Info, $"GL.LinkProgram had info log:\n{Info}");

            GL.GetShader(Shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(Shader);
                //Logger.Instance.Log(LogLevel.Error, $"Error occurred whilst compiling Shader({Shader}).\n\n{infoLog}");
            }
        }

        return Shader;
    }

    public void Dispose()
    {
        GL.DeleteProgram(Program);
    }
}