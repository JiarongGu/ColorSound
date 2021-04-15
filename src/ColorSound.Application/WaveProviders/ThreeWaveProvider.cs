using ColorSound.Application.Components;
using System;
using System.Collections.Generic;

namespace ColorSound.Application.WaveProviders
{

    public class ThreeWaveProvider : WaveProvider32
    {
        private int sample = 0;
        private int envelopeOn = 0;
        private int envelopeOff = 0;

        private Envelope envelope = new Envelope();

        public ThreeWaveProvider()
        {
            Amplitude = 0.5;
        }

        public double Amplitude { get; set; }

        public double Frequence1 { get; private set; }

        public double Frequence2 { get; private set; }

        public double Frequence3 { get; private set; }

        public void Play(double freq1, double freq2, double freq3)
        {
            Frequence1 = freq1;
            Frequence2 = freq2;
            Frequence3 = freq3;

            envelopeOn = sample / WaveFormat.SampleRate;
            envelopeOff = 0;
        }

        public void Pause()
        {
            if (envelopeOff == 0) { 
                envelopeOff = sample / WaveFormat.SampleRate;
            }
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;

            for (int n = 0; n < sampleCount; n++)
            {
                var amplitude = envelope.Amplitude(sample / sampleRate, envelopeOn, envelopeOff) * Amplitude;

                buffer[n + offset] = (float)amplitude * (float)(
                   + 0.5 * Oscillator.GetValue(sample, Frequence1, sampleRate, OscillatorType.SINE, 5, 0.001)
                   + 0.3 * Oscillator.GetValue(sample, Frequence2, sampleRate, OscillatorType.SQUARE, 5, 0.001)
                   + 0.2 * Oscillator.GetValue(sample, Frequence3, sampleRate, OscillatorType.TRIANGLE, 5, 0.001)
                );

                sample++;
                if (sample >= int.MaxValue) sample = 0;
            }

            return sampleCount;
        }
    }
}
