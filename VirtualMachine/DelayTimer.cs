using System;
using System.Threading.Tasks;

namespace Emmetienne.ChipOtto.VirtualMachine
{
    public class DelayTimer
    {
        public byte delayTimer;
        public bool running;

        public DelayTimer()
        {
            Console.WriteLine("Delay timer initialized");
            running = true;
        }

        public void SetDelayTimer(byte delayValue)
        {
            delayTimer = delayValue;
        }

        public byte GetCurrentDelayTimer()
        {
            return this.delayTimer;
        }

        public async void Execute()
        {
            while (running)
            {
                await Task.Delay(16);
                if (delayTimer == 0)
                    continue;

                delayTimer--;
            }
        }
        public void ShutDown()
        {
            running = false;
        }

        public override string ToString()
        {
            return this.delayTimer.ToString().PadLeft(5);
        }
    }
}
