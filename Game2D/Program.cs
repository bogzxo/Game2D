using OpenTK.Windowing.Desktop;
using System;
using System.Runtime.InteropServices;

namespace Game2D
{
    static class Program
    {
        static void Main()
        {
            var s = NativeWindowSettings.Default;
            s.Size = new OpenTK.Mathematics.Vector2i(1920, 1080);
            s.RedBits = 10;
            s.GreenBits = 10;
            s.BlueBits = 10;
            s.AlphaBits = 10;
            new GameManager(GameWindowSettings.Default, s).Run();
            //GameManager.Instance.Run();
        }
    }
}