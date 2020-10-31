using System;
using System.Collections.Generic;
using System.Linq;
using GLib;
using ManagedBass;
using ManagedBass.Mix;

namespace DREngine.Game.Audio
{
    /// <summary>
    ///  A mixer that can play audio and can have effects applied to it (for now just volume control)
    /// </summary>
    public class AudioMixer
    {
        private int _stream;

        private float _volume;
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                if (!Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, _volume))
                {
                    Debug.LogError($"ERROR: {Bass.LastError.ToString()}");
                }
            }
        }

        public AudioMixer(AudioOutput output)
        {
            _stream = BassMix.CreateMixerStream(output.SampleRate, output.ChannelCount, BassFlags.MixerChanMatrix);
            Volume = 1f;
            output.AddMixer(this);
        }

        internal void PlayChannel(int channel)
        {
            Bass.ChannelPlay(channel, true);
            if (!BassMix.MixerAddChannel(_stream, channel, BassFlags.MixerChanMatrix))
            {
                Debug.LogError($"ERROR: {Bass.LastError.ToString()}");
            }

            Bass.ChannelPlay(_stream, false);
        }

        internal void StopChannel(int channel)
        {
            Debug.Log("Channel stopped");
            Bass.ChannelStop(channel);
            BassMix.MixerRemoveChannel(channel);
        }

        public void StopAll()
        {
            List<int> channels = new List<int>(BassMix.MixerGetChannels(_stream));
            foreach (int channel in channels)
            {
                StopChannel(channel);
            }
        }


    }
}
