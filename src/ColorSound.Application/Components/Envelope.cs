namespace ColorSound.Application.Components
{
    public class Envelope
    {
        public Envelope()
        {
            AttackTime = 0.1;
            DecayTime = 0.1;
            SustainAmplitude = 1.0;
            ReleaseTime = 0.2;
            StartAmplitude = 1.0;
        }

        public double AttackTime { get; set; }

        public double DecayTime { get; set; }

        public double SustainAmplitude { get; set; }

        public double ReleaseTime { get; set; }

        public double StartAmplitude { get; set; }

        public double Amplitude(double time, double timeOn, double timeOff)
        {
            var amplitude = 0.0;
            var releaseAmplitude = 0.0;

            if (timeOn > timeOff) // Note is on
            {
                var lifeTime = time - timeOn;

                if (lifeTime <= AttackTime)
                    amplitude = (lifeTime / AttackTime) * StartAmplitude;

                if (lifeTime > AttackTime && lifeTime <= (AttackTime + DecayTime))
                    amplitude = ((lifeTime - AttackTime) / DecayTime) * (SustainAmplitude - StartAmplitude) + StartAmplitude;

                if (lifeTime > (AttackTime + DecayTime))
                    amplitude = SustainAmplitude;
            }
            else // Note is off
            {
                var lifeTime = timeOff - timeOn;

                if (lifeTime <= AttackTime)
                    releaseAmplitude = (lifeTime / AttackTime) * StartAmplitude;

                if (lifeTime > AttackTime && lifeTime <= (AttackTime + DecayTime))
                    releaseAmplitude = ((lifeTime - AttackTime) / DecayTime) * (SustainAmplitude - StartAmplitude) + StartAmplitude;

                if (lifeTime > (AttackTime + DecayTime))
                    releaseAmplitude = SustainAmplitude;

                amplitude = ((time - timeOff) / ReleaseTime) * (0 - releaseAmplitude) + releaseAmplitude;
            }

            // Amplitude should not be negative
            if (amplitude <= 0)
                amplitude = 0;

            return amplitude;
        }
    }
}
