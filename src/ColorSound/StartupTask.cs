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
        private static int SAMPLE_RATE = 50;
        private static int MAX_IDLE = 2 * 1000 / 50; // 2 seconds

        KeyWaveProvider keyWaveProvider = new KeyWaveProvider();
        ThreeWaveProvider threeWaveProvider = new ThreeWaveProvider();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // PlayAssetMusic("Tokyo Teddy Bear - Hatsune Miku.mp3");

            var waveOut1 = new WasapiOutRT(AudioClientShareMode.Shared, SAMPLE_RATE * 4);
            keyWaveProvider.SetWaveFormat(44100, 1);
            waveOut1.Init(() => keyWaveProvider);
            // waveOut1.Play();

            var waveOut2 = new WasapiOutRT(AudioClientShareMode.Shared, SAMPLE_RATE * 4);
            threeWaveProvider.SetWaveFormat(44100, 2);
            waveOut2.Init(() => threeWaveProvider);
            waveOut2.Play();

            var accelerometer = DeviceFactory.Build.ThreeAxisAccelerometerADXL345();
            var display = DeviceFactory.Build.RgbLcdDisplay();

            // init devices
            accelerometer.Initialize();
                

            var lastXyz = accelerometer.GetAcclXYZ();
            var last = 0;

            while (true)
            {
                var xyz = accelerometer.GetAcclXYZ();

                if (IsAcclUpdated(lastXyz, xyz))
                {
                    threeWaveProvider.Play(xyz[0] * 1000, xyz[1] * 1000, xyz[2] * 1000);

                    var key = Math.Sqrt(
                        + Math.Pow(xyz[0] - lastXyz[0], 2) 
                        + Math.Pow(xyz[1] - lastXyz[1], 2) 
                        + Math.Pow(xyz[2] - lastXyz[2], 2)
                    ) * 20;

                    // var key = Math.Sqrt(Math.Pow(xyz[0], 2) + Math.Pow(xyz[1], 2) + Math.Pow(xyz[2], 2)) * 5;
                    display.SetText($"Key: {key}");

                    keyWaveProvider.Play(key);
                    lastXyz = xyz;
                    last = 0;
                }
                else if (last > MAX_IDLE)
                {
                    threeWaveProvider.Pause();
                    keyWaveProvider.Pause();
                }
                else 
                {
                    last += SAMPLE_RATE;
                }


                Thread.Sleep(SAMPLE_RATE);
            }
        }

        private static bool IsAcclUpdated(double[] value1, double[] value2) 
        {
            var noise = 0.10;

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
