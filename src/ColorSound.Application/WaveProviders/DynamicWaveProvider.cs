using ColorSound.Core.Synthesizers;
using System.Collections.Generic;
using System.Linq;

namespace ColorSound.Process.WaveProviders
{
    public class DynamicWaveProvider : WaveProvider32
    {
        private long sample;

        public DynamicWaveProvider(int sampleRate, int channels) : base(sampleRate, channels)
        {
            Synthesizers = new List<SynthesizerBase>();
        }

        public IList<SynthesizerBase> Synthesizers { get; }

        public void Play(double[][] frequencies)
        {
            for (var i = 0; i < Synthesizers.Count; i++)
            {
                Synthesizers[i].Play(frequencies[i]);
            }
        }

        public void Pause()
        {
            foreach (var synthesizer in Synthesizers)
            {
                synthesizer.Pause();
            }
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            for (int n = 0; n < sampleCount; n++)
            {
                var time = (double)sample / WaveFormat.SampleRate;
                buffer[n + offset] = (float)Synthesizers.Select(s => s.Next(time)).Average();
                sample++;
            }

            return sampleCount;
        }
    }
}
