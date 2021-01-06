using System.Runtime.Serialization.Formatters.Binary;
using GameEngine;
using Gdk;
using Gtk;

namespace DREngine.Editor
{
    public class Icons
    {

        public const int IconSize = 24;
        
        public readonly Pixbuf Folder;
        public readonly Pixbuf TextFile;

        public readonly Pixbuf New;
        public readonly Pixbuf Save;
        public readonly Pixbuf Open;
        public readonly Pixbuf Export;

        public readonly Pixbuf Play;
        public readonly Pixbuf Stop;

        public readonly Pixbuf ImageFile;
        public readonly Pixbuf AudioFile;
        public readonly Pixbuf FontFile;

        public readonly Pixbuf ProjectFile;
        public readonly Pixbuf UnknownFile;

        public Icons()
        {
            /*
            Debug.Log("BEGINNING PRINT...");
            foreach (string ctx in IconTheme.Default.ListContexts())
            {
                foreach (string icon in IconTheme.Default.ListIcons(ctx))
                {
                    if (icon.ToLower().Contains("open") || icon.ToLower().Contains("save")|| icon.ToLower().Contains("new"))
                    Debug.Log($"FOUND: {ctx} : {icon}");
                }
            }
            Debug.Log("done");
            */
            Folder = LoadThemeIcon("inode-directory-symbolic");
            TextFile = LoadThemeIcon("text-x-generic-symbolic");

            New = LoadThemeIcon("document-new");
            Save = LoadThemeIcon("document-save");
            Open = LoadThemeIcon("folder-new-symbolic");
            Export = LoadThemeIcon("applications-games-symbolic");

            Play = LoadThemeIcon("media-playback-start-symbolic");
            Stop = LoadThemeIcon("media-playback-stop-symbolic");

            ImageFile = LoadThemeIcon("image-x-generic-symbolic");
            AudioFile = LoadThemeIcon("audio-x-generic-symbolic");
            FontFile = LoadThemeIcon("font-x-generic-symbolic");
            UnknownFile = LoadThemeIcon("dialog-question-symbolic");
            ProjectFile = LoadThemeIcon("text-editor-symbolic");
        }

        public Pixbuf ScaleToRegularSize(Pixbuf buf, int targetSize = IconSize)
        {
            InterpType interp = InterpType.Bilinear;
            // This part here is kinda silly but whatever
            if (buf.Height == 0)
            {
                return buf.ScaleSimple(targetSize, 0, interp);
            }
            if (buf.Width == 0)
            {
                return buf.ScaleSimple(0, targetSize, interp);
            }
            bool wider = (buf.Width > buf.Height);
            double factor;
            if (wider)
            {
                factor = (double)targetSize / (double)buf.Width;
            }
            else
            {
                factor = (double)targetSize / (double)buf.Height;
            }
            return buf.ScaleSimple((int)(buf.Width * factor), (int)(buf.Height * factor), interp);
        }

        private Pixbuf LoadThemeIcon(string icon, int size = IconSize)
        {
            if (IconTheme.Default.HasIcon(icon))
            {
                return Gtk.IconTheme.Default.LoadIcon(icon, size, (IconLookupFlags)0);
            }
            else
            {
                Debug.LogWarning($"Failed to load icon {icon}");
            }

            return null;
        }
    }
}
