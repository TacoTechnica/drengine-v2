using System;
using System.Collections.Generic;
using System.Linq;
using ManagedBass;

namespace DREngine.Game.Audio
{
    public abstract class AudioStorageBase
    {
        private GamePlus _game;
        private Path _fpath;

        protected AudioOutput _output;

        public AudioStorageBase(AudioOutput targetOutput, Path audioFile)
        {
            _fpath = audioFile;
            _output = targetOutput;

            LoadClip();
            //_game.WhenSafeToLoad.AddListener(LoadClip);
        }

        private void LoadClip()
        {
            OnLoad(_fpath);
            //_game.WhenSafeToLoad.RemoveListener(LoadClip);
        }

        protected abstract void OnLoad(Path path);

        public abstract int GetStream();
        public abstract int GetSample();

        //public abstract ISampleProvider GetNewSampleProvider();
        //public abstract IWaveProvider GetNewWaveProvider();
    }

    public class AudioStorageCached : AudioStorageBase
    {
        private Path _path;
        public AudioStorageCached(AudioOutput targetOutput, Path audioFile) : base(targetOutput, audioFile)
        {

        }

        protected override void OnLoad(Path path)
        {
            _path = path;
        }

        public override int GetStream()
        {
            return -1;
        }

        public override int GetSample()
        {
            return Bass.SampleLoad(_path, 0, 0, 1000, BassFlags.MixerChanMatrix | BassFlags.Decode);
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

        public override int GetStream()
        {
            return Bass.CreateStream(_path, 0, 0, BassFlags.MixerChanMatrix | BassFlags.Decode);
        }

        public override int GetSample()
        {
            return -1;
        }
    }

    /*
    public class AudioStorageCached : AudioStorageBase
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public AudioStorageCached(AudioOutput targetOutput, Path audioFile) : base(targetOutput, audioFile)
        {
        }

        protected override void OnLoad(Path path)
        {
            AudioFileReader baseReader = new AudioFileReader(path);
            var audioFileReader = new MediaFoundationResampler(baseReader, _output.SampleRate).ToSampleProvider();
            WaveFormat = audioFileReader.WaveFormat;
            Debug.Log($"{WaveFormat.SampleRate}, {WaveFormat.Channels}");
            var wholeFile = new List<float>((int)(baseReader.Length / 4));
            var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
            int samplesRead;
            while((samplesRead = audioFileReader.Read(readBuffer,0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }
            AudioData = wholeFile.ToArray();
        }

        public override ISampleProvider GetNewSampleProvider()
        {
            return new SampleProvider(this);
        }

        public override IWaveProvider GetNewWaveProvider()
        {
            return null;
        }

        /// <summary>
        /// Cached Audio Sample provider
        /// </summary>
        class SampleProvider : ISampleProvider
        {
            private readonly AudioStorageCached cachedSound;
            private long position;

            public WaveFormat WaveFormat => cachedSound.WaveFormat;

            public SampleProvider(AudioStorageCached cachedSound)
            {
                this.cachedSound = cachedSound;
            }

            public int Read(float[] buffer, int offset, int count)
            {
                var availableSamples = cachedSound.AudioData.Length - position;
                var samplesToCopy = System.Math.Min(availableSamples, count);
                Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
                position += samplesToCopy;
                return (int)samplesToCopy;
            }

        }

    }

    public class AudioStorageStreamed : AudioStorageBase
    {
        public string Fpath;

        public int TargetSampleRate;

        public AudioStorageStreamed(AudioOutput targetOutput, Path audioFile) : base(targetOutput, audioFile)
        {
            Fpath = audioFile;
            TargetSampleRate = targetOutput.SampleRate;
        }

        protected override void OnLoad(Path path)
        {
            // We don't need to load anything right now...
            // TODO: Pre-cache the first second or so of audio and start with that while the file stream is loaded.
        }

        public override ISampleProvider GetNewSampleProvider()
        {
            return null; //new WaveToSampleProvider(new StreamProvider(this));
        }

        public override IWaveProvider GetNewWaveProvider()
        {
            return new StreamProvider(this);
        }

        class StreamProvider : IWaveProvider, IDisposable
        {
            private MediaFoundationResampler _reader;
            private AudioStorageStreamed _streamedAudio;
            //private float[] _readBuffer;

            private bool _disposed;

            public WaveFormat WaveFormat => _reader.WaveFormat;

            public StreamProvider(AudioStorageStreamed streamedAudio)
            {
                _streamedAudio = streamedAudio;
                _reader = new MediaFoundationResampler(new AudioFileReader(streamedAudio.Fpath), streamedAudio.TargetSampleRate);
                //_readBuffer = new float[_reader.WaveFormat.SampleRate * _reader.WaveFormat.Channels];

                _disposed = false;
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                if (_disposed) return 0;

                int samplesRead = _reader.Read(buffer, 0, buffer.Length);//_reader.Read(_readBuffer, 0, _readBuffer.Length);
                if (samplesRead <= 0)
                {
                    // We're done!
                    Dispose();
                }

                return samplesRead;
            }

            public void Dispose()
            {
                _disposed = true;
                _reader?.Dispose();
            }
        }
    }
    */
}
