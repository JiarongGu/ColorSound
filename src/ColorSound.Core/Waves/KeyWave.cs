using ColorSound.Core.Synthesizers;
using System;

namespace ColorSound.Core.Waves
{
    public class KeyWave: IWave
    {
        private long _sample;

        public KeyWave(SynthesizerBase synthesizer)
        {
            Synthesizer = synthesizer;
        }

        public SynthesizerBase Synthesizer { get; }

        public double Key { get; set; }

        public void Play(double key)
        {
            Key = key;

            var freq1 = Scale(key);
            var freq2 = Scale(key + 12);
            var freq3 = Scale(key + 24);

            Synthesizer.Play(new double[] { freq1, freq2, freq3 });
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

        private double Scale(double nNoteID, int nScaleID = 0)
        {
            switch (nScaleID)
            {
                case 0:
                default:
                    return 256 * Math.Pow(1.0594630943592952645618252949463, nNoteID);
            }
        }
    }
}
