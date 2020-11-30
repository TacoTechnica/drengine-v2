using GameEngine;
using Gdk;
using Gtk;

namespace DREngine.Editor
{
    public class Icons
    {

        public readonly Pixbuf Folder;
        public readonly Pixbuf File;

        public readonly Pixbuf New;
        public readonly Pixbuf Save;
        public readonly Pixbuf Open;
        public readonly Pixbuf Export;

        public readonly Pixbuf Play;
        public readonly Pixbuf Stop;

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
            Folder = LoadIcon("inode-directory");
            File = LoadIcon("text-x-generic");

            New = LoadIcon("document-new");
            Save = LoadIcon("document-save");
            Open = LoadIcon("folder-new-symbolic");
            Export = LoadIcon("applications-games-symbolic");

            Play = LoadIcon("media-playback-start-symbolic");
            Stop = LoadIcon("media-playback-stop-symbolic");
        }

        private Pixbuf LoadIcon(string icon, int size = 24)
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
