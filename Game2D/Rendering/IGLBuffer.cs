namespace Game2D.Rendering;

public interface IGLBuffer : IDisposable
{
    int Handle { get; set; }
}