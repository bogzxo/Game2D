using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.OpenGL;

public class FrameBufferObject : IDisposable
{
    public int Width;
    public int Height;

    public uint FBOHandle;

    public uint NormalTexture;
    public uint ColorTexture;
    public uint FragTexture;
    public uint DepthTexture;

    public FrameBufferObject(int width, int height)
    {
        Width = width;
        Height = height;

        //Logger.Instance.Log(LogLevel.Info, $"GL LOG: Created FBO {Width}x{Height}.");

        // Create Color Tex
        GL.GenTextures(1, out ColorTexture);
        GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        // Create normal tex
        GL.GenTextures(1, out NormalTexture);
        GL.BindTexture(TextureTarget.Texture2D, NormalTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16, width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        // Create Frag Tex
        GL.GenTextures(1, out FragTexture);
        GL.BindTexture(TextureTarget.Texture2D, FragTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16, width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        // Create Depth buffer
        GL.GenTextures(1, out DepthTexture);
        GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent16, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        // Create a FBO and attach the textures
        GL.GenFramebuffers(1, out FBOHandle);
        GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);
        GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);
        GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment1Ext, TextureTarget.Texture2D, FragTexture, 0);
        GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment2Ext, TextureTarget.Texture2D, NormalTexture, 0);
        GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTexture, 0);

        GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // disable rendering into the FBO

        GL.BindTexture(TextureTarget.Texture2D, 0); // bind default texture
    }


    public void Update()
    {
        if (GameManager.Instance.Size != new OpenTK.Mathematics.Vector2i(Width, Height))
            Resize(GameManager.Instance.Size.X, GameManager.Instance.Size.Y);
    }

    public void Resize(int width, int height)
    {
        Width = width;
        Height = height;

        // Create Color Tex
        GL.DeleteFramebuffer(FBOHandle);
        GL.DeleteTexture(ColorTexture);

        GL.GenTextures(1, out ColorTexture);
        GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        // Create normal tex
        GL.DeleteTexture(NormalTexture);

        GL.GenTextures(1, out NormalTexture);
        GL.BindTexture(TextureTarget.Texture2D, NormalTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        // Create Frag Tex
        GL.DeleteTexture(FragTexture);

        GL.GenTextures(1, out FragTexture);
        GL.BindTexture(TextureTarget.Texture2D, FragTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        // Create Depth buffer
        GL.DeleteTexture(DepthTexture);

        GL.GenTextures(1, out DepthTexture);
        GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        // Create a FBO and attach the textures
        GL.DeleteFramebuffer(FBOHandle);

        GL.GenFramebuffers(1, out FBOHandle);
        GL.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);
        GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);
        GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment1Ext, TextureTarget.Texture2D, FragTexture, 0);
        GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment2Ext, TextureTarget.Texture2D, NormalTexture, 0);
        GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTexture, 0);

        GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // disable rendering into the FBO
        //Logger.Instance.Log(LogLevel.Success, $"GL LOG: FBO resized from {Width}x{Height} to {width}x{height}.");
    }

    public void Use()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
    }

    public void End()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void Dispose()
    {
        //Logger.Instance.Log(LogLevel.Info, $"GL LOG: Deleted FBO({FBOHandle}) size {Width}x{Height}.");

        GL.DeleteTexture(ColorTexture);
        GL.DeleteTexture(NormalTexture);
        GL.DeleteTexture(FragTexture);
        GL.DeleteTexture(DepthTexture);

        GL.DeleteBuffer(FBOHandle);
    }
}