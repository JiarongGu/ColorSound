using ColorSound.Core.Synthesizers;
using ColorSound.Helper;
using ColorSound.WaveProviders;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ColorSound
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }


        private async void MainPage_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            var sampleRate = 22000;
            var latency = 200;

            var waveProvider1 = new GeneralWaveProvider(new Synthesizer1 { Amplitude = 0.3 }, sampleRate, 1);
            var waveOut1 = new WasapiOutRT(AudioClientShareMode.Shared, latency);
            waveOut1.Init(() => waveProvider1);
            waveOut1.Play();

            var waveProvider2 = new GeneralWaveProvider(new Harmonica(), sampleRate, 2);
            var waveOut2 = new WasapiOutRT(AudioClientShareMode.Shared, latency);
            waveOut2.Init(() => waveProvider2);
            waveOut2.Play();

            var imageCapture = new ImageCapture();
            await imageCapture.StartAsync();

            _ = Task.Run(() =>
            {
                var total = 16;
                var lastColorSet = GenerateEmptyColorSet(total);
                var colorBuffer = new List<Color[]>();

                while (true)
                {
                    if (colorBuffer.Count < 4)
                    {
                        var imagePixel = imageCapture.ImagePixelData;

                        if (imagePixel != null)
                        {
                            colorBuffer.Add(GetReducedPixel(imagePixel, 40, (int)imagePixel.Height / total));
                        }
                        else 
                        {
                            colorBuffer.Add(GenerateEmptyColorSet(total));
                        }
                    }
                    else
                    {
                        var colorSet = colorBuffer.Average();

                        var color1 = colorSet[0];
                        var color2 = colorSet[5];

                        colorBuffer.Clear();

                        // var updated = colorSet.Select((color, index) => IsColorUpdated(lastColorSet[index], color)).Any(x => x);

                        if (IsColorUpdated(lastColorSet[0], color1))
                        {
                            waveProvider1.Play(GetColorFactors(color1));
                            lastColorSet[0] = color1;
                        }

                        if (IsColorUpdated(lastColorSet[5], color2))
                        {
                            waveProvider2.Play(GetColorFactors(new Color[] { lastColorSet[5], color2 }.Average()));
                            lastColorSet[5] = color2;
                        }
                    }
                    Thread.Sleep(10);
                }
            });
        }

        private double[] GetColorFactors(Color color) 
        {
            var hue = color.GetHue();
            var saturation = color.GetSaturation();
            var value = Math.Sqrt(Math.Pow(color.R, 2) + Math.Pow(color.G, 2) + Math.Pow(color.B, 2));
            return new double[] { value, hue, saturation };
        }

        private Color[] GenerateEmptyColorSet(int total) 
        {
            return Enumerable.Range(0, total).Select(i => Color.FromArgb(0, 0, 0)).ToArray();
        }

        private bool IsColorUpdated(Color value1, Color value2)
        {
            var noise = 5;

            if (Math.Abs(value1.A - value2.A) > noise)
                return true;

            if (Math.Abs(value1.R - value2.R) > noise)
                return true;

            if (Math.Abs(value1.G - value2.G) > noise)
                return true;

            if (Math.Abs(value1.B - value2.B) > noise)
                return true;

            return false;
        }

        public Color[] GetReducedPixel(ImagePixelData data, int width, int height)
        {
            var total = data.Height / height;
            var color = new Color[total];

            for (var i = 0; i < total; i++)
            {
                var heightOffset = height * i;
                var flattenColor = data.GetFlattenColor(0, width, heightOffset, heightOffset + height);
                color[i] = flattenColor.Average();
            }

            return color;
        }

    }
}
