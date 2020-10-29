using System;
using GLib;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DREngine.Game.Audio
{
    /// <summary>
    ///  A mixer that can play audio and can have effects applied to it (for now just volume control)
    /// </summary>
    public class AudioMixer
    {

        private readonly MixingSampleProvider mixer;

        public MixingSampleProvider NAudioMixer => mixer;

        public AudioMixer(AudioOutput output)
        {
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(output.SampleRate, output.ChannelCount));

            mixer.ReadFully = true;

            output.AddMixer(this);
        }

        public void PlaySample(ISampleProvider sample)
        {
            mixer.AddMixerInput(sample);
        }

        public void StopSample(ISampleProvider sample)
        {
            mixer.RemoveMixerInput(sample);
        }
    }
}
