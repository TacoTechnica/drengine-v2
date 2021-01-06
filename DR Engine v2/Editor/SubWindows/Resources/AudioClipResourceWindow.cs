using System;
using GameEngine.Game.Audio;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources
{
    public class AudioClipResourceWindow : ResourceWindow<AudioClip>
    {
        public AudioClipResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
        }

        protected override void OnInitialize(Box container)
        {
            throw new NotImplementedException();
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            throw new NotImplementedException();
        }

        protected override void OnClose()
        {
            throw new NotImplementedException();
        }

        protected override void OnOpen(AudioClip resource, Box container)
        {
            throw new NotImplementedException();
        }
    }
}