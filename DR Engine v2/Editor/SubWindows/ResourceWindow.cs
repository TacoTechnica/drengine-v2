using System;
using System.IO;
using GameEngine;
using GameEngine.Game;
using Gdk;
using Gtk;
using Action = System.Action;

namespace DREngine.Editor.SubWindows
{
    public abstract class ResourceWindow<T> : SubWindow where T : IGameResource
    {
        private DREditor _editor;

        public string CurrentPath { get; private set; }
        public T CurrentResource { get; private set; }

        public bool Dirty { get; private set; }

        private Box _container;

        private HBox _topMenu;

        public new string Title { get; private set; }

        public ResourceWindow(DREditor editor, string title) : base(editor, title)
        {
            Title = title;
            _editor = editor;
            this.DeleteEvent += OnDeleteEvent;

        }

        private void OnDeleteEvent(object o, DeleteEventArgs args)
        {
            //args.Event.SendEvent = false;
            OnClose();
        }

        public void Open(string path)
        {
            try
            {
                CurrentResource = _editor.ResourceLoader.GetResource<T>(path);
            }
            catch (Exception e)
            {
                CurrentResource = default(T);
                bool fileExists = File.Exists(path); 
                if (fileExists)
                {
                    Debug.LogSilent($"Caught Load Error: {e.Message}");
                }

                OnLoadError(fileExists, e);
                return;
            }

            OnOpen(CurrentResource, _container);
            Dirty = false;
        }

        public void Save()
        {
            if (CurrentPath != null)
            {
                CurrentResource?.Save(CurrentPath);
                Dirty = false;
                base.Title = Title;
            }
        }

        protected override void OnInitialize()
        {
            // Init layout

            VBox mainBox = new VBox();
            
            _topMenu = new HBox();
            _topMenu.HeightRequest = 16;
            
            mainBox.PackStart(_topMenu, false, true, 4);
            
            AddMenuBarItem("Save", _editor.Icons.Save, () => {Debug.Log("TODO: SAVE SAVE SAVE RN!");});

            _container = new VBox();

            OnInitialize(_container);

            mainBox.PackStart(_container, true, true, 4);

            Add(mainBox);
        }

        #region Abstract Functions
        protected abstract void OnInitialize(Box container);
        protected abstract void OnOpen(T resource, Box container);
        protected abstract void OnLoadError(bool fileExists, Exception exception);
        protected abstract void OnClose();
        #endregion

        #region Very Useful Protected Functions for use in children
        
        protected void MarkDirty()
        {
            Dirty = true;
            base.Title = Title + " * ";
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