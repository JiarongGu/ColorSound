using ColorSound.Core.Synthesizers;
using ColorSound.WaveProviders;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ColorSound
{
    public sealed partial class MainPage : Page
    {
        private int count;

        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }


        private async void MainPage_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            var waveProvider = new GeneralWaveProvider(new Harmonica(), 44100, 1);
            var waveOut = new WasapiOutRT(AudioClientShareMode.Shared, 50);
            var imageCapture = new ImageCapture();
            await imageCapture.StartAsync();

            waveOut.Init(() => waveProvider);
            waveOut.Play();

            //var timer = new Timer(200);

            //timer.Elapsed += OnTimedEvent;

            //timer.Start();

            Color lastColor = Color.FromArgb(0, 0, 0);

            _ = Task.Run(async () =>
              {
                  while (true)
                  {
                      var imagePixel = await imageCapture.GetImagePixelDataAsync();
                      Color color = imagePixel != null ? GetReducedPixel(imagePixel, 10, (int)imagePixel.Height / 16)[8] : Color.FromArgb(0, 0, 0);

                      if (IsColorUpdated(lastColor, color))
                      {
                          var hue = color.GetHue();
                          var saturation = color.GetSaturation();
                          var value = Math.Sqrt(Math.Pow(color.R, 2) + Math.Pow(color.G, 2) + Math.Pow(color.B, 2));

                          waveProvider.Play(new double[] { value, hue, saturation });
                          lastColor = color;
                      }

                      Thread.Sleep(10);
                  }
              });
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

                var red = flattenColor.Select(c => (int)c.R).Average();
                var green = flattenColor.Select(c => (int)c.G).Average();
                var blue = flattenColor.Select(c => (int)c.B).Average();
                var alpha = flattenColor.Select(c => (int)c.A).Average();

                color[i] = Color.FromArgb((int)alpha, (int)red, (int)green, (int)blue);
            }

            return color;
        }

    }
}
