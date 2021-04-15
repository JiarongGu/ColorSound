
using ColorSound.Application.WaveProviders;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Timers;

// This example code shows how you could implement the required main function for a 
// Console UWP Application. You can replace all the code inside Main with your own custom code.

// You should also change the Alias value in the AppExecutionAlias Extension in the 
// Package.appxmanifest to a value that you define. To edit this file manually, right-click
// it in Solution Explorer and select View Code, or open it with the XML Editor.

namespace ColorSound.Console
{
    class Program
    {
        static KeyWaveProvider waveProvider = new KeyWaveProvider();

        static void Main(string[] args)
        {
            var waveOut = new WasapiOutRT(AudioClientShareMode.Shared, 200);

            waveProvider.SetWaveFormat(44100, 1); // 16kHz mono
            waveProvider.Key = 0;

            waveOut.Init(() => waveProvider);

            var timer = new Timer(200);

            timer.Elapsed += OnTimedEvent;

            timer.Start();

            waveOut.Play();

            System.Console.ReadLine();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (waveProvider.Key > 40)
            {
                waveProvider.Key = 0;
            }
            waveProvider.Key += 1;
        }
    }
}
