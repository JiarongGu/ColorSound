using ColorSound.Core.Synthesizers;
using ColorSound.Core.Waves;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColorSound.Console.WaveProviders
{
    public interface IGeneralWaveProvider
    {
        SynthesizerBase Synthesizer { get; }

        int SampleRate { get; set; }

        int Channels { get; set; }

        void Play(double[] frequencies, double? amplitude);

        void Pause();

        int Read(float[] buffer, int offset, int sampleCount);
    }

    public class GeneralWaveProvider<T> : WaveProvider32, IGeneralWaveProvider where T : SynthesizerBase
    {
        private GeneralWave _wave;
        private int _sampleRate;

        public GeneralWaveProvider(T synthesizer, int sampleRate, int channels) : base(sampleRate, channels)
        {
            _wave = new GeneralWave(synthesizer);
            _sampleRate = sampleRate;
            Synthesizer = synthesizer;
        }

        public T Synthesizer { get; }

        public int SampleRate
        {
            get => WaveFormat.SampleRate;
            set => SetWaveFormat(value, WaveFormat.Channels);
        }

        public int Channels
        {
            get => WaveFormat.Channels;
            set => SetWaveFormat(WaveFormat.SampleRate, value);
        }

        SynthesizerBase IGeneralWaveProvider.Synthesizer => Synthesizer;

        public void Play(double[] frequencies, double? amplitudeFactor)
        {
            _wave.Play(frequencies);

            if (amplitudeFactor != null)
            {
                _wave.Synthesizer.AmplitudeFactor = amplitudeFactor.Value;
            }
        }

        public void Pause()
        {
            _wave.Pause();
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            return _wave.Read(buffer, offset, sampleCount, _sampleRate);
        }
    }
}
