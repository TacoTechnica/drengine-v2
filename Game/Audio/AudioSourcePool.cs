
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DREngine.Game.Audio
{
    public class AudioSourcePool
    {
        // TODO: Add tracking of sources to allow for deletion and stuff like that. Also add a "max source" option.

        private AudioMixer _mixer;

        public AudioSourcePool(AudioMixer mixer)
        {
            _mixer = mixer;
        }

        public void Play(AudioClip clip)
        {
            ISampleProvider sp;
            if (clip.UsesSample)
            {
                sp = AudioSource.ConvertToCorrectChannelCount(_mixer, clip.GetNewSampleProvider());
            }
            else
            {
                sp = new WaveToSampleProvider(AudioSource.ConvertToCorrectChannelCount(_mixer, clip.GetNewWaveProvider()));
            }
            _mixer.PlaySample(sp);
        }

        public void StopAll()
        {
            _mixer.StopAll();
        }
    }
}
