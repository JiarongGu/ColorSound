using ColorSound.Core.Synthesizers;
using ColorSound.Core.Waves;
using System;

namespace ColorSound.WaveProviders
{
    public class KeyWaveProvider : WaveProvider32
    {
        private KeyWave _wave;
        private int _sampleRate;

        public KeyWaveProvider(SynthesizerBase synthesizer, int sampleRate, int channels) : base(sampleRate, channels)
        {
            _wave = new KeyWave(synthesizer);
            _sampleRate = sampleRate;
        }

        public double Key { get => _wave.Key; set => _wave.Key = value; }

        public void Play(double key)
        {
            _wave.Play(key);
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
