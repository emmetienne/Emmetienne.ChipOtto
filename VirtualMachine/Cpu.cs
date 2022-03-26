using Emmetienne.ChipOtto.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Emmetienne.ChipOtto.VirtualMachine
{
    public class Cpu
    {
        private readonly Display display;
        private readonly Keyboard keyboard;
        private readonly Beeper beeper;
        private readonly DelayTimer delayTimer;

        private byte[] rom;

        private readonly bool debugInfo;

        private int programCounter;
        private ushort iRegister;
        private byte[] memory;
        private byte[] vRegisters;
        private Stack<ushort> stack = new Stack<ushort>();

        private bool drawFlag;


        private readonly byte[] fonts = {0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
                                         0x20, 0x60, 0x20, 0x20, 0x70, // 1
                                         0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
                                         0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
                                         0x90, 0x90, 0xF0, 0x10, 0x10, // 4
                                         0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
                                         0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
                                         0xF0, 0x10, 0x20, 0x40, 0x40, // 7
                                         0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
                                         0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
                                         0xF0, 0x90, 0xF0, 0x90, 0x90, // A
                                         0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
                                         0xF0, 0x80, 0x80, 0x80, 0xF0, // C
                                         0xE0, 0x90, 0x90, 0x90, 0xE0, // D
                                         0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
                                         0xF0, 0x80, 0xF0, 0x80, 0x80  // F
                                        };

        private bool running;

        public Cpu(Display display, Keyboard keyboard, Beeper beeper, DelayTimer delayTimer, byte[] rom, bool debugInfo = true)
        {
            this.display = display;
            this.keyboard = keyboard;
            this.beeper = beeper;
            this.delayTimer = delayTimer;
            this.rom = rom;
            this.debugInfo = debugInfo;

            Reset();
        }

        public void Reset()
        {
            this.memory = new byte[4096];
            this.iRegister = 0;
            this.vRegisters = new byte[16];
            this.programCounter = 512;
            this.stack = new Stack<ushort>();

            LoadRomInMemory();

            display.DrawFromMemory(memory);

            for (int i = 0; i < fonts.Length; i++)
                memory[i] = fonts[i];

            running = true;
        }

        public void LoadRomInMemory()
        {
            for (int i = 0; i < this.rom.Length; i++)
            {
                memory[512 + i] = this.rom[i];
            }
        }

        public void Execute()
        {
            Console.Clear();

            // create a thread for keyboard input
            var keyboardThread = new Thread(new ThreadStart(keyboard.Execute));
            keyboardThread.Start();

            // create a thread for beeper
            var beeperThread = new Thread(new ThreadStart(beeper.Execute));
            beeperThread.Start();

            // create a thread for the delay timer
            var delayTimerThread = new Thread(new ThreadStart(delayTimer.Execute));
            delayTimerThread.Start();

            while (running)
            {
                try
                {
                    if (keyboard.IsShutDownPressed())
                        ShutDown();

                    if (keyboard.IsResetPressed())
                        Reset();

                    ExecuteOpCode();
                    display.Execute(drawFlag);
                    drawFlag = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            ShutDown();
        }

        private void ExecuteOpCode()
        {
            var opCodeUShort = (ushort)BitConverter.ToUInt16(new byte[2] { (byte)memory[programCounter + 1], (byte)memory[programCounter] }, 0); ;
            var opCode = new OpCode(opCodeUShort);

            if (debugInfo)
                PrintDebugInfo(opCode);

            switch (opCode.Nibble)
            {
                case 0x0000:
                    switch (opCode.Code)
                    {
                        case 0x00e0:
                            ClearDisplayMemory();
                            drawFlag = true;
                            programCounter += 2;
                            break;
                        case 0x00ee:
                            programCounter = stack.Pop();
                            programCounter += 2;
                            break;
                        default: throw new Exception($"Not supported Op Code <{opCode.Code.ToString("X4")}>");
                    }
                    break;
                case 0x1000:
                    programCounter = opCode.NNN;
                    break;
                case 0x2000:
                    this.stack.Push((ushort)programCounter);
                    programCounter = opCode.NNN;
                    break;
                case 0x3000:
                    if (vRegisters[opCode.X] == opCode.NN)
                        programCounter += 4;
                    else
                        programCounter += 2;
                    break;
                case 0x4000:
                    if (vRegisters[opCode.X] != opCode.NN)
                        programCounter += 4;
                    else
                        programCounter += 2;
                    break;
                case 0x5000:
                    if (vRegisters[opCode.X] == vRegisters[opCode.Y])
                        programCounter += 4;
                    else
                        programCounter += 2;
                    break;
                case 0x6000:
                    vRegisters[opCode.X] = (byte)opCode.NN;
                    programCounter += 2;
                    break;
                case 0x7000:
                    var result = (int)vRegisters[opCode.X] + opCode.NN;

                    if (result > 255)
                        result -= 256;

                    vRegisters[opCode.X] = (byte)result;
                    programCounter += 2;
                    break;
                case 0x8000:
                    switch (opCode.N)
                    {
                        case 0:
                            vRegisters[opCode.X] = vRegisters[opCode.Y];
                            programCounter += 2;
                            break;
                        case 1:
                            vRegisters[opCode.X] = (byte)(vRegisters[opCode.X] | vRegisters[opCode.Y]);
                            programCounter += 2;
                            break;
                        case 2:
                            vRegisters[opCode.X] = (byte)(vRegisters[opCode.X] & vRegisters[opCode.Y]);
                            programCounter += 2;
                            break;
                        case 3:
                            vRegisters[opCode.X] = (byte)(vRegisters[opCode.X] ^ vRegisters[opCode.Y]);
                            programCounter += 2;
                            break;
                        case 4:
                            var sumResult = (int)vRegisters[opCode.X] + vRegisters[opCode.Y];

                            if (sumResult > 255)
                            {
                                sumResult -= 256;
                                vRegisters[0xf] = (byte)1;
                            }
                            else
                            {
                                vRegisters[0xf] = (byte)0;
                            }

                            vRegisters[opCode.X] = (byte)sumResult;
                            programCounter += 2;
                            break;
                        case 5:
                            var subtractionResult = (int)vRegisters[opCode.X] - vRegisters[opCode.Y];

                            if (subtractionResult < 0)
                            {
                                subtractionResult += 256;
                                vRegisters[0xf] = (byte)0;
                            }
                            else
                                vRegisters[0xf] = (byte)1;

                            vRegisters[opCode.X] = (byte)subtractionResult;
                            programCounter += 2;
                            break;
                        case 6:
                            vRegisters[0xf] = (byte)(vRegisters[opCode.X] & 0x000f);
                            vRegisters[opCode.X] = (byte)(vRegisters[opCode.X] >> 1);
                            programCounter += 2;
                            break;
                        case 7:
                            var vYsubtractionResult = (int)vRegisters[opCode.Y] - vRegisters[opCode.X];

                            if (vYsubtractionResult < 0)
                            {
                                vYsubtractionResult += 256;
                                vRegisters[0xf] = (byte)0;
                            }
                            else
                            {
                                vRegisters[0xf] = (byte)1;
                            }

                            vRegisters[opCode.X] = (byte)vYsubtractionResult;
                            programCounter += 2;
                            break;
                        case 14:
                            vRegisters[0xf] = (byte)(vRegisters[opCode.X] & 0xf0000);
                            vRegisters[opCode.X] = (byte)(vRegisters[opCode.X] << 1);
                            programCounter += 2;
                            break;
                    }
                    break;
                case 0x9000:
                    if (vRegisters[opCode.Y] != vRegisters[opCode.X])
                        programCounter += 4;
                    else
                        programCounter += 2;
                    break;
                case 0xa000:
                    iRegister = (ushort)opCode.NNN;
                    programCounter += 2;
                    break;
                case 0xb000:
                    break;
                case 0xc000:
                    var random = new Random();
                    var randomNumber = (byte)random.Next(0, 255);
                    vRegisters[opCode.X] = (byte)(randomNumber & opCode.NN);
                    programCounter += 2;
                    break;
                case 0xd000:
                    SetVideoMemory(vRegisters[opCode.X], vRegisters[opCode.Y], opCode.N);
                    display.DrawFromMemory(memory);
                    drawFlag = true;
                    programCounter += 2;
                    break;
                case 0xe000:
                    switch (opCode.Code & 0xf0ff)
                    {
                        case 0xe09e:
                            if (keyboard.CheckIfKeyIsPressed(vRegisters[opCode.X]))
                                programCounter += 4;
                            else
                                programCounter += 2;
                            break;
                        case 0xe0a1:
                            if (!keyboard.CheckIfAnyKeyIsPressed() || !keyboard.CheckIfKeyIsPressed(vRegisters[opCode.X]))
                                programCounter += 4;
                            else
                                programCounter += 2;
                            break;
                    }
                    break;
                case 0xf000:
                    switch (opCode.Code & 0xf0ff)
                    {
                        case 0xf007:
                            vRegisters[opCode.X] = (byte)this.delayTimer.GetCurrentDelayTimer();
                            programCounter += 2;
                            break;
                        case 0xf00a:
                            if (!keyboard.CheckIfAnyKeyIsPressed())
                                break;

                            vRegisters[opCode.X] = (byte)keyboard.LastKeyPressed;
                            programCounter += 2;
                            break;
                        case 0xf015:
                            this.delayTimer.SetDelayTimer((byte)vRegisters[opCode.X]);
                            programCounter += 2;
                            break;
                        case 0xf018:
                            beeper.SetSoundDelayTimer(vRegisters[opCode.X]);
                            programCounter += 2;
                            break;
                        case 0xf029:
                            iRegister = (ushort)(vRegisters[opCode.X] * 5);
                            programCounter += 2;
                            break;
                        case 0xf01E:
                            iRegister += (byte)vRegisters[opCode.X];
                            programCounter += 2;
                            break;
                        case 0xf033:
                            memory[iRegister] = (byte)(vRegisters[opCode.X] / 100);
                            memory[iRegister + 1] = (byte)((vRegisters[opCode.X] / 10) % 10);
                            memory[iRegister + 2] = (byte)(vRegisters[opCode.X] % 10);
                            programCounter += 2;
                            break;
                        case 0xf055:
                            for (int i = 0; i <= opCode.X; i++)
                            {
                                memory[iRegister + i] = vRegisters[i];
                            }
                            programCounter += 2;
                            break;
                        case 0xf065:
                            for (int i = 0; i <= opCode.X; i++)
                            {
                                vRegisters[i] = memory[iRegister + i];
                            }
                            programCounter += 2;
                            break;
                        default: throw new Exception($"Not supported Op Code <{opCode.Code.ToString("X4")}>");
                    }
                    break;
                default: throw new Exception($"Not supported Op Code <{opCode.Code.ToString("X4")}>");
            }
        }



        public void SetVideoMemory(int vX, int vY, int height)
        {
            vRegisters[0xf] = 0;

            for (int row = 0; row < height; row++)
            {
                var spriteByte = memory[iRegister + row];

                var spriteBits = new BitArray(new byte[] { spriteByte });

                for (int c = 0; c < spriteBits.Count; c++)
                {
                    var pixelByte = (byte)((spriteByte >> (7 - c) | 0) & 0x1);

                    int index = 2048 + (vX + c + (vY + row) * 64);

                    if (index >= memory.Length)
                        index = 2048;

                    var oldPixel = memory[index];
                    memory[index] = (byte)(memory[index] ^ pixelByte);

                    if (oldPixel != 0 && memory[index] == 0)
                        vRegisters[0xF] = (byte)1;
                }
            }


        }

        public void ClearDisplayMemory()
        {
            for (int i = 0x800; i < memory.Length; i++)
            {
                memory[i] = (byte)0;
            }

            display.DrawFromMemory(memory);
            drawFlag = true;
        }

        private void ShutDown()
        {
            keyboard.ShutDown();
            delayTimer.ShutDown();
            beeper.ShutDown();
            this.running = false;
        }

        private void PrintDebugInfo(OpCode opCode)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            // header
            var header = $"Program counter: {programCounter.ToString().PadLeft(5)}";
            Console.WriteLine(header);
            // Timers
            var timers = $"Delay timer: {this.delayTimer.ToString()} | Sound delay timer: {this.beeper.ToString()}";
            Console.WriteLine(timers);
            // vRegisters
            Console.WriteLine("V registers:");
            var vRegistersHeader = "V0   | V1  | V2  | V3  | V4  | V5  | V6  | V7  | V8  | V9  | V10 | V11 | V12 | V13 | V14 | V15";
            var valueVRegistersHeader = $"{vRegisters[0].ToString("X4").PadRight(5)}|{vRegisters[1].ToString("X4").PadRight(5)}|{vRegisters[2].ToString("X4").PadRight(5)}|{vRegisters[3].ToString("X4").PadRight(5)}|{vRegisters[4].ToString("X4").PadRight(5)}|{vRegisters[5].ToString("X4").PadRight(5)}|{vRegisters[6].ToString("X4").PadRight(5)}|{vRegisters[7].ToString("X4").PadRight(5)}|{vRegisters[8].ToString("X4").PadRight(5)}|{vRegisters[9].ToString("X4").PadRight(5)}|{vRegisters[10].ToString("X4").PadRight(5)}|{vRegisters[11].ToString("X4").PadRight(5)}|{vRegisters[12].ToString("X4").PadRight(5)}|{vRegisters[13].ToString("X4").PadRight(5)}|{vRegisters[14].ToString("X4").PadRight(5)}|{vRegisters[15].ToString("X4").PadRight(5)}";
            Console.WriteLine(vRegistersHeader);
            Console.WriteLine(valueVRegistersHeader);
            Console.WriteLine($"Current Op Code: {opCode.Code.ToString("X4")}");
            Console.WriteLine(opCode.PrintOpCodeData());
            Console.WriteLine($"Keyoboard pressed: {this.keyboard.CheckIfAnyKeyIsPressed().ToString().PadRight(5)}");
            Console.WriteLine(this.keyboard.ToString());
        }


    }
}
