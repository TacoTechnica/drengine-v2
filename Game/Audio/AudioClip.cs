﻿using System;
using NAudio.Wave;

namespace DREngine.Game.Audio
{
    /// <summary>
    /// Represents an audio clip that can store and stream audio in various ways.
    /// </summary>
    public class AudioClip
    {
        private AudioStorageBase _clip;

        private AudioClipType _type;

        // TODO: Add default volume and pitch scale
        public AudioClip(AudioOutput targetOutput, Path audioFile, AudioClipType type = AudioClipType.Cached)
        {
            _type = type;
            switch (type)
            {
                case AudioClipType.Cached:
                    _clip = new AudioStorageCached(targetOutput, audioFile);
                    break;
                case AudioClipType.Streamed:
                    _clip = new AudioStorageStreamed(targetOutput, audioFile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public ISampleProvider GetNewSampleProvider()
        {
            return _clip.GetNewSampleProvider();
        }
    }

    public enum AudioClipType
    {
        Cached,
        Streamed
    }
}
