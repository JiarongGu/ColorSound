using ColorSound.Application.Components;
using System;

namespace ColorSound.Application.WaveProviders
{

    public class KeyWaveProvider : WaveProvider32
    {
        int sample;

        public KeyWaveProvider()
        {
            Amplitude = 0.25f;
            Key = 0;
        }

        public double Amplitude { get; set; }

        public double Key { get; set; }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;

            var freq1 = Scale(Key);
            var freq2 = Scale(Key + 12);
            var freq3 = Scale(Key + 24);

            for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = (float)Amplitude * (float)(Oscillator.GetValue(sample / sampleRate, freq1, OscillatorType.SINE, 5, 0.001)
                   + 0.5 * Oscillator.GetValue(sample / sampleRate, freq1, OscillatorType.TRIANGLE, 3, 0.001)
                   + 0.1 * Oscillator.GetValue(sample / sampleRate, freq2, OscillatorType.SAW_ANA)
                   + 0.005 * Oscillator.GetValue(sample / sampleRate, freq3, OscillatorType.NOISE));

                sample++;
                if (sample >= int.MaxValue) sample = 0;
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
