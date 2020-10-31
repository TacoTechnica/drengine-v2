using System;
using ManagedBass;
using ManagedBass.Mix;

namespace DREngine.Game.Audio
{
    public class AudioSource
    {
        private const int EMPTY_CHANNEL = -1;

        private AudioMixer _mixer;
        private int _channel;

        private AudioClip _currentClip;

        public AudioSource(AudioMixer mixer)
        {
            _mixer = mixer;
            _channel = EMPTY_CHANNEL;
            _currentClip = null;
        }

        public void Play(AudioClip clip)
        {
            Stop(clip);

            // If we're playing the same clip, use the old channel.
            if (_currentClip != clip)
            {
                _currentClip = clip;
                _channel = clip.GetNewSource();
            }

            _mixer.PlayChannel(_channel);
        }

        public void Stop(AudioClip toOverride = null)
        {
            if (_channel != EMPTY_CHANNEL)
            {
                _mixer.StopChannel(_channel);
                _channel = EMPTY_CHANNEL;

                // If we're overriding with the same clip, don't make a new source.
                if (_currentClip != toOverride)
                {
                    if (_currentClip.UsesSample)
                    {
                        Bass.SampleFree(_channel);
                    }
                    else
                    {
                        Bass.StreamFree(_channel);
                    }
                }
            }

            _currentClip = null;
        }
    }
}
