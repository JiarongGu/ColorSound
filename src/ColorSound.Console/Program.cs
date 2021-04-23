using ColorSound.Console.WaveProviders;
using ColorSound.Core.Synthesizers;
using NAudio.Wave;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace ColorSound.Console
{
    public class Program
    {
        static KeyWaveProvider sync1 = new KeyWaveProvider(new DrumKick(), 44100, 1);
        static KeyWaveProvider sync2 = new KeyWaveProvider(new Harmonica(), 44100, 1);
        static int count = 0;

        static void Main(string[] args)
        {
            var waveOut1 = new WaveOut();
            var waveOut2 = new WaveOut();

            sync1.SetWaveFormat(44100, 1); // 16kHz mono
            sync2.SetWaveFormat(44100, 1);

            waveOut1.Init(sync1);
            waveOut2.Init(sync2);

            var timer = new Timer(200);

            timer.Elapsed += OnTimedEvent;

            timer.Start();

            waveOut1.Play();
            // waveOut2.Play();

            System.Console.ReadLine();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (count % 2 == 0)
            {
                if (sync1.Key > 32)
                {
                    sync1.Play(0);
                }
                else
                {
                    sync1.Play(sync1.Key + 1);
                }
                sync2.Pause();
            }
            else
            {
                if (sync2.Key > 32)
                {
                    sync2.Play(0);
                }
                else
                {
                    sync2.Play(sync2.Key + 1);
                }
                sync1.Pause();
            }

            count++;
        }
    }
}
