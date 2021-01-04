
using Gdk;
using Gtk;

namespace DREngine.Editor
{
    public class ImageIcon : Image
    {
        public ImageIcon(Pixbuf pixbuf, int width, int height) : base(pixbuf)
        {
            Rescale(width, height);
        }

        public ImageIcon(string path, int width, int height) : base(path)
        {
            Rescale(width, height);
        }

        public void Rescale(int targetWidth, int targetHeight)
        {
            Pixbuf old = Pixbuf;
            Pixbuf = Pixbuf.ScaleSimple(targetWidth, targetHeight, InterpType.Hyper);
            old.Dispose();
        }
    }
}