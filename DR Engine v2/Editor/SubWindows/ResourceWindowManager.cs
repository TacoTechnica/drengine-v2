using System.Collections.Generic;
using System.IO;
using DREngine.Editor.SubWindows.Resources;
using DREngine.Editor.SubWindows.Resources.SceneEditor;
using DREngine.Editor.SubWindows.Resources.VNEditor;
using DREngine.ResourceLoading;

namespace DREngine.Editor.SubWindows
{
    /// <summary>
    ///     You want to open up a resource? Here ya go.
    /// </summary>
    public class ResourceWindowManager
    {
        private readonly Dictionary<string, SubWindow> _openWindows = new Dictionary<string, SubWindow>();
        private readonly DREditor _editor;

        public ResourceWindowManager(DREditor editor)
        {
            _editor = editor;
        }

        public SubWindow OpenResource(ProjectPath path)
        {
            if (_openWindows.ContainsKey(path))
            {
                var window = _openWindows[path];
                if (window != null && window.IsOpen)
                {
                    // Make window appear first
                    window.Present();
                    return _openWindows[path];
                }
            }

            var extension = new FileInfo(path.ToString()).Extension;
            if (extension.StartsWith(".")) extension = extension.Substring(1);

            // Open a new window
            var newWindow = CreateResourceWindow(path, extension);
            _openWindows[path] = newWindow;
            newWindow.Initialize();
            if (newWindow is SavableWindow saveWindow) saveWindow.Open(path);
            return newWindow;
        }

        public bool AnyWindowDirty()
        {
            foreach (var window in _openWindows.Values)
                if (window.IsOpen && window is SavableWindow sWindow)
                    if (sWindow.Dirty)
                        return true;

            return false;
        }

        public void ForceCloseAllWindows()
        {
            foreach (var window in _openWindows.Values)
            {
                window.Close();
                window.Dispose();
            }

            _openWindows.Clear();
        }

        private SubWindow CreateResourceWindow(ProjectPath path, string extension)
        {
            if (path.RelativePath == "project.json") return new ProjectSettingsWindow(_editor, path);

            switch (extension)
            {
                case "png":
                    return new SpriteResourceWindow(_editor, path);
                case "json":
                case "txt":
                    return new SimpleTextWindow(_editor, path);
                case "ttf":
                    return new FontResourceWindow(_editor, path);
                case "wav":
                    return new AudioClipResourceWindow(_editor, path);
                case "vn":
                    return new VNResourceWindow(_editor, path);
                case "scene":
                    return new SceneResourceWindow(_editor, path);
            }

            return new UnknownResourceWindow(_editor, path, extension);
        }
    }
}