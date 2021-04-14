using ColorSound.Core.Oscillators;
using System;

namespace ColorSound.Core.WaveProviders
{

    public class KeyWaveProvider : WaveProvider32
    {
        int sample;

        public KeyWaveProvider()
        {
            Amplitude = 0.25f; // let's not hurt our ears
            Key = 0;
        }

        public float Amplitude { get; set; }

        public int Key { get; set; }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;

            var freq1 = Scale(Key);
            var freq2 = Scale(Key + 12);
            var freq3 = Scale(Key + 24);

            for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = Amplitude * (float)(Oscillator.GetValue(sample, freq1, sampleRate, OscillatorType.SINE, 5, 0.001)
                   + 0.5 * Oscillator.GetValue(sample, freq1, sampleRate, OscillatorType.TRIANGLE, 3, 0.001)
                   + 0.1 * Oscillator.GetValue(sample, freq2, sampleRate, OscillatorType.SAW_ANA)
                   + 0.005 * Oscillator.GetValue(sample, freq3, sampleRate, OscillatorType.NOISE));

                sample++;
                if (sample >= sampleRate) sample = 0;
            }

            return sampleCount;
        }

        private double Scale(int nNoteID, int nScaleID = 0)
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
