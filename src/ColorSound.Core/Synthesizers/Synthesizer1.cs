using ColorSound.Core.Components;

namespace ColorSound.Core.Synthesizers
{
    public class Synthesizer1 : SynthesizerBase
    {
        public Synthesizer1() : base(new Envelope { AttackTime = 1, DecayTime = 0.1 }, 3) { }

        public override double Next(double[] frequencies, double time)
        {
            return + 0.5 * Oscillator.GetValue(time, frequencies[0], OscillatorType.SINE, 5, 0.001)
                   + 0.6 * Oscillator.GetValue(time, frequencies[1], OscillatorType.SQUARE, 5, 0.001)
                   + 0.2 * Oscillator.GetValue(time, frequencies[2], OscillatorType.TRIANGLE, 5, 0.001);
        }
    }
}
