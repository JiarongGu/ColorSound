using ColorSound.Core.Synthesizers;
using ColorSound.Core.Waves;

namespace ColorSound.WaveProviders
{
    public class GeneralWaveProvider: WaveProvider32
    {
        private GeneralWave _wave;
        private int _sampleRate;

        public GeneralWaveProvider(SynthesizerBase synthesizer, int sampleRate, int channels) : base(sampleRate, channels)
        {
            _wave = new GeneralWave(synthesizer);
            _sampleRate = sampleRate;
        }

        public double Key { get => _wave.Key; set => _wave.Key = value; }

        public void Play(double[] frequencies)
        {
            _wave.Play(frequencies);
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
