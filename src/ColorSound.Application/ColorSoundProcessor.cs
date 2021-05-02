using ColorSound.Process.Helper;
using ColorSound.Process.WaveProviders;
using ColorSound.Core.Synthesizers;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace ColorSound.Process
{
    public class ColorSoundProcessor : IDisposable
    {
        private static int SAMPLE_RATE = 22000;
        private static int LATENCY = 100;
        private static int SAMPLE_COLOR_COUNT = 48;
        private static int SAMPLE_COLOR_RANGE = 20;
        private static int NOISE_FACTOR = 20;

        private static Color[] DEFAULT_COLOR_SET = SAMPLE_COLOR_COUNT.GenerateDefaultColorArray();

        private ImageCapture _imageCapture;

        private GeneralWaveProvider<Synthesizer1> _wave1;
        private GeneralWaveProvider<Harmonica> _wave2;
        private IGeneralWaveProvider[] _waves;

        private WasapiOutRT _waveOutput1;
        private WasapiOutRT _waveOutput2;
        private WasapiOutRT[] _waveOutputs;

        // runtime values
        private List<Color[]> _buffer = new List<Color[]>();
        private Color[] _lastProcessed = DEFAULT_COLOR_SET;
        private bool _output = false;
        private long[] _counters = new long[SAMPLE_COLOR_COUNT];

        public event EventHandler<SoftwareBitmap> FrameUpdated;

        public event EventHandler<Color[]> ColorUpdated;

        public ColorSoundProcessor(double amplitude)
        {
            _imageCapture = new ImageCapture();
            _imageCapture.FrameUpdated += OnFrameUpdated;

            _wave1 = new GeneralWaveProvider<Synthesizer1>(new Synthesizer1 { Amplitude = amplitude }, SAMPLE_RATE, 1);
            _waveOutput1 = new WasapiOutRT(AudioClientShareMode.Shared, LATENCY);
            _waveOutput1.Init(() => _wave1);

            _wave2 = new GeneralWaveProvider<Harmonica>(new Harmonica { Amplitude = amplitude }, SAMPLE_RATE, 2);
            _waveOutput2 = new WasapiOutRT(AudioClientShareMode.Shared, LATENCY);
            _waveOutput2.Init(() => _wave2);

            _waves = new IGeneralWaveProvider[] { _wave1, _wave2 };
            _waveOutputs = new WasapiOutRT[] { _waveOutput1, _waveOutput2 };
        }

        protected virtual void OnFrameUpdated(object sender, SoftwareBitmap e)
        {
            FrameUpdated?.Invoke(this, e);
        }

        public void Dispose()
        {
            _imageCapture.Dispose();
            _waveOutput1.Dispose();
            _waveOutput2.Dispose();
        }

        public double Amplitude
        {
            get => _waves[0].Synthesizer.Amplitude;
            set => Array.ForEach(_waves, wave => wave.Synthesizer.Amplitude = value);
        }

        public Color[] ProcessedColor => _lastProcessed;

        public async Task RunProcessAsync(bool output)
        {
            if (output != _output)
            {
                _output = output;
                if (_output)
                {
                    Array.ForEach(_waveOutputs, wave => wave.Play());
                    await _imageCapture.StartAsync();
                }
                else
                {
                    Array.ForEach(_waveOutputs, wave => wave.Pause());
                    await _imageCapture.StopAsync();
                }
            }

            if (!output)
            {
                return;
            }

            if (_buffer.Count < 5)
            {
                var imagePixel = _imageCapture.ImagePixelData;

                if (imagePixel != null)
                {
                    var sampleHeight = (int)imagePixel.Height / SAMPLE_COLOR_COUNT;
                    _buffer.Add(imagePixel.GetReducedColor(SAMPLE_COLOR_RANGE, sampleHeight));
                }
                else
                {
                    _buffer.Add(DEFAULT_COLOR_SET);
                }

                return;
            }

            var colorSet = _buffer.Average();
            _buffer.Clear();

            var wave1Point = (int)(SAMPLE_COLOR_COUNT * 0.4);
            var wave2Point = (int)(SAMPLE_COLOR_COUNT * 0.6);

            ProcessWave(colorSet, _wave1, wave1Point, 10, (color, last) => GetColorFactors(new Color[] { color, last }.Average()));
            ProcessWave(colorSet, _wave2, wave2Point, 15, (color, last) => GetColorFactors(color));

            ColorUpdated?.Invoke(this, new Color[] { _lastProcessed[wave1Point], _lastProcessed[wave2Point]  });
        }

        private void ProcessWave(Color[] colorSet, IGeneralWaveProvider wave, int index, int idle, Func<Color, Color, double[]> iterator)
        {
            var color = colorSet[index];

            if (IsColorUpdated(_lastProcessed[index], color))
            {
                var value = iterator(color, _lastProcessed[index]);
                wave.Play(value, color.GetBrightness());
                _lastProcessed[index] = color;
                _counters[index] = 0;
            }
            else if (_counters[index] > idle)
            {
                wave.Pause();
                _counters[index] = 0;
            }
            else
            {
                _counters[index]++;
            }
        }

        private double[] GetColorFactors(Color color)
        {
            var hue = color.GetHue();
            var saturation = color.GetSaturation();

            var keyBase = 1.0594630943592952645618252949463;
            var factor = 256 * Math.Pow(saturation, 0.2) / 5;
            var node = Math.Pow(Math.Abs(255 - hue), 0.8);

            var keyValue1 = factor * Math.Pow(keyBase, node / 10);
            var keyValue2 = factor * Math.Pow(keyBase, node / 10 + 12);
            var keyValue3 = factor * Math.Pow(keyBase, node / 10 + 24);

            return new double[] { keyValue1, keyValue2, keyValue3 };
        }


        private bool IsColorUpdated(Color value1, Color value2)
        {
            if (Math.Abs(value1.A - value2.A) > NOISE_FACTOR)
                return true;

            if (Math.Abs(value1.R - value2.R) > NOISE_FACTOR)
                return true;

            if (Math.Abs(value1.G - value2.G) > NOISE_FACTOR)
                return true;

            if (Math.Abs(value1.B - value2.B) > NOISE_FACTOR)
                return true;

            return false;
        }
    }
}
