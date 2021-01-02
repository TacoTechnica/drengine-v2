using System;
using Gdk;
using WindowType = Gtk.WindowType;

namespace DREngine.Editor
{
    public abstract class SubWindow : Gtk.Window
    {
        public SubWindow(DREditor editor, string title="") : base(WindowType.Toplevel)
        {
            //Parent = DREditor.Instance.Window;
            if (title != "") Title = title;
            this.Decorated = true;
            this.AttachedTo = editor.Window;
            this.TypeHint = WindowTypeHint.Dialog;
            this.SkipTaskbarHint = true;

            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();

            ShowAll();
        }

        protected abstract void Initialize();

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
