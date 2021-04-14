using ColorSound.Core;
using ColorSound.Core.Oscillators;
using ColorSound.Core.WaveProviders;
using NAudio.Midi;
using NAudio.Wave;
using System;
using System.Timers;

namespace ColorSound.Console
{
    class Program
    {
        static KeyWaveProvider waveProvider = new KeyWaveProvider();

        static void Main(string[] args)
        {
            System.Console.WriteLine($"Audio Devices {WaveOut.DeviceCount}:");

            for (int device = 0; device < WaveOut.DeviceCount; device++)
            {
                System.Console.WriteLine($"{device}: {WaveOut.GetCapabilities(device).ProductName}");
            }

            waveProvider.SetWaveFormat(44100, 1); // 16kHz mono
            waveProvider.Key = 0;

            var waveOut = new WaveOut();

            waveOut.Init(waveProvider);

            var timer = new Timer(200);

            timer.Elapsed += OnTimedEvent;

            timer.Start();

            waveOut.Play();

            System.Console.ReadLine();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (waveProvider.Key > 40) {
                waveProvider.Key = 0;
            }
            waveProvider.Key += 1;
        }
    }
}
