using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DREngine.Game.Audio
{
    public class AudioSource
    {
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

            if (clip.UsesSample)
            {
                _sampleProvider = ConvertToCorrectChannelCount(_mixer, clip.GetNewSampleProvider());
            }
            else
            {
                _sampleProvider = new WaveToSampleProvider(ConvertToCorrectChannelCount(_mixer, clip.GetNewWaveProvider()));
            }
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

        public static ISampleProvider ConvertToCorrectChannelCount(AudioMixer mixer, ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.NAudioMixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.NAudioMixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }
        public static IWaveProvider ConvertToCorrectChannelCount(AudioMixer mixer, IWaveProvider input)
        {
            if (input.WaveFormat.Channels == mixer.NAudioMixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.NAudioMixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoProvider16(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }
    }
}
