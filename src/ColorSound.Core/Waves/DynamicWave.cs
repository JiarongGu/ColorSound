using ColorSound.Core.Synthesizers;
using System.Collections.Generic;
using System.Linq;

namespace ColorSound.Core.Waves
{
    public class DynamicWave: IWave
    {
        private long _sample;

        public DynamicWave()
        {
            Synthesizers = new List<SynthesizerBase>();
        }

        public IReadOnlyList<SynthesizerBase> Synthesizers { get; }

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

        public int Read(float[] buffer, int offset, int sampleCount, int sampleRate)
        {
            for (int n = 0; n < sampleCount; n++)
            {
                var time = (double)_sample / sampleRate;
                buffer[n + offset] = (float)Synthesizers.Select(s => s.Next(time)).Average();
                _sample++;
            }

            return sampleCount;
        }
    }
}
