using System;
using System.IO;
using GameEngine;
using GameEngine.Game;
using Gdk;
using Gtk;
using Action = System.Action;
using Key = Gdk.Key;

namespace DREngine.Editor.SubWindows
{
    public abstract class SavableWindow : SubWindow
    {
        private DREditor _editor;

        public string CurrentPath { get; private set; }

        public bool Dirty { get; private set; }

        public string RootTitle { get; private set; }


        private Box _container;
        private HBox _topMenu;

        private bool _includeTopBar;

        
        public SavableWindow(DREditor editor, ProjectPath resPath, bool includeTopBar = true) : base(editor, $"{resPath?.RelativePath}")
        {
            RootTitle = Title;
            _editor = editor;
            _includeTopBar = includeTopBar;
        }

        private void OnKeyPressEvent(object o, KeyPressEventArgs args)
        {
            bool control = (args.Event.State & ModifierType.ControlMask) != 0;
            OnKey(args.Event.Key, control);
        }

        private void OnDeleteEvent(object o, DeleteEventArgs args)
        {
            if (Dirty)
            {
                // Intercept close
                args.RetVal = true;
                string message = "Unsaved changes, close anyway?";

                MessageDialog dialogue = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Question,
                    ButtonsType.OkCancel, false, message);
                bool ok = (ResponseType) dialogue.Run() == ResponseType.Ok;

                dialogue.Dispose();
                if (ok)
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

        public void Open(string path)
        {
            if (File.Exists(path))
            {
                CurrentPath = path;
                OnOpen(path, _container);
                Dirty = false;
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

            VBox mainBox = new VBox();

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

            this.DeleteEvent += OnDeleteEvent;

            this.KeyPressEvent += OnKeyPressEvent;

        }

        protected virtual void OnKey(Key key, bool control)
        {
            if (control)
            {
                if (key == Key.S || key == Key.s)
                {
                    Save();
                    return;
                }
            }

            if (key == Key.Escape)
            {
                Close();
            }
        }
        
        #region Abstract Functions
        protected abstract void OnInitialize(Box container);
        protected abstract void OnOpen(string path, Box container);
        protected abstract void OnSave(string path);
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
            Button result = new Button();
            result.TooltipText = name;

            if (onPress != null)
            {
                result.Pressed += (sender, args) => { onPress.Invoke(); };
            }

            if (icon == null)
            {
                result.Label = name;
            } else {
                result.Image = new Image(icon);
            }
            _topMenu.PackStart(result, false, false, 0);
            result.Show();
            
            return result;
        }
        
        #endregion
    }
}