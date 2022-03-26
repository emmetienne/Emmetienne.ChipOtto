using System;
using System.IO;
using System.Windows.Forms;

namespace Emmetienne.ChipOtto.Services
{
    public class RomLoaderService
    {
        public RomLoaderService()
        {
            Console.WriteLine($"{nameof(RomLoaderService)} initialized");
        }

        public byte[] ReadRomFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Chip-8 rom (*.ch8)|*.ch8|Chip-8 rom (*.c8)|*.c8|All files (*.*)|*.*";

                if (DialogResult.OK == fileDialog.ShowDialog())
                    filePath = fileDialog.FileName;
                else
                    throw new Exception("No file has been selected");
            }

            Console.WriteLine($"Reading rom from {filePath}");

            var romByteArray = File.ReadAllBytes(filePath);

            Console.WriteLine(BitConverter.ToString(romByteArray));

            return romByteArray;
        }

        private void PrintRomOpCodes(ushort[] romOpCodes)
        {
            Console.WriteLine("OpCodes list:");
            foreach (var opCode in romOpCodes)
            {
                Console.Write($"[{opCode.ToString("X4")}]");
            }
            Console.WriteLine(string.Empty);
        }
    }
}
