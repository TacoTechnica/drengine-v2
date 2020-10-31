
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

        }

        public void StopAll()
        {
            _mixer.StopAll();
        }
    }
}
