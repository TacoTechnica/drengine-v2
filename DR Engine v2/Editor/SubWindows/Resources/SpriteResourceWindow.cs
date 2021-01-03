using System;
using GameEngine.Game;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources
{
    public class SpriteResourceWindow : ResourceWindow<Sprite>
    {
        private Image _image;
        private Label _label;

        public SpriteResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
        }

        protected override void OnInitialize(Box container)
        {
            _image = new Image();
            _label = new Label("Nothing loaded");
            container.PackStart(_image, true, true, 4);
            container.PackEnd(_label, false, false, 4);
            _image.Show();
            _label.Show();
        }

        protected override void OnOpen(Sprite resource, Box container)
        {
            // Load sprite
            _image.File = resource.Path;
            _label.Text = $"{resource.Width} x {resource.Height} Sprite";
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            // display error
            _image.Clear();
            _label.Text = $"ERROR: {exception.ToString()}";
        }

        protected override void OnClose()
        {
            // We're good.
        }
    }
}