using OpenTK.Graphics.OpenGL4;

namespace Game2D.OpenGL;

public struct VertexAttribPointer
{
    public int Index { get; set; }
    public int Size { get; set; }
    public int Stride { get; set; }
    public int Offset { get; set; }

    public VertexAttribPointerType Type { get; set; }

    public void Use()
    {
        GL.VertexAttribPointer(Index, Size, Type, false, Stride, Offset);
        GL.EnableVertexAttribArray(Index);
    }
}

public class VertexBufferObject : IDisposable
{
    public int Handle { get; set; }

    private readonly int _mVao, _mEbo;

    private List<VertexAttribPointer> _vertexAttribs;
    public int ElementSize { get; private set; }

    public VertexBufferObject()
    {
        this.Handle = GL.GenBuffer();
        this._mEbo = GL.GenBuffer();
        this._mVao = GL.GenVertexArray();
        this._vertexAttribs = new List<VertexAttribPointer>();
    }

    public void PushVertexAttribPointer(int index, int size, VertexAttribPointerType type, int stride, int offset)
    {
        this._vertexAttribs.Add(new VertexAttribPointer
        {
            Index = index,
            Size = size,
            Type = type,
            Stride = stride,
            Offset = offset
        });
    }

    public void BufferData<T>(T[] data, int size) where T : struct
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, this.Handle);
        {
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * size, data, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int vSize);
            if (data.Length * size != vSize)
                throw new ApplicationException("Vertex buffer data not uploaded correctly");
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void BufferIndices(uint[] data)
    {
        ElementSize = data.Length;

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._mEbo);
        {
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out int vSize);
            if (data.Length * sizeof(uint) != vSize)
                throw new ApplicationException("Element data not uploaded correctly");
        }
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    public void Use()
    {
        GL.BindVertexArray(_mVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, this.Handle);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, this._mEbo);

        foreach (var item in _vertexAttribs)
            item.Use();
    }

    public void End()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(this._mVao);
        GL.DeleteBuffer(this.Handle);
    }
}