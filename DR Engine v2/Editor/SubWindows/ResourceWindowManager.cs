using System.Collections.Generic;
using System.IO;

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

            // Open a new window
            SubWindow newWindow = CreateResourceWindow(path, extension);
            _openWindows[path] = newWindow;
            newWindow.Initialize();
            return newWindow;
        }

        private SubWindow CreateResourceWindow(ProjectPath path, string extension)
        {
            return new UnknownResourceWindow(_editor, path, extension);
        }
    }
}
