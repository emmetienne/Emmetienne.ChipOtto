using System;
using System.Media;
using System.Threading.Tasks;

namespace Emmetienne.ChipOtto.VirtualMachine
{
    public class Beeper
    {
        private bool running;
        public byte soundDelayTimer;
        private byte lastSoundDelay;
        private readonly SoundPlayer soundPlayer;

        public Beeper(string beepFilePath)
        {
            Console.WriteLine("Beeper initialized");

            if (!string.IsNullOrWhiteSpace(beepFilePath))
                this.soundPlayer = new SoundPlayer(beepFilePath);
            else
                this.soundPlayer = new SoundPlayer($"{AppContext.BaseDirectory}/Data/Beep.wav");

            soundDelayTimer = 0;

            running = true;
        }

        public void SetSoundDelayTimer(byte delayValue)
        {
            soundDelayTimer = delayValue;
        }

        public async void Execute()
        {
            while (running)
            {
                await Task.Delay(16);
                if (soundDelayTimer == 0)
                {
                    soundPlayer.Stop();
                    lastSoundDelay = (byte)0;
                    continue;
                }

                if (lastSoundDelay == 0)
                    soundPlayer.PlayLooping();

                lastSoundDelay = soundDelayTimer;

                soundDelayTimer--;
            }
        }

        public void ShutDown()
        {
            running = false;
        }

        public override string ToString()
        {
            return this.soundDelayTimer.ToString().PadLeft(5);
        }
    }
}
