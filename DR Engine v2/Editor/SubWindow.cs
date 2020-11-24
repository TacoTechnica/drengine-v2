using System;
using Gdk;
using WindowType = Gtk.WindowType;

namespace DREngine.Editor
{
    public class SubWindow : Gtk.Window
    {
        public SubWindow(string title="") : base(WindowType.Toplevel)
        {
            //Parent = DREditor.Instance.Window;
            if (title != "") Title = title;
            this.Decorated = true;
            this.AttachedTo = DREditor.Instance.Window;
            //this.TypeHint = WindowTypeHint.Dialog;
            this.TypeHint = WindowTypeHint.Dialog;
            this.SkipTaskbarHint = true;
            //this.SkipPagerHint = true;
        }

#region Garbage

        public SubWindow(int nothing) : base("none")
        {
            DREditor.Instance.Window.AlertProblem("Invalid SubWindow initialization!");
        }
        public SubWindow(IntPtr raw) : base(raw)
        {
            DREditor.Instance.Window.AlertProblem("Invalid SubWindow initialization!");
        }

#endregion
    }
}
