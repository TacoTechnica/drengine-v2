using DREngine.Editor.SubWindows.FieldWidgets;
using Gdk;
using Gtk;
using Key = Gdk.Key;
using Window = Gtk.Window;

namespace DREngine.Editor.Components
{
    public abstract class EasyDialog : Dialog
    {
        private string _failureString;

        protected readonly Text Post;

        [FieldIgnore] public bool UseSubmitShortcut = true;

        // Default buttons
        public EasyDialog(DREditor editor, Window parent, string title = "New Project") : this(editor, parent, title,
            "Create", ResponseType.Accept, "Cancel", ResponseType.Cancel)
        {
        }

        public EasyDialog(DREditor editor, Window parent, string title = "New Project", params object[] buttondata) :
            base(title, parent, DialogFlags.DestroyWithParent, buttondata)
        {
            var fields = new FieldBox(editor, GetType(), true);
            Post = new Text("");

            ContentArea.PackStart(fields, true, true, 16);

            fields.Modified += OnModified;

            ContentArea.PackEnd(Post, false, false, 16);

            fields.Show();
            Post.Show();
            fields.LoadTarget(this);

            KeyReleaseEvent += (o, args) =>
            {
                if (UseSubmitShortcut)
                {
                    var key = args.Event.Key;
                    var ctrl = (args.Event.State & ModifierType.ControlMask) != 0;

                    if (ctrl && key == Key.Return)
                    {
                        // ? Is this working, I think not.
                        fields.UnsetFocusChain();
                        // Attempt to submit if we press ctrl+enter.
                        Respond(ResponseType.Accept);
                    }
                }
            };
        }

        public new ResponseType Run()
        {
            return (ResponseType) base.Run();
        }

        public bool RunUntilAccept()
        {
            while (true)
                if (Run() == ResponseType.Accept)
                {
                    var failuresStillPresent = !CheckForFailuresPreSubmit() || Failed();
                    if (failuresStillPresent)
                        SetFailure(_failureString);
                    else
                        return true;
                }
                else
                {
                    return false;
                }
        }

        public string GetFailureString()
        {
            return _failureString;
        }

        public bool Failed()
        {
            return _failureString != null;
        }

        protected void SetFailure(string message)
        {
            Post.Text = message;
            Post.SetMode(Text.LabelType.Warning);
            _failureString = message;
        }

        protected void ResetFailure()
        {
            Post.SetMode(Text.LabelType.Normal);
            _failureString = null;
        }

        protected void SetPostText(string text)
        {
            Post.SetMode(Text.LabelType.Normal);
            Post.Text = text;
            _failureString = null;
        }

        protected abstract void OnModified();

        protected abstract bool CheckForFailuresPreSubmit();
    }
}