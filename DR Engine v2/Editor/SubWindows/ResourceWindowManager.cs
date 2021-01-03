using System.Collections.Generic;
using System.IO;
using DREngine.Editor.SubWindows.Resources;

namespace DREngine.Editor.SubWindows
{
    /// <summary>
    /// You want to open up a resource? Here ya go.
    /// </summary>
    public class ResourceWindowManager
    {
        private DREditor _editor;

        private readonly Dictionary<string, SubWindow> _openWindows = new Dictionary<string, SubWindow>();

        public ResourceWindowManager(DREditor editor)
        {
            _editor = editor;
        }
        
        public SubWindow OpenResource(ProjectPath path)
        {
            if (_openWindows.ContainsKey(path))
            {
                SubWindow window = _openWindows[path];
                if (window != null && window.IsOpen)
                {
                    // Make window appear first
                    window.Present();
                    return _openWindows[path];
                }
            }

            string extension = new FileInfo(path.ToString()).Extension;
            if (extension.StartsWith(".")) extension = extension.Substring(1);

            // Open a new window
            SubWindow newWindow = CreateResourceWindow(path, extension);
            _openWindows[path] = newWindow;
            newWindow.Initialize();
            if (newWindow is SavableWindow saveWindow)
            {
                saveWindow.Open(path);
            }
            return newWindow;
        }

        private SubWindow CreateResourceWindow(ProjectPath path, string extension)
        {
            if (path.RelativePath == "/project.json")
            {
                return new ProjectSettingsWindow(_editor, path);
            }

            switch (extension)
            {
                case "png":
                    return new SpriteResourceWindow(_editor, path);
                case "json":
                case "txt":
                    return new SimpleTextWindow(_editor, path);
            }
            return new UnknownResourceWindow(_editor, path, extension);
        }
    }
}
