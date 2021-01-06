using System;
using GameEngine.Game.Resources;
using ManagedBass;

namespace GameEngine.Game.Audio
{
    /// <summary>
    /// Represents an audio clip that can store and stream audio in various ways.
    /// </summary>
    public class AudioClip : IGameResource
    {
        [ExtraData] public AudioClipType Type;

        public bool UsesSample { get; private set; }
        public Path Path { get; set; }

        private AudioStorageBase _clip = null;

        // TODO: Add default volume and pitch scale
        public AudioClip(GamePlus game, Path audioFile, AudioClipType type = AudioClipType.Streamed)
        {
            Path = audioFile;
            Type = type;
            Load(game.ResourceLoaderData);
        }

        // Deserialize Constructor
        public AudioClip() {}


        public int GetNewChannelSource(out int toFree)
        {
            if (UsesSample)
            {
                toFree = -1; // We do not free our sampled audio until it is freed as a resource.
                int sample = _clip.GetSample();
                int result = Bass.SampleGetChannel(sample, true);
                if (result == 0)
                {
                    Debug.LogError($"Failed to create sample source: {Bass.LastError}");
                }

                return result;
            }

            toFree = _clip.GetStream();
            return toFree;
        }


        public void Load(ResourceLoaderData loader)
        {
            AudioOutput targetOutput = loader.AudioOutput;
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
            ExtraResourceHelper.LoadExtraData(this, Path);
        }

        public void Save(Path path)
        {
            Path = path;
            ExtraResourceHelper.SaveExtraData(this, path);
        }

        public void Unload()
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
