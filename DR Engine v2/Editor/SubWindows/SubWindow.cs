using System;
using GameEngine;
using Gdk;
using Math = GameEngine.Math;
using WindowType = Gtk.WindowType;

namespace DREngine.Editor.SubWindows
{
    public abstract class SubWindow : Gtk.Window
    {

        private DREditor _editor;

        public bool IsOpen { get; private set; }
        public SubWindow(DREditor editor, string title="") : base(WindowType.Toplevel)
        {
            //Parent = DREditor.Instance.Window;
            if (title != "") Title = title;
            this.Decorated = true;
            this.AttachedTo = editor.Window;
            this.TypeHint = WindowTypeHint.Dialog;
            this.SkipTaskbarHint = true;

            _editor = editor;

            IsOpen = true;
        }

        public void Initialize()
        {
            OnInitialize();
            ShowAll();
            
            
            RequestMinSize(128, 128);
            //RequestMaxSize(_editor.Window.Display.PrimaryMonitor.Workarea.Width, _editor.Window.Display.PrimaryMonitor.Workarea.Height);
        }
        protected abstract void OnInitialize();

        protected override void OnDestroyed()
        {
            IsOpen = false;
            base.OnDestroyed();
        }

        protected virtual void RequestMinSize(int minWidth, int minHeight)
        {
            int width, height;
            this.GetSize(out width, out height);
            this.SetSizeRequest(Math.Max(width, minWidth), Math.Max(height, minHeight));
        }

        protected virtual void RequestMaxSize(int maxWidth, int maxHeight)
        {
            int width, height;
            this.GetSize(out width, out height);
            this.SetSizeRequest(Math.Min(width, maxWidth), Math.Min(height, maxHeight));
        }

        #region Garbage

        public SubWindow(int nothing) : base("none")
        {
            throw new InvalidOperationException();
        }
        public SubWindow(IntPtr raw) : base(raw)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
