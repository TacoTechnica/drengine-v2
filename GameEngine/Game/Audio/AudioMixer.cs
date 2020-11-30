using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ManagedBass;
using ManagedBass.Mix;

namespace GameEngine.Game.Audio
{
    /// <summary>
    ///  A mixer that can play audio and can have effects applied to it (for now just volume control)
    /// </summary>
    public class AudioMixer
    {
        /**
         *
         * I cannot get libbass.so and libbassmix.so to work together. I have this problem:
         * http://www.un4seen.com/forum/?topic=18656.0
         * and NO solution, because it feels like nobody uses C# on linux.
         *
         * For now my only solution is to keep BassMix encapsulated and have two alternative implementations.
         *
         */
        private const bool IS_THERE_MIXER_BULLSHIT = true;

        internal static bool IgnoreBassMixLibrary => true;//IS_THERE_MIXER_BULLSHIT && RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        private AudioMixerLinux _linuxFix = null;

        private int _stream;

        private float _volume;
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;

                if (IgnoreBassMixLibrary)
                {
                    _linuxFix.SetVolume(_volume);
                }
                else
                {
                    // Normal way
                    if (!Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, _volume))
                    {
                        Debug.LogError($"ERROR: {Bass.LastError.ToString()}");
                    }
                }
            }
        }

        public AudioMixer(AudioOutput output)
        {
            Debug.Log("b4");
            if (IgnoreBassMixLibrary)
            {
                _linuxFix = new AudioMixerLinux(output.SampleRate, output.ChannelCount);
                _stream = -1;
            }
            else
            {
                _stream = BassMix.CreateMixerStream(output.SampleRate, output.ChannelCount, BassFlags.MixerMatrix);
            }

            Debug.Log("After");
            Volume = 1f;
            output.AddMixer(this);
        }

        internal void PlayChannel(int channel)
        {
            Bass.ChannelPlay(channel, true);
            if (IgnoreBassMixLibrary)
            {
                _linuxFix.AddChannel(channel);
            } else {
                if (!BassMix.MixerAddChannel(_stream, channel, BassFlags.MixerMatrix))
                {
                    if (Bass.LastError != Errors.Decode)
                    {
                        Debug.LogError($"ERROR: {Bass.LastError.ToString()}");
                    }
                }

                Bass.ChannelPlay(_stream, false);
            }
        }

        internal void StopChannel(int channel)
        {
            Debug.Log("Channel stopped");
            Bass.ChannelStop(channel);
            if (IgnoreBassMixLibrary)
            {
                _linuxFix.RemoveChannel(channel);
            }
            else
            {
                BassMix.MixerRemoveChannel(channel);
            }
        }

        public void StopAll()
        {
            List<int> channels = new List<int>(IgnoreBassMixLibrary? _linuxFix.GetChannels() : BassMix.MixerGetChannels(_stream));
            foreach (int channel in channels)
            {
                StopChannel(channel);
            }
        }


        // This is stupid
        class AudioMixerLinux
        {
            private List<int> _channels;
            private float _volume;
            public AudioMixerLinux(int outputSampleRate, int outputChannelCount)
            {
                _channels = new List<int>();
                _volume = 1;
            }

            private float VolumeScale(float volume)
            {
                return volume * volume;
            }

            public void AddChannel(int channel)
            {
                Bass.ChannelSetAttribute(channel, ChannelAttribute.Volume, _volume);
                _channels.Add(channel);
            }

            public void RemoveChannel(int channel)
            {
                _channels.Remove(channel);
            }

            public IEnumerable<int> GetChannels()
            {
                return _channels;
            }

            public void SetVolume(float volume)
            {
                _volume = VolumeScale(volume);
                foreach (int channel in GetChannels())
                {
                    Bass.ChannelSetAttribute(channel, ChannelAttribute.Volume, _volume);
                }
            }
        }

    }
}
