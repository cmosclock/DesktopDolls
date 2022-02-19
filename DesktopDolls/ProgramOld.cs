namespace DesktopDolls;

public class ProgramOld
{
    public void Main()
    {
        // Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + "dll");
        //
        // SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
        // var windowWidth = 500;
        // var windowHeight = 500;
        // var window = SDL.SDL_CreateWindow("title", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, windowWidth, windowHeight,
        //     SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        // var renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        //
        // // load img
        // var img = SDL_image.IMG_LoadTexture(renderer, @"C:\Users\pc\Documents\pcfiles\projects\DesktopDolls\DesktopDolls\m4a1\m4a1.png");
        // // get the width and height of the texture
        // SDL.SDL_QueryTexture(img, out _, out _, out var imgW, out var imgH); 
        // // put the location where we want the texture to be drawn into a rectangle
        // // I'm also scaling the texture 2x simply by setting the width and height
        // SDL.SDL_Rect texr;
        // texr.x = windowWidth/2;
        // texr.y = windowHeight/2;
        // texr.w = imgW*2;
        // texr.h = imgH*2; 
        // var textrPtr = Marshal.AllocHGlobal(Marshal.SizeOf(texr));
        // Marshal.StructureToPtr(texr, textrPtr, false);
        //
        // var surface = SDL.SDL_GetWindowSurface(window);
        // var surfaceStruct = Marshal.PtrToStructure<SDL.SDL_Surface>(surface);
        // SDL.SDL_FillRect(surface, IntPtr.Zero, SDL.SDL_MapRGB(surfaceStruct.format, 0x00, 0x00, 0x00));
        // SDL.SDL_UpdateWindowSurface(window);
        // var fps = 120;
        // var running = true;
        // uint start;
        //
        // // spine
        // var textureLoader = new XnaTextureLoader();
        // var atlas = new Atlas(@"C:\Users\pc\Documents\pcfiles\projects\DesktopDolls\DesktopDolls\m4a1\m4a1.atlas", textureLoader);
        // var skeletonBinary = new SkeletonBinary(atlas);
        // var skeletonData = skeletonBinary.ReadSkeletonData(@"C:\Users\pc\Documents\pcfiles\projects\DesktopDolls\DesktopDolls\m4a1\m4a1.skel");
        // var skeleton = new Skeleton(skeletonData);
        // Console.WriteLine("Hello, World!");
        //
        //
        // while (running)
        // {
        //     start = SDL.SDL_GetTicks();
        //     while (SDL.SDL_PollEvent(out var sdlEvent) != 0)
        //     {
        //         switch(sdlEvent.type)
        //         {
        //             case SDL.SDL_EventType.SDL_QUIT:
        //             running = false;
        //             break;
        //         }
        //     }
        //     // game logic and rendering.
        //     // player(screen);
        //     // SDL_Flip(screen);
        //     
        //     SDL.SDL_RenderClear(renderer);
        //     // copy the texture to the rendering context
        //     SDL.SDL_RenderCopy(renderer, img, IntPtr.Zero, textrPtr);
        //     // flip the backbuffer
        //     // this means that everything that we prepared behind the screens is actually shown
        //     SDL.SDL_RenderPresent(renderer);
        //
        //     if(1000 / fps > SDL.SDL_GetTicks() - start)
        //     {
        //         SDL.SDL_Delay((uint)(1000 / fps - (SDL.SDL_GetTicks() - start)));
        //     }
        // }
        // Marshal.FreeHGlobal(textrPtr);
        // SDL.SDL_DestroyWindow(window);
        // SDL.SDL_Quit();
    }
}