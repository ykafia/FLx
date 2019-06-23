using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SkiaSharp;
using GLFW;
using System.Globalization;


namespace FLx.Engine
{
    public class Engine
    {
        public NativeWindow Window { get; set; }
        public SKCanvas Canvas { get; set; }
        public SKSurface Surface { get; set; }
        public GRContext Context { get; set; }
        public List<Keys> KeysPressed { get; set; }
        public List<Point> MousePosition { get; set; }
        public DateTime TimeLastFrame{get;set;}
        private int temporary = 90;

        public Engine(string appName, int height, int width)
        {
            this.Window = new NativeWindow(width, height, appName);
            this.Context = GenerateSkiaContext(this.Window);
            this.Surface = GenerateSkiaSurface(this.Context, this.Window.ClientSize);
            this.Canvas = this.Surface.Canvas;
        }
        public void Run()
        {
            this.TimeLastFrame = DateTime.Now;
            while (!this.Window.IsClosing)
            {
                
                //Render the frame
                this.Render();
                //Wait for the time to refresh the window
                while((DateTime.Now - this.TimeLastFrame).TotalMilliseconds<100)
                //Refresh the window
                this.Refresh();
                TimeLastFrame = DateTime.Now;
                
            }
        }
        public void Render()
        {
            //TODO: Add the rendering in this one.
            this.Canvas.Clear(SKColor.Parse("#F0F0F0"));
            var inputInfoPaint = new SKPaint { Color = SKColor.Parse("#F34336"), TextSize = 18, IsAntialias = true };
            this.Canvas.DrawText("Hello world", 10, this.temporary, inputInfoPaint);
            this.temporary = (this.temporary + 1)%90 + 90;
        }
        public void Refresh()
        {
            //TODO: Do the refresh function with the Canvas.Flush() suivi de Window.SwapBuffers()
            
            this.Canvas.Flush();
            this.Window.SwapBuffers();
        }
        public object GetNativeContext(NativeWindow nwin)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Native.GetWglContext(nwin);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // XServer
                return Native.GetGLXContext(nwin);
                // Wayland
                //return Native.GetEglContext(nativeWindow);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Native.GetNSGLContext(nwin);
            }

            throw new PlatformNotSupportedException();
        }
        public GRContext GenerateSkiaContext(NativeWindow nativeWindow)
        {
            var nativeContext = this.GetNativeContext(nativeWindow);
            var glInterface = GRGlInterface.AssembleGlInterface(nativeContext, (contextHandle, name) => Glfw.GetProcAddress(name));
            return GRContext.Create(GRBackend.OpenGL, glInterface);
        }
        private SKSurface GenerateSkiaSurface(GRContext skiaContext, Size surfaceSize)
        {
            var frameBufferInfo = new GRGlFramebufferInfo((uint)new UIntPtr(0), GRPixelConfig.Rgba8888.ToGlSizedFormat());
            var backendRenderTarget = new GRBackendRenderTarget(surfaceSize.Width,
                                                                surfaceSize.Height,
                                                                0,
                                                                8,
                                                                frameBufferInfo);
            return SKSurface.Create(skiaContext, backendRenderTarget, GRSurfaceOrigin.BottomLeft, SKImageInfo.PlatformColorType);
        }
    }
}
