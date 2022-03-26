using Emmetienne.ChipOtto.Model;
using SDL2;
using System;

namespace Emmetienne.ChipOtto.VirtualMachine
{
    public class Display
    {
        const int screenWidth = 64;
        const int screenHeight = 32;
        private IntPtr window;
        public IntPtr renderer;
        private readonly DisplaySettings screen;

        public Display(DisplaySettings screen)
        {
            Console.WriteLine("Renderer initialized");
            this.screen = screen;

            InitializeDisplay();
        }

        private void PresentDisplay()
        {
            SDL.SDL_RenderPresent(renderer);
        }

        public void Execute(bool draw)
        {
            SDL.SDL_PollEvent(out SDL.SDL_Event e);
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    break;
                default:
                    break;
            }

            if (draw)
                PresentDisplay();
        }

        void PrepareDisplay()
        {
            SDL.SDL_RenderClear(renderer);
        }

        public void InitializeDisplay()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
                Console.WriteLine($"Error in initializating SDL {SDL.SDL_GetError()}");

            this.window = SDL.SDL_CreateWindow("ChipOtto", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, screenWidth * screen.Scale, screenHeight * screen.Scale, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS);

            if (this.window == IntPtr.Zero)
                Console.WriteLine($"Error in initializing SDL Window {SDL.SDL_GetError()}");

            SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "linear");
            SDL.SDL_SetHint(SDL.SDL_HINT_ALLOW_TOPMOST, "1");

            this.renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC );

            if (this.renderer == IntPtr.Zero)
                Console.WriteLine($"Error in initializing SDL Renderer {SDL.SDL_GetError()}");

            SDL.SDL_RaiseWindow(window);

            PrepareDisplay();
        }

        public void DrawFromMemory(byte[] memory)
        {
            var x = 0;
            var y = 0;

            for (int i = 0x800; i < memory.Length; i++)
            {
                var pixelByte = memory[i];
                var isForeground = pixelByte == 1 ? true : false;

                if (x == screenWidth)
                {
                    x = 0;
                    y++;
                }

                PlotPixel(x++, y, isForeground);
            }
        }

        private void PlotPixel(int x, int y, bool foreGround)
        {
            if (foreGround)
                SDL.SDL_SetRenderDrawColor(renderer, 3, 205, 45, 255);
            else
                SDL.SDL_SetRenderDrawColor(renderer, 0, 52, 5, 255);

            var rectangle = new SDL.SDL_Rect();
            rectangle.x = x * screen.Scale;
            rectangle.y = y * screen.Scale;
            rectangle.w = screen.Scale;
            rectangle.h = screen.Scale;

            SDL.SDL_RenderFillRect(renderer, ref rectangle);
        }
    }
}
