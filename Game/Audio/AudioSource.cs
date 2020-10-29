using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DREngine.Game.Audio
{
    public class AudioSource
    {
        public float Volume; // TODO: Volume control (0 - 1)
        public float Pitch; // TODO: Is this necessary?

        private ISampleProvider _sampleProvider;

        private AudioMixer _mixer;

        public AudioSource(AudioMixer mixer)
        {
            _mixer = mixer;
            _sampleProvider = null;
        }

        public void Play(AudioClip clip)
        {
            if (_sampleProvider != null)
            {
                _mixer.StopSample(_sampleProvider);
            }
            _sampleProvider = ConvertToCorrectChannelCount(clip.GetNewSampleProvider());
            _mixer.PlaySample(_sampleProvider);
        }

        public void Stop()
        {
            if (_sampleProvider != null)
            {
                _mixer.StopSample(_sampleProvider);
                _sampleProvider = null;
            }
        }

        private ISampleProvider ConvertToCorrectChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == _mixer.NAudioMixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && _mixer.NAudioMixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }
    }
}
