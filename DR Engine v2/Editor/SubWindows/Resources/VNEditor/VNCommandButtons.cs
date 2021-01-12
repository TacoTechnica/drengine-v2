using DREngine.Game.VN;
using Gdk;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources.VNEditor
{
    public class DialogueCommandButton : CommandButton<DialogCommand>
    {
        private TextView _example;

        public DialogueCommandButton(DREditor editor) : base(editor, editor.Icons.Play)
        {
        }

        protected override void Initialize(VBox content)
        {
            _example = new TextView();
            content.PackStart(_example, true, true, 16);
            _example.Show();
        }

        protected override void OnLoad(DialogCommand command)
        {
            _example.Buffer.Text = Command.Name + ": " + Command.Text;
        }
    }

    public class UnknownCommandButton : CommandButton<VNCommand>
    {
        private Label _label;

        // TODO: Generate icon based on type name.
        public UnknownCommandButton(DREditor editor) : base(editor, editor.Icons.UnknownFile) { }

        protected override void Initialize(VBox content)
        {
            _label = new Label("Unknown");
            content.PackStart(_label, false, false, 16);
            _label.Show();
        }

        protected override void OnLoad(VNCommand command)
        {
            _label.Text = "Command: " + command.GetType();
        }
    }

    #region Abstracts

    public abstract class CommandButton<T> : BaseCommandButton where T : VNCommand
    {
        protected T Command;
        public override VNCommand VNCommand
        {
            get => Command;
        }

        protected CommandButton(DREditor editor, Pixbuf icon) : base(editor, icon) { }

        public override void Load(VNCommand command)
        {
            Command = (T)command;
            OnLoad(Command);
        }

        protected abstract void OnLoad(T command);
    }

    public abstract class BaseCommandButton : ListBoxRow
    {
        public abstract VNCommand VNCommand { get; }

        private Widget _buffer;

        private Pixbuf _icon;

        private Image _img;
        
        public BaseCommandButton(DREditor editor, Pixbuf icon)
        {
            _icon = icon;

            // These are kinda useless sadly.
            this.Activatable = true;
            this.Selectable = true;
        }

        public void Initialize(Widget buffer)
        {
            _buffer = buffer;
            VBox outer = new VBox();
            Add(outer);
            HBox inner = new HBox();
            _img = new Image(_icon);
            inner.PackStart(_img, false, true, 16);
            _img.Show();
            VBox innerInner = new VBox();
            outer.PackStart(_buffer, true, true, 0);
            innerInner.HeightRequest = 64;
            Initialize(innerInner);
            inner.PackStart(innerInner, true, true,16);
            innerInner.Show();
            outer.PackStart(inner, true, true, 0);
            inner.Show();
            outer.Show();
        }

        public void ShowBuffer()
        {
            _buffer.HeightRequest = 60;
            _buffer.Visible = true;
            QueueDraw();
        }

        public void HideBuffer()
        {
            _buffer.HeightRequest = 0;
            _buffer.Visible = false;
            QueueDraw();
        }

        public Widget GetDragHandle()
        {
            return this;
        }
        
        protected abstract void Initialize(VBox content);

        public abstract void Load(VNCommand command);
    }
    
    #endregion
}
