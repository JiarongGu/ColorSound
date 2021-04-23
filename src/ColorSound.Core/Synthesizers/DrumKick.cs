using ColorSound.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColorSound.Core.Synthesizers
{
    public class DrumKick : SynthesizerBase
    {
        public DrumKick() : base(new Envelope
        {
            AttackTime = 0.01,
            DecayTime = 0.15,
            SustainAmplitude = 0,
            ReleaseTime = 0
        }, 1) { }

        public override double Next(double[] frequencies, double time)
        {
            return 0.9 * Oscillator.GetValue(time, frequencies[0], OscillatorType.TRIANGLE, 1, 1) +
                0.1 * Oscillator.GetValue(time, 0, OscillatorType.NOISE);
        }
    }
}
