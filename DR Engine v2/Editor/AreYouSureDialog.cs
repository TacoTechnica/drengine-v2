using Gtk;

namespace DREngine.Editor
{
    public class AreYouSureDialog : Dialog
    {
        public AreYouSureDialog(Window parent, string title, string message, string accept = "Yes", string cancel = "Cancel") : base(
            title, parent, DialogFlags.DestroyWithParent, accept, ResponseType.Accept, cancel, ResponseType.Cancel)
        {
            Label label = new Label(message);
            label.Xpad = 32;
            label.Ypad = 32;
            label.Wrap = true;
            this.ContentArea.Add(label);
            label.Show();
        }
    }
}