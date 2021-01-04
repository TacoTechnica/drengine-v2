using System;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine;
using GameEngine.Game;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources
{
    public class SpriteResourceWindow : ResourceWindow<Sprite>
    {
        private DREditor _editor;

        private Image _image;
        private Label _label;

        private FieldBox _fields;

        public SpriteResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _editor = editor;
        }

        protected override void OnInitialize(Box container)
        {
            _image = new Image();
            _label = new Label("Nothing loaded");
            _fields = new ExtraDataFieldBox(_editor, typeof(Sprite), true);
            container.PackStart(_image, true, true, 4);
            container.PackEnd(_label, false, false, 4);
            container.PackEnd(_fields, false, true, 4);
            _image.Show();
            _label.Show();
            _fields.Show();

            _fields.Modified += MarkDirty;
        }

        protected override void OnOpen(Sprite resource, Box container)
        {
            // Load sprite
            _image.File = resource.Path;
            _label.Text = $"{resource.Width} x {resource.Height} Sprite";
            _fields.LoadTarget(resource);
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            // display error
            _image.Clear();
            _label.Text = $"ERROR: {exception.ToString()}";
        }

        protected override void OnClose()
        {
            if (Dirty)
            {
                CurrentResource?.Load(_editor.ResourceLoaderData);
            }
        }
    }
}