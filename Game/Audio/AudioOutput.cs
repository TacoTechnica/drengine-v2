using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DREngine.Game.Audio
{
    /// <summary>
    /// Represents our game's one and only audio output device (speakers/headphones/everything)
    /// </summary>
    public class AudioOutput : IDisposable
    {

        private readonly IWavePlayer outputDevice;
        private readonly MixingSampleProvider mainMixer;

        public int SampleRate => WaveFormat.SampleRate;
        public int ChannelCount => WaveFormat.Channels;

        public WaveFormat WaveFormat { get; private set; }

        public AudioOutput(int sampleRate = 44100, int channelCount = 2)
        {

            outputDevice = new WaveOutEvent();
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount);
            mainMixer = new MixingSampleProvider(WaveFormat);
            mainMixer.ReadFully = true;
            outputDevice.Init(mainMixer);
            // Continuously play and accept audio.
            outputDevice.Play();
        }

        public void AddMixer(AudioMixer mixer)
        {
            mainMixer.AddMixerInput(mixer.NAudioMixer);
        }

        public void Dispose()
        {
            outputDevice.Dispose();
        }
    }
}
