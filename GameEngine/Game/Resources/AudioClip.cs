using System;
using ManagedBass;

namespace GameEngine.Game.Audio
{
    /// <summary>
    /// Represents an audio clip that can store and stream audio in various ways.
    /// </summary>
    public class AudioClip : IGameResource
    {
        public AudioClipType Type { get; set; }
        public bool UsesSample { get; private set; } = true;
        public string Path { get; set; }

        private AudioStorageBase _clip = null;

        // TODO: Add default volume and pitch scale
        public AudioClip(GamePlus game, Path audioFile, AudioClipType type = AudioClipType.Cached)
        {
            Path = audioFile;
            Type = type;
            Load(game);
        }

        // Deserialize Constructor
        public AudioClip() {}


        public int GetNewChannelSource(out int toFree)
        {
            if (UsesSample)
            {
                toFree = -1; // We do not free our sampled audio until it is freed as a resource.
                return Bass.SampleGetChannel(toFree, true);
            }

            toFree = _clip.GetStream();
            return toFree;
        }


        public void Load(GamePlus game)
        {
            AudioOutput targetOutput = game.AudioOutput;
            switch (Type)
            {
                case AudioClipType.Cached:
                    _clip = new AudioStorageCached(targetOutput, Path);
                    UsesSample = true;
                    break;
                case AudioClipType.Streamed:
                    _clip = new AudioStorageStreamed(targetOutput, Path);
                    UsesSample = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
            }

            Assert.IsNotNull(_clip);
            _clip.Load();
        }

        public void Unload(GamePlus game)
        {
            Assert.IsNotNull(_clip);
            _clip.Unload();
        }
    }

    public enum AudioClipType
    {
        Cached,
        Streamed
    }
}
