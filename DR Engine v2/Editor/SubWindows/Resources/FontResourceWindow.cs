using System;
using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using Gdk;
using GLib;
using Gtk;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework.Graphics;
using Pango;
using Font = GameEngine.Game.Resources.Font;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace DREngine.Editor.SubWindows.Resources
{
    public class FontResourceWindow : ResourceWindow<Font>
    {

        private const string DefaultTestString = "The Quick Brown Fox Jumped over the Lazy Dog";

        private DREditor _editor;
        private FieldBox _fields;

        //private TextView _textEdit;

        private Image _imageView;
        
        public FontResourceWindow(DREditor editor, ProjectPath resPath) : base(editor, resPath)
        {
            _editor = editor;
        }

        protected override void OnInitialize(Box container)
        {
            this.RequestMinSize(300, 300);
            ScrolledWindow scroll = new ScrolledWindow();
            scroll.MaxContentHeight = 1000;
            scroll.MaxContentWidth = 1000;
            _imageView = new Image();
            
            scroll.Add(_imageView);
            //_textEdit = new TextView();
            //_textEdit.Buffer.Text = DefaultTestString;

            _fields = new ExtraDataFieldBox(_editor, typeof(Font), true);
            _fields.Modified += Modified;

            container.PackStart(scroll, true, true, 16);
            _imageView.Show();
            scroll.Show();
            //container.PackStart(_imageView, false, true, 16);
            //container.PackStart(_textEdit, false, true, 16);
            container.PackEnd(_fields, false, true, 16);
        }

        private void Modified()
        {
            MarkDirty();
            //CurrentResource.Size;
        }

        protected override void OnOpen(Font resource, Box container)
        {
            _fields.LoadTarget(resource);
            string temp = System.IO.Path.GetTempFileName();
            Texture2D tex = resource.SpriteFont.Texture;
            tex.SaveAsPng(new FileStream(temp, FileMode.Create), tex.Width, tex.Height);
            _imageView.File = temp;
        }

        protected override void OnLoadError(bool fileExists, Exception exception)
        {
            // TODO: Display Error
        }

        protected override void OnClose()
        {
            // Nothing.
        }

    }
}