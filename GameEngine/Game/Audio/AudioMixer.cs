using System.Collections.Generic;
using ManagedBass;
using ManagedBass.Mix;

namespace GameEngine.Game.Audio
{
    /// <summary>
    ///  A mixer that can play audio and can have effects applied to it (for now just volume control)
    /// </summary>
    public class AudioMixer
    {

        private readonly AudioMixerLinux _linuxFix;

        private readonly int _stream;

        private float _volume;

        public AudioMixer(AudioOutput output)
        {
            if (IgnoreBassMixLibrary)
            {
                _linuxFix = new AudioMixerLinux();
                _stream = -1;
            }
            else
            {
                _stream = BassMix.CreateMixerStream(output.SampleRate, output.ChannelCount, BassFlags.MixerChanMatrix);
            }

            Volume = 1f;
            output.AddMixer(this);
        }

        internal static bool IgnoreBassMixLibrary => true;//IS_THERE_MIXER_BULLSHIT && RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

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
                    if (!Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, _volume)) Debug.LogError($"ERROR: {Bass.LastError.ToString()}");
                }
            }
        }

        internal void PlayChannel(int channel)
        {
            Bass.ChannelPlay(channel, true);
            if (IgnoreBassMixLibrary)
            {
                _linuxFix.AddChannel(channel);
            } else {
                if (!BassMix.MixerAddChannel(_stream, channel, BassFlags.MixerChanMatrix))
                    if (Bass.LastError != Errors.Decode) Debug.LogError($"ERROR: {Bass.LastError.ToString()}");

                Bass.ChannelPlay(_stream);
            }
        }

        internal void StopChannel(int channel)
        {
            Bass.ChannelStop(channel);
            if (IgnoreBassMixLibrary)
                _linuxFix.RemoveChannel(channel);
            else
                BassMix.MixerRemoveChannel(channel);
        }

        public void StopAll()
        {
            var channels = new List<int>(IgnoreBassMixLibrary? _linuxFix.GetChannels() : BassMix.MixerGetChannels(_stream));
            foreach (var channel in channels) StopChannel(channel);
        }


        // This is stupid
        private class AudioMixerLinux
        {
            private readonly List<int> _channels;
            private float _volume;

            public AudioMixerLinux()
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
                foreach (var channel in GetChannels()) Bass.ChannelSetAttribute(channel, ChannelAttribute.Volume, _volume);
            }
        }
    }
}
