using System;

namespace Emmetienne.ChipOtto.Model
{
    public class OpCode
    {
        public ushort Code { get;set; }
        public ushort Nibble { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public ushort N { get; set; }
        public ushort NN { get; set; }
        public ushort NNN { get; set; }

        public OpCode(ushort opCode)
        {
            this.Code = opCode;
            this.Nibble = (ushort)(opCode & 0xF000);
            this.X = (byte)((opCode & 0x0f00) >> 8);
            this.Y = (byte)((opCode & 0x00f0) >> 4);
            this.N = (ushort)(0x000f & opCode);
            this.NN = (ushort)(0x00ff & opCode);
            this.NNN = (ushort)(0x0fff & opCode);
        }

        public string PrintOpCodeData()
        {
            return $"OpCode | Nibble | X  | Y  | N  | NN  | NNN  {Environment.NewLine}{this.Code.ToString("X4")}   |  {this.Nibble.ToString("X4")}  | {this.X.ToString().PadRight(2, ' ')} | {this.Y.ToString().PadRight(2, ' ')} | {this.N.ToString().PadRight(2, ' ')} | {this.NN.ToString().PadRight(3, ' ')} | {this.NNN.ToString().PadRight(4,' ')} ";
        }
    }
}
