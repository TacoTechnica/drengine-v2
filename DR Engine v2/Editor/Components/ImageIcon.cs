using Gdk;
using Gtk;

namespace DREngine.Editor.Components
{
    public class ImageIcon : Image
    {
        public ImageIcon(Pixbuf pixbuf, int width, int height) : base(pixbuf)
        {
            if (Pixbuf != null) Rescale(width, height);
        }

        public ImageIcon(string path, int width, int height) : base(path)
        {
            if (Pixbuf != null) Rescale(width, height);
        }

        public void Rescale(int targetWidth, int targetHeight)
        {
            var old = Pixbuf;
            Pixbuf = Pixbuf.ScaleSimple(targetWidth, targetHeight, InterpType.Hyper);
            old.Dispose();
        }
    }
}