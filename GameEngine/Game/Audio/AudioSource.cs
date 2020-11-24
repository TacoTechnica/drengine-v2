using System;
using ManagedBass;
using ManagedBass.Mix;

namespace GameEngine.Game.Audio
{
    public class AudioSource
    {
        private const int EMPTY_CHANNEL = -1;

        private AudioMixer _mixer;
        private int _channel;
        private int _toFree;

        private AudioClip _currentClip;

        public AudioSource(AudioMixer mixer)
        {
            _mixer = mixer;
            _channel = EMPTY_CHANNEL;
            _currentClip = null;
        }

        public void Play(AudioClip clip)
        {

            // If we're playing the same clip, use the old channel.
            if (_currentClip != clip)
            {
                if (_currentClip != null)
                {
                    // Stop the current clip!
                    StopInternal();
                }
                _currentClip = clip;
                _channel = clip.GetNewChannelSource(out _toFree);
                Debug.Log($"NEW: {_toFree}");
            }

            _mixer.PlayChannel(_channel);
        }

        public void Stop()
        {
            if (_channel != EMPTY_CHANNEL)
            {
                StopInternal();

                _channel = EMPTY_CHANNEL;
                _currentClip = null;
            }
        }

        private void StopInternal()
        {
            _mixer.StopChannel(_channel);

            if (_currentClip.UsesSample)
            {
                Bass.SampleFree(_toFree);
                Debug.Log($"DELETE: {_toFree}");
            }
            else
            {
                Bass.StreamFree(_channel);
            }
        }
    }
}
