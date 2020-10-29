using System;
using System.Linq;
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

        private readonly MixingProvider mixer;

        public float Volume
        {
            get => mixer.Volume;
            set => mixer.Volume = value;
        }

        public MixingProvider NAudioMixer => mixer;

        public AudioMixer(AudioOutput output)
        {
            mixer = new MixingProvider(
                WaveFormat.CreateIeeeFloatWaveFormat(output.SampleRate, output.ChannelCount));

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

        public void StopAll()
        {
            mixer.RemoveAllMixerInputs();
        }

        public class MixingProvider : ISampleProvider
        {
            private MixingSampleProvider _mixer;

            public float Volume
            {
                get => _volume;
                set
                {
                    _volume = value;
                    _realScale = _volume * _volume;
                }
            }

            private float _volume;
            private float _realScale;

            public WaveFormat WaveFormat => _mixer.WaveFormat;

            public MixingProvider(WaveFormat format)
            {
                _mixer = new MixingSampleProvider(format);
                _mixer.ReadFully = true;
                Volume = 1;
            }

            public int Read(float[] buffer, int offset, int count)
            {
                int result = _mixer.Read(buffer, offset, count);
                for (int i = 0; i < buffer.Length; ++i)
                {
                    buffer[i] *= _realScale;
                }

                return result;
            }

            public void AddMixerInput(ISampleProvider p)
            {
                _mixer.AddMixerInput(p);
            }

            public void RemoveMixerInput(ISampleProvider p)
            {
                _mixer.RemoveMixerInput(p);
            }

            public void RemoveAllMixerInputs()
            {
                _mixer.RemoveAllMixerInputs();
            }
        }

    }
}
