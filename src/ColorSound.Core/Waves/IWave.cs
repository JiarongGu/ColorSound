namespace ColorSound.Core.Waves
{
    public interface IWave
    {
        int Read(float[] buffer, int offset, int sampleCount, int sampleRate);
    }
}
