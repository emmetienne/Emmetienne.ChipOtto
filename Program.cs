using Emmetienne.ChipOtto.Services;
using Emmetienne.ChipOtto.VirtualMachine;
using System;

namespace Emmetienne.ChipOtto
{ 
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "ChipOtto";

            Console.WriteLine("Chip 8 VM started");

            // Read configurations
            var configurationService = new ConfigurationService();
            var configurations = configurationService.GetConfiguration();

            // Load Rom from file
            var romLoaderService = new RomLoaderService();
            var romData = romLoaderService.ReadRomFromFile(configurations.DefaultRomPath);

            // Initialize component to be passed to the cpu
            var display = new Display(configurations.DisplaySettings);
            var keyBoard = new Keyboard();
            var beeper = new Beeper(configurations.BeepSoundPath);
            var delayTimer = new DelayTimer();

            var cpu = new Cpu(display, keyBoard, beeper, delayTimer, romData, configurations.Debug);
            cpu.Execute();
        }
    }
}
