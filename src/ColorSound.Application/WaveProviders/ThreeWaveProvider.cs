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
        private List<Node> nodes = new List<Node>();

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
            var node = new Node(nodes);
            node.TimeOn = sample / WaveFormat.SampleRate;
            node.Process = (time, on, off) => GetValue(freq1, freq2, freq3, Amplitude, time, on, off);
        }

        public void Pause()
        {
            nodes.ForEach(node =>
            {
                if (node.TimeOff == 0)
                {
                    node.TimeOff = sample / WaveFormat.SampleRate;
                }
            });
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;

            for (int n = 0; n < sampleCount; n++)
            {
                var time = sample / sampleRate;
                var amplitude = envelope.Amplitude(time, envelopeOn / sampleRate, envelopeOff / sampleRate) * Amplitude;

                buffer[n + offset] = (float)amplitude * (float)(0.5 * Oscillator.GetValue(time, Frequence1, OscillatorType.SINE, 5, 0.001)
                   + 0.3 * Oscillator.GetValue(time, Frequence2, OscillatorType.SQUARE, 5, 0.001)
                   + 0.2 * Oscillator.GetValue(time, Frequence3, OscillatorType.TRIANGLE, 5, 0.001));

                sample++;
                if (sample >= int.MaxValue) sample = 0;
            }

            return sampleCount;
        }

        private float GetValue(double freq1, double freq2, double freq3, double amplitude, double time, double on, double off)
        {
            var ampl = envelope.Amplitude(time, on, off) * amplitude;

            return (float)ampl * (float)(0.5 * Oscillator.GetValue(time, freq1, OscillatorType.SINE, 5, 0.001)
               + 0.3 * Oscillator.GetValue(time, freq2, OscillatorType.SQUARE, 5, 0.001)
               + 0.2 * Oscillator.GetValue(time, freq3, OscillatorType.TRIANGLE, 5, 0.001));
        }
    }
}
