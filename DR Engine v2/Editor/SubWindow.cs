using System;
using GameEngine;
using Gdk;
using WindowType = Gtk.WindowType;

namespace DREngine.Editor
{
    public abstract class SubWindow : Gtk.Window
    {
        
        public bool IsOpen { get; private set; }
        public SubWindow(DREditor editor, string title="") : base(WindowType.Toplevel)
        {
            //Parent = DREditor.Instance.Window;
            if (title != "") Title = title;
            this.Decorated = true;
            this.AttachedTo = editor.Window;
            this.TypeHint = WindowTypeHint.Dialog;
            this.SkipTaskbarHint = true;

            IsOpen = true;
        }

        public void Initialize()
        {
            OnInitialize();
            ShowAll();
        }
        protected abstract void OnInitialize();

        protected override void OnDestroyed()
        {
            IsOpen = false;
            base.OnDestroyed();
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
