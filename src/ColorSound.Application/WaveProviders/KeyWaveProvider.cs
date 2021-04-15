using ColorSound.Application.Components;
using System;

namespace ColorSound.Application.WaveProviders
{

    public class KeyWaveProvider : WaveProvider32
    {
        private int sample;
        private int envelopeOn = 0;
        private int envelopeOff = 0;
        private Envelope envelope = new Envelope();

        public KeyWaveProvider()
        {
            Amplitude = 0.5f;
            envelope.AttackTime = 1;
            envelope.DecayTime = 0.1;
        }

        public double Amplitude { get; set; }

        public double Key { get; set; }

        public void Play(double key) 
        {
            envelopeOn = sample / WaveFormat.SampleRate;
            envelopeOff = 0;
            Key = key;
        }

        public void Pause() 
        {
            if (envelopeOff == 0)
            {
                envelopeOff = sample / WaveFormat.SampleRate;
            }
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;

            var freq1 = Scale(Key);
            var freq2 = Scale(Key + 12);
            var freq3 = Scale(Key + 24);

            for (int n = 0; n < sampleCount; n++)
            {
                var amplitude = envelope.Amplitude(sample / sampleRate, envelopeOn, envelopeOff) * Amplitude;

                buffer[n + offset] = (float)amplitude * (float)(
                   + 0.7 * Oscillator.GetValue(sample, freq1, sampleRate, OscillatorType.SINE, 5, 0.001)
                   + 0.2 * Oscillator.GetValue(sample, freq2, sampleRate, OscillatorType.TRIANGLE, 3, 0.001)
                   + 0.1 * Oscillator.GetValue(sample, freq3, sampleRate, OscillatorType.SAW_ANA));

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
