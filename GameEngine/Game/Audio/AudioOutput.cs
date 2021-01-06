using System;
using System.Reflection;
using ManagedBass;

namespace GameEngine.Game.Audio
{
    /// <summary>
    /// Represents our game's one and only audio output device (speakers/headphones/everything)
    /// </summary>
    public class AudioOutput : IDisposable
    {

        //private readonly IWavePlayer outputDevice;
        //private readonly MixingSampleProvider mainMixer;

        public int SampleRate { get; private set; }
        public int ChannelCount { get; private set; }

        //public WaveFormat WaveFormat { get; private set; }

        public bool Initialized { get; private set; }

        public AudioOutput(int sampleRate = 44100)
        {
            if (!Bass.Init(-1, sampleRate, DeviceInitFlags.Default))
            {
                if (Bass.LastError != Errors.Already)
                {
                    throw new InvalidProgramException(
                        $"ManagedBass Audio lib failed to initialize! Error: {Bass.LastError.ToString()}");
                }
                else
                {
                    Debug.LogSilent("(ManagedBass already initialized, ignoring)");
                }
            }

            Initialized = true;
            SampleRate = sampleRate;
            ChannelCount = 2;
            Debug.LogDebug($"ManagedBass Audio {Bass.Version} has been initialized!");
        }

        ~AudioOutput()
        {
            Dispose();
        }

        public void AddMixer(AudioMixer mixer)
        {
            //mainMixer.AddMixerInput(mixer.NAudioMixer);
        }

        public void Dispose()
        {
            if (Initialized) Bass.Free();
        }
    }
}
