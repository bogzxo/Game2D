using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Game2D.OpenGL;

internal static class Util
{
    [Pure]
    public static float Lerp(float firstFloat, float secondFloat, float by)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }

    [Pure]
    public static float Clamp(float value, float min, float max)
    {
        return value < min ? min : value > max ? max : value;
    }

    [Conditional("DEBUG")]
    public static void CheckGLError(string title)
    {
        var error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Debug.Print($"{title}: {error}");
        }
    }

    public static string ReadResource(string name)
    {
        // Determine path
        var assembly = Assembly.GetExecutingAssembly();
        // Format: "{Namespace}.{Folder}.{filename}.{Extension}"

        string resourceName = assembly.GetManifestResourceNames()
        .Single(str => str.EndsWith(name));

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
    {
        GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateTexture(TextureTarget target, string Name, out int Texture)
    {
        GL.CreateTextures(target, 1, out Texture);
        LabelObject(ObjectLabelIdentifier.Texture, Texture, $"Texture: {Name}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateProgram(out int Program)
    {
        Program = GL.CreateProgram();
        LabelObject(ObjectLabelIdentifier.Program, Program, $"Created Program");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateShader(ShaderType type, out int Shader)
    {
        Shader = GL.CreateShader(type);
        LabelObject(ObjectLabelIdentifier.Shader, Shader, $"Created Shader: {type}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateBuffer(out int Buffer)
    {
        GL.CreateBuffers(1, out Buffer);
        LabelObject(ObjectLabelIdentifier.Buffer, Buffer, $"Created Buffer");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateBuffer(string Name, out int Buffer)
    {
        GL.CreateBuffers(1, out Buffer);
        LabelObject(ObjectLabelIdentifier.Buffer, Buffer, $"Buffer: {Name}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateVertexBuffer(string Name, out int Buffer) => CreateBuffer($"VBO: {Name}", out Buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateElementBuffer(string Name, out int Buffer) => CreateBuffer($"EBO: {Name}", out Buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateVertexArray(string Name, out int VAO)
    {
        GL.CreateVertexArrays(1, out VAO);
        LabelObject(ObjectLabelIdentifier.VertexArray, VAO, $"VAO: {Name}");
    }
}