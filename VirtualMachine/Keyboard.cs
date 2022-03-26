using SDL2;
using System;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Emmetienne.ChipOtto.VirtualMachine
{
    public class Keyboard
    {
        private bool running;
        public byte LastKeyPressed { get; set; }
        public bool IsAKeyPressed { get; set; }
   
        private IntPtr keyboardState;
        byte[] keyBoardStateArray;

        // array for scancodes ordered from 0 to F
        private SDL.SDL_Scancode[] scanCodeArray = new SDL.SDL_Scancode[] { SDL_Scancode.SDL_SCANCODE_X, SDL.SDL_Scancode.SDL_SCANCODE_1, SDL.SDL_Scancode.SDL_SCANCODE_2, SDL.SDL_Scancode.SDL_SCANCODE_3,
                                                                            SDL_Scancode.SDL_SCANCODE_Q, SDL.SDL_Scancode.SDL_SCANCODE_W, SDL.SDL_Scancode.SDL_SCANCODE_E, SDL.SDL_Scancode.SDL_SCANCODE_A,
                                                                            SDL_Scancode.SDL_SCANCODE_S, SDL.SDL_Scancode.SDL_SCANCODE_D, SDL.SDL_Scancode.SDL_SCANCODE_Z, SDL.SDL_Scancode.SDL_SCANCODE_C,
                                                                            SDL_Scancode.SDL_SCANCODE_4, SDL.SDL_Scancode.SDL_SCANCODE_R, SDL.SDL_Scancode.SDL_SCANCODE_F, SDL.SDL_Scancode.SDL_SCANCODE_V
                                                                          };


        public Keyboard()
        {
            Console.WriteLine("Keyboard initialized");
            LastKeyPressed = (byte)255;
            running = true;
        }

        public void Execute()
        {
            Console.WriteLine("Poll keyboard threaded");
            while (running)
            {
                SDL.SDL_PollEvent(out SDL.SDL_Event e);

                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    running = false;

                keyboardState = SDL.SDL_GetKeyboardState(out int keyboardArraySize);

                if (keyBoardStateArray == null || keyBoardStateArray.Length == 0)
                    keyBoardStateArray = new byte[keyboardArraySize];

                Marshal.Copy(keyboardState, keyBoardStateArray, 0, keyboardArraySize);


                if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)

                    GetKeyPressedByScanCode(e.key.keysym.scancode);

                if (LastKeyPressed == 255)
                    Console.BackgroundColor = ConsoleColor.Black;
                else
                    Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        public bool CheckIfKeyIsPressed(byte keyPressed)
        {
            return keyBoardStateArray[(int)scanCodeArray[keyPressed]] == 1;
        }

        public bool CheckIfAnyKeyIsPressed()
        {
            if (keyBoardStateArray == null)
                return false;

            for (int i = 0; i < scanCodeArray.Length; i++)
            {
                if (keyBoardStateArray[(int)scanCodeArray[i]] == 1)
                    return true;
            }

            return false;
        }

        public void GetKeyPressedByScanCode(SDL.SDL_Scancode scanCode)
        {
            switch (scanCode)
            {
                case SDL.SDL_Scancode.SDL_SCANCODE_1:
                    LastKeyPressed = (byte)1;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_2:
                    LastKeyPressed = (byte)2;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_3:
                    LastKeyPressed = (byte)3;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_4:
                    LastKeyPressed = (byte)0X000c;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_Q:
                    LastKeyPressed = (byte)4;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_W:
                    LastKeyPressed = (byte)5;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_E:
                    LastKeyPressed = (byte)6;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_R:
                    LastKeyPressed = (byte)0x000d;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_A:
                    LastKeyPressed = (byte)7;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_S:
                    LastKeyPressed = (byte)8;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_D:
                    LastKeyPressed = (byte)9;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_F:
                    LastKeyPressed = (byte)0x000e;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_Z:
                    LastKeyPressed = (byte)0x000a;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_X:
                    LastKeyPressed = (byte)0;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_C:
                    LastKeyPressed = (byte)0x000b;
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_V:
                    LastKeyPressed = (byte)0x000f;
                    break;
            }
        }

        public void ShutDown()
        {
            running = false;
        }

        public override string ToString()
        {
            if (keyBoardStateArray == null)
                return "---";

            var outString = string.Empty;

            for (int i = 0; i < scanCodeArray.Length; i++)
            {

                if (keyBoardStateArray[(int)scanCodeArray[i]] == 1)
                    outString += "true";
                else
                    outString += "false";
            }

            return outString.PadRight(5);
        }

        public bool IsResetPressed()
        {
            if (keyBoardStateArray == null || keyBoardStateArray.Length == 0)
                return false;

            return keyBoardStateArray[(int)SDL_Scancode.SDL_SCANCODE_F2] != 0;
        }

        public bool IsShutDownPressed()
        {
            if (keyBoardStateArray == null || keyBoardStateArray.Length == 0)
                return false;

            return keyBoardStateArray[(int)SDL_Scancode.SDL_SCANCODE_F4] != 0;
        }
    }
}
