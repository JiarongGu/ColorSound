using ColorSound.Core.Components;

namespace ColorSound.Core.Synthesizers
{
    public abstract class SynthesizerBase
    {
        protected int _frequencyCount;
        protected double _time = 0;
        protected double _on = 0;
        protected double _off = 0;

        protected Envelope _envelope;

        protected double[] _frequencies;

        public SynthesizerBase(Envelope envelope, int frequencyCount)
        {
            _envelope = envelope;
            _frequencyCount = frequencyCount;
            Amplitude = 0.5;
        }

        public double Amplitude { get; set; }

        public double AmplitudeFactor { get; set; } = 1;

        public void Play(double[] frequencies)
        {
            _frequencies = frequencies;
            _on = _time;
            _off = 0;
        }

        public void Pause()
        {
            if (_off == 0)
            {
                _off = _time;
            }
        }

        public double Next(double time)
        {
            _time = time;

            if (_frequencies == null || _frequencies.Length != _frequencyCount)
                return 0;

            var amplitude = _envelope.Amplitude(_time, _on, _off) * Amplitude * AmplitudeFactor;
            return amplitude * Next(_frequencies, _time);
        }

        public abstract double Next(double[] frequencies, double time);
    }
}
