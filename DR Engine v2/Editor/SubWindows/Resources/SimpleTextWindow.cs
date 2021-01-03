using System;
using GameEngine;
using GameEngine.Util;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources
{
    public class SimpleTextWindow : SavableWindow
    {
        private TextView _text;

        private bool _error = false;

        private bool _loadFlag = false;
        
        public SimpleTextWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
        }

        protected override void OnInitialize(Box container)
        {
            _text = new TextView();
            _text.Monospace = true;
            container.PackStart(_text, true, true, 4);
            _text.Show();
            
            _text.Buffer.Changed += BufferOnChanged;
        }

        private void BufferOnChanged(object? sender, EventArgs e)
        {
            // When we load, this event will be triggered. Ignore that first time.
            if (_loadFlag)
            {
                _loadFlag = false;
                return;
            }
            MarkDirty();
        }

        protected override void OnOpen(string path, Box container)
        {
            _error = false;
            _text.Editable = true;
            _loadFlag = true;
            _text.Buffer.Text = IOHelper.ReadTextFile(path);
        }

        protected override void OnSave(string path)
        {
            if (_error) return;
            Debug.LogSilent($"File Saved at {path}");
            IOHelper.WriteTextFile(path, _text.Buffer.Text);
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            // Print the error into the text field and make it impossible to save.
            _error = true;
            _text.Editable = false;
            _text.Buffer.Text = exception.ToString();
        }

        protected override void OnClose()
        {
            // Nothing to be done.
        }
    }
}