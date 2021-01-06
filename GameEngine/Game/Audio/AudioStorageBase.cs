using System.IO;
using ManagedBass;

namespace GameEngine.Game.Audio
{
    public abstract class AudioStorageBase
    {
        private readonly Path _fpath;
        private GamePlus _game;

        protected AudioOutput _output;

        public AudioStorageBase(AudioOutput targetOutput, Path audioFile)
        {
            if (!File.Exists(audioFile)) throw new FileNotFoundException($"Audio File does Not Exist: {audioFile}");
            _fpath = audioFile;
            _output = targetOutput;
        }

        public void Load()
        {
            OnLoad(_fpath);
            //_game.WhenSafeToLoad.RemoveListener(LoadClip);
        }

        public void Unload()
        {
            OnUnload();
        }

        protected abstract void OnLoad(Path path);
        protected abstract void OnUnload();

        public abstract int GetStream();
        public abstract int GetSample();

        //public abstract ISampleProvider GetNewSampleProvider();
        //public abstract IWaveProvider GetNewWaveProvider();
    }

    public class AudioStorageCached : AudioStorageBase
    {
        private Path _path;

        private int _sample = -1;

        public AudioStorageCached(AudioOutput targetOutput, Path audioFile) : base(targetOutput, audioFile)
        {
            _sample = -1;
        }

        protected override void OnLoad(Path path)
        {
            _path = path;
            if (_sample != -1) Unload();

            _sample = Bass.SampleLoad(_path, 0, 0, 1000, BassFlags.MixerMatrix | BassFlags.Decode);
            if (_sample == 0) Debug.LogError($"Failed to load sample: {Bass.LastError}");
        }

        protected override void OnUnload()
        {
            if (_sample != -1)
            {
                Bass.SampleFree(_sample);
                _sample = -1;
            }
        }

        public override int GetStream()
        {
            return -1;
        }

        public override int GetSample()
        {
            return _sample;
        }
    }

    public class AudioStorageStreamed : AudioStorageBase
    {
        private Path _path;

        public AudioStorageStreamed(AudioOutput targetOutput, Path audioFile) : base(targetOutput, audioFile)
        {
        }

        protected override void OnLoad(Path path)
        {
            _path = path;
        }

        protected override void OnUnload()
        {
            // Do nothing, we're streamed.
        }

        public override int GetStream()
        {
            Debug.Log($"Stream from {_path}");
            var result = Bass.CreateStream(_path);
            if (result == 0) Debug.LogError($"Stream error: {Bass.LastError}");
            return result;
            /*,
                AudioMixer.IgnoreBassMixLibrary? BassFlags.MixerChanMatrix : BassFlags.MixerChanMatrix | BassFlags.Decode
                );*/
        }

        public override int GetSample()
        {
            return -1;
        }
    }
}