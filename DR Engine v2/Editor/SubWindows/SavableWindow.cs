using System;
using System.IO;
using DREngine.Editor.Components;
using DREngine.ResourceLoading;
using GameEngine;
using Gdk;
using Gtk;
using Action = System.Action;
using Key = Gdk.Key;
using Path = GameEngine.Game.Path;

namespace DREngine.Editor.SubWindows
{
    public abstract class SavableWindow : SubWindow
    {
        private Box _container;
        private readonly DREditor _editor;

        private readonly bool _includeTopBar;
        private HBox _topMenu;


        public SavableWindow(DREditor editor, ProjectPath resPath, bool includeTopBar = true) : base(editor,
            $"{resPath?.RelativePath}")
        {
            RootTitle = Title;
            _editor = editor;
            _includeTopBar = includeTopBar;
        }

        public Path CurrentPath { get; private set; }

        public bool Dirty { get; private set; }

        public string RootTitle { get; }

        private void OnKeyPressEvent(object o, KeyPressEventArgs args)
        {
            var control = (args.Event.State & ModifierType.ControlMask) != 0;
            OnKey(args.Event.Key, control);
        }

        private void OnDeleteEvent(object o, DeleteEventArgs args)
        {
            if (Dirty)
            {
                // Intercept close
                args.RetVal = true;
                var message = "Unsaved changes, close anyway and discard changes?";

                if (AreYouSureDialog.Run(this, "Unsaved Changes", message, "Close and Discard Changes", "Cancel"))
                {
                    // Don't Intercept close
                    args.RetVal = false;
                    OnClose();
                }
            }
            else
            {
                // Don't Intercept close
                args.RetVal = false;
                OnClose();
            }
        }

        public void Open(Path path)
        {
            if (File.Exists(path))
            {
                CurrentPath = path;
                OnOpen(path, _container);
                Dirty = false;
            }
            else
            {
                Debug.LogWarning($"[SavableWindow] File does not exist on path {path}");
            }
        }

        public void Save()
        {
            if (CurrentPath != null && Dirty)
            {
                OnSave(CurrentPath);
                Dirty = false;
                Title = RootTitle;
            }
        }

        protected override void OnInitialize()
        {
            // Init layout

            var mainBox = new VBox();

            if (_includeTopBar)
            {
                _topMenu = new HBox();
                _topMenu.HeightRequest = 16;

                mainBox.PackStart(_topMenu, false, true, 4);

                AddMenuBarItem("Save", _editor.Icons.Save, () => { Save(); });
            }

            _container = new VBox();

            OnInitialize(_container);

            mainBox.PackStart(_container, true, true, 4);

            Add(mainBox);

            DeleteEvent += OnDeleteEvent;

            KeyPressEvent += OnKeyPressEvent;
        }

        protected virtual void OnKey(Key key, bool control)
        {
            if (control)
                if (key == Key.S || key == Key.s)
                {
                    Save();
                    return;
                }

            if (key == Key.Escape) Close();
        }

        #region Abstract Functions

        protected abstract void OnInitialize(Box container);
        protected abstract void OnOpen(Path path, Box container);
        protected abstract void OnSave(Path path);
        protected abstract void OnLoadError(bool fileExists, Exception exception);
        protected abstract void OnClose();

        #endregion

        #region Very Useful Protected Functions for use in children

        protected void MarkDirty()
        {
            Dirty = true;
            Title = "*" + RootTitle + " * ";
        }

        protected Button AddMenuBarItem(string name, Pixbuf icon = null, Action onPress = null)
        {
            var result = new Button();
            result.TooltipText = name;

            if (onPress != null) result.Pressed += (sender, args) => { onPress.Invoke(); };

            if (icon == null)
                result.Label = name;
            else
                result.Image = new Image(icon);
            _topMenu.PackStart(result, false, false, 0);
            result.Show();

            return result;
        }

        #endregion
    }
}