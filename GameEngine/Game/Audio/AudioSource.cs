using System;
using System.Threading;
using System.Threading.Tasks;
using GameEngine.Game.Resources;
using ManagedBass;

namespace GameEngine.Game.Audio
{
    public class AudioSource
    {
        private const int EMPTY_CHANNEL = -1;
        private int _channel;

        private AudioClip _currentClip;

        private readonly AudioMixer _mixer;
        private int _toFree;

        public AudioSource(AudioMixer mixer)
        {
            _mixer = mixer;
            _channel = EMPTY_CHANNEL;
            _currentClip = null;
        }

        public bool Playing => Bass.ChannelIsActive(_channel) == PlaybackState.Playing;

        public void Play(AudioClip clip, Action onStop = null)
        {
            // If we're playing the same clip, use the old channel.
            if (_currentClip != clip)
            {
                if (_currentClip != null)
                    // Stop the current clip!
                    StopInternal();
                _currentClip = clip;
                _channel = clip.GetNewChannelSource(out _toFree);
                //Debug.Log($"NEW: {_toFree}");
            }

            _mixer.PlayChannel(_channel);

            if (onStop != null)
                new Task(() =>
                {
                    Debug.Log("PLAY");
                    while (Playing)
                    {
                        Thread.Yield(); // This shouldn't be necessary??
                        //Thread.Sleep(500);
                    }
                    Debug.Log("STOP!");
                    onStop.Invoke();
                }).Start();
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

            if (!_currentClip.UsesSample)
                Bass.StreamFree(_channel);
            //Debug.Log($"DELETE: {_toFree}");
        }
    }
}