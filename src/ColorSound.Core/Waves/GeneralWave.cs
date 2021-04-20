using ColorSound.Core.Synthesizers;

namespace ColorSound.Core.Waves
{
    public class GeneralWave: IWave
    {
        private long _sample;

        public GeneralWave(SynthesizerBase synthesizer)
        {
            Synthesizer = synthesizer;
        }

        public SynthesizerBase Synthesizer { get; }

        public double Key { get; set; }

        public void Play(double[] frequencies)
        {
            Synthesizer.Play(frequencies);
        }

        public void Pause()
        {
            Synthesizer.Pause();
        }

        public int Read(float[] buffer, int offset, int sampleCount, int sampleRate)
        {
            for (int n = 0; n < sampleCount; n++)
            {
                var time = (double)_sample / sampleRate;
                buffer[n + offset] = (float)Synthesizer.Next(time);
                _sample++;
            }

            return sampleCount;
        }

    }
}
