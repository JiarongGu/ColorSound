using System;

namespace ColorSound.Core.Components
{
    public enum OscillatorType
    {
        SINE,
        SQUARE,
        TRIANGLE,
        SAW_ANA,
        SAW_DIG,
        NOISE
    }

    public class Oscillator
    {
        public static double GetValue(
            double time, double frequency, OscillatorType type,
            double dHertz = 0, double dAmplitude = 0, double custom = 50
        )
        {
            var freq = ToVelocity(frequency) * time + dAmplitude * dHertz * Math.Sin(ToVelocity(frequency) * time);

            switch (type)
            {
                case OscillatorType.SINE:
                    return Math.Sin(freq);

                case OscillatorType.SQUARE: // Square wave between -1 and +1
                    return Math.Sin(freq) > 0 ? 1.0 : -1.0;

                case OscillatorType.TRIANGLE: // Triangle wave between -1 and +1
                    return Math.Asin(Math.Sin(freq)) * (2.0 / Math.PI);

                case OscillatorType.SAW_ANA: // Saw wave (analogue / warm / slow)
                    {
                        var dOutput = 0.0;
                        for (var n = 1.0; n < custom; n++)
                            dOutput += (Math.Sin(n * freq)) / n;
                        return (dOutput * (2.0 / Math.PI));
                    }

                case OscillatorType.SAW_DIG:
                    return (2.0 / Math.PI) * (frequency * Math.PI * (time % 1.0 / frequency) - (Math.PI / 2.0));

                case OscillatorType.NOISE:
                    {
                        var random = new Random();
                        return (2.0 * random.NextDouble() - 1.0);
                    }

                default:
                    return 0;
            }
        }

        private static double ToVelocity(double frequency)
        {
            return frequency * 2.0 * Math.PI;
        }
    }
}
