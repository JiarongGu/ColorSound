using ColorSound.Core.Components;

namespace ColorSound.Core.Synthesizers
{
    public class Harmonica : SynthesizerBase
    {
        public Harmonica() : base(new Envelope(), 3) { }

        public override double Next(double[] frequencies, double time)
        {
            return 0.7 * Oscillator.GetValue(time, frequencies[0], OscillatorType.SINE, 5, 0.001)
                   + 0.2 * Oscillator.GetValue(time, frequencies[1], OscillatorType.TRIANGLE, 3, 0.001)
                   + 0.1 * Oscillator.GetValue(time, frequencies[2], OscillatorType.SAW_ANA)
                   + 0.0001 * Oscillator.GetValue(time, 0, OscillatorType.NOISE);
        }
    }
}
