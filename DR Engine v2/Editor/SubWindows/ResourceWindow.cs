using System;
using System.IO;
using GameEngine;
using GameEngine.Game.Resources;
using Gtk;
using Path = GameEngine.Game.Path;

namespace DREngine.Editor.SubWindows
{
    public abstract class ResourceWindow<T> : SavableWindow where T : IGameResource
    {
        private readonly DREditor _editor;

        protected ResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _editor = editor;
        }

        public T CurrentResource { get; private set; }

        protected override void OnOpen(Path path, Box container)
        {
            try
            {
                CurrentResource = _editor.ResourceLoader.GetResource<T>(path);
            }
            catch (Exception e)
            {
                CurrentResource = default;
                var fileExists = File.Exists(path);
                if (fileExists) Debug.LogSilent($"Caught Load Error: {e.Message}");

                OnLoadError(fileExists, e);
                return;
            }

            OnOpen(CurrentResource, container);
        }

        protected override void OnSave(Path path)
        {
            CurrentResource?.Save(path);
        }


        #region Abstract Functions

        protected abstract void OnOpen(T resource, Box container);

        #endregion
    }
}