using System.IO;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine;
using Gdk;
using Gtk;
using Key = Gdk.Key;
using Window = Gtk.Window;

namespace DREngine.Editor.Components
{
    public abstract class EasyDialog : Dialog
    {
        
        [FieldIgnore] public bool UseSubmitShortcut = true;
        

        private FieldBox _fields;

        protected Text _post;

        private string _failureString = null;

        // Default buttons
        public EasyDialog(DREditor editor, Window parent, string title="New Project") : this(editor, parent, title, "Create", ResponseType.Accept, "Cancel", ResponseType.Cancel) {}
        public EasyDialog(DREditor editor, Window parent, string title="New Project", params object[] buttondata) : base(title, parent, DialogFlags.DestroyWithParent, buttondata)
        {
            _fields = new FieldBox(editor, GetType(), true);
            _post = new Text("");

            ContentArea.PackStart(_fields, true, true, 16);

            _fields.Modified += OnModified;

            ContentArea.PackEnd(_post, false, false, 16);

            _fields.Show();
            _post.Show();
            _fields.LoadTarget(this);

            this.KeyReleaseEvent += (o, args) =>
            {
                if (UseSubmitShortcut)
                {
                    Key key = args.Event.Key;
                    bool ctrl = (args.Event.State & ModifierType.ControlMask) != 0;

                    if (ctrl && key == Key.Return)
                    {
                        // ? Is this working, I think not.
                        _fields.UnsetFocusChain();
                        // Attempt to submit if we press ctrl+enter.
                        this.Respond(ResponseType.Accept);
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
            {
                if (Run() == ResponseType.Accept)
                {
                    bool failuresStillPresent = !CheckForFailuresPreSubmit() || Failed(); 
                    if (failuresStillPresent)
                    {
                        SetFailure(_failureString);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
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
            _post.Text = message;
            _post.SetMode(Text.Warning);
            _failureString = message;
        }

        protected void ResetFailure()
        {
            _post.SetMode(Text.Normal);
            _failureString = null;
        }

        protected void SetPostText(string text)
        {
            _post.SetMode(Text.Normal);
            _post.Text = text;
            _failureString = null;
        }

        protected abstract void OnModified();

        protected abstract bool CheckForFailuresPreSubmit();
    }
}