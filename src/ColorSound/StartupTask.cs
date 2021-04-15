using System;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Devices.Gpio;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using ColorSound.Application.WaveProviders;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using Windows.Storage;
using Windows.Media.Playback;
using System.Threading;

using GrovePi;
using Windows.Media.Core;

namespace ColorSound
{
    public sealed class StartupTask : IBackgroundTask
    {
        KeyWaveProvider keyWaveProvider = new KeyWaveProvider();
        ThreeWaveProvider threeWaveProvider = new ThreeWaveProvider();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // PlayAssetMusic("Tokyo Teddy Bear - Hatsune Miku.mp3");

            var waveOut1 = new WasapiOutRT(AudioClientShareMode.Shared, 200);
            keyWaveProvider.SetWaveFormat(16000, 1);
            keyWaveProvider.Key = 0;

            waveOut1.Init(() => keyWaveProvider);
            // waveOut1.Play();

            var waveOut2 = new WasapiOutRT(AudioClientShareMode.Shared, 200);
            threeWaveProvider.SetWaveFormat(16000, 2);
            waveOut2.Init(() => threeWaveProvider);
            waveOut2.Play();

            var rotarySensor = DeviceFactory.Build.RotaryAngleSensor(Pin.AnalogPin0);
            var accelerometer = DeviceFactory.Build.ThreeAxisAccelerometerADXL345();
            var display = DeviceFactory.Build.RgbLcdDisplay();
            
            // init devices
            display.SetBacklightRgb(255, 255, 255);
            accelerometer.Initialize();

            var timer = new System.Timers.Timer(100);
            timer.Elapsed += (o, e) =>
            {
                if (keyWaveProvider.Key > 40)
                {
                    keyWaveProvider.Key = 0;
                }
                keyWaveProvider.Key++;
            };

            timer.Start();

            var lastXyz = accelerometer.GetAcclXYZ();

            while (true)
            {
                // degree from 0 - 300
                // amplitude from 0 - 1
                var degrees = rotarySensor.Degrees();
                var xyz = accelerometer.GetAcclXYZ();

                keyWaveProvider.Amplitude = Math.Pow(2, degrees / 300) - 1;

                if (IsAcclUpdated(lastXyz, xyz))
                {
                    threeWaveProvider.Play(xyz[0] * 1000, xyz[1] * 1000, xyz[2] * 1000);
                    lastXyz = xyz;
                }
                else 
                {
                    threeWaveProvider.Pause();
                }

                display.SetText($"d:{degrees}, x:{xyz[0]}, y:{xyz[1]}, z:{xyz[2]}");

                Thread.Sleep(10);
            }
        }

        private static bool IsAcclUpdated(double[] value1, double[] value2) 
        {
            var noise = 0.12;

            if (Math.Abs(value1[0] - value2[0]) > noise)
                return true;

            if (Math.Abs(value1[1] - value2[1]) > noise)
                return true;

            if (Math.Abs(value1[2] - value2[2]) > noise)
                return true;

            return false;
        }

        private static async void PlayAssetMusic(string fileName) {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{fileName}"));
            var player = BackgroundMediaPlayer.Current;
            player.AutoPlay = false;
            player.Source = MediaSource.CreateFromStorageFile(file);
            player.Play();
        }
    }
}
