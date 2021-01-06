using System;
using Gdk;
using Gtk;
using Math = GameEngine.Math;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace DREngine.Editor.SubWindows
{
    public abstract class SubWindow : Window
    {
        private DREditor _editor;

        public SubWindow(DREditor editor, string title = "") : base(WindowType.Toplevel)
        {
            //Parent = DREditor.Instance.Window;
            if (title != "") Title = title;
            Decorated = true;
            AttachedTo = editor.Window;
            TypeHint = WindowTypeHint.Dialog;
            SkipTaskbarHint = true;

            _editor = editor;

            IsOpen = false;
        }

        public bool IsOpen { get; private set; }

        public void Initialize()
        {
            OnInitialize();
            ShowAll();

            DeleteEvent += OnDeleteEvent;

            RequestMinSize(128, 128);
            //RequestMaxSize(_editor.Window.Display.PrimaryMonitor.Workarea.Width, _editor.Window.Display.PrimaryMonitor.Workarea.Height);
        }

        private void OnDeleteEvent(object o, DeleteEventArgs args)
        {
            IsOpen = false;
        }

        protected override void Dispose(bool disposing)
        {
            IsOpen = false;
            base.Dispose(disposing);
        }

        protected override void OnDestroyed()
        {
            IsOpen = false;
            base.OnDestroyed();
        }

        protected override void OnShown()
        {
            IsOpen = true;
            base.OnShown();
        }

        protected override bool OnDestroyEvent(Event evnt)
        {
            IsOpen = false;
            return base.OnDestroyEvent(evnt);
        }

        protected virtual void RequestMinSize(int minWidth, int minHeight)
        {
            int width, height;
            GetSize(out width, out height);
            SetSizeRequest(Math.Max(width, minWidth), Math.Max(height, minHeight));
        }

        protected virtual void RequestMaxSize(int maxWidth, int maxHeight)
        {
            int width, height;
            GetSize(out width, out height);
            SetSizeRequest(Math.Min(width, maxWidth), Math.Min(height, maxHeight));
        }


        protected abstract void OnInitialize();


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