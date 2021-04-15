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
using System.Threading;

using GrovePi;

namespace ColorSound
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static int SAMPLE_RATE = 50;
        private static int MAX_IDLE = 2 * 1000 / 50; // 2 seconds

        ThreeWaveProvider threeWaveProvider = new ThreeWaveProvider();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var waveOut = new WasapiOutRT(AudioClientShareMode.Shared, SAMPLE_RATE);
            threeWaveProvider.SetWaveFormat(44100, 1);
            waveOut.Init(() => threeWaveProvider);
            waveOut.Play();

            var accelerometer = DeviceFactory.Build.ThreeAxisAccelerometerADXL345();

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
                    lastXyz = xyz;
                    last = 0;
                }
                else if (last > MAX_IDLE)
                {
                    threeWaveProvider.Pause();
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
    }
}
