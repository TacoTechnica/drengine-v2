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

        public float Volume;
        /*
        {
            get => mixer.Volume;
            set => mixer.Volume = value;
        }
        */

        public AudioMixer(AudioOutput output)
        {
            _stream = BassMix.CreateMixerStream(output.SampleRate, output.ChannelCount, BassFlags.Default);
            output.AddMixer(this);
        }

        internal void PlayChannel(int channel)
        {
            BassMix.MixerAddChannel(_stream, channel, BassFlags.Default);
            Bass.ChannelPlay(channel, true);
        }

        internal void StopChannel(int channel)
        {
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
