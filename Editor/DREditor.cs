using System;
using System.Drawing;

using Gtk;

namespace DREngine
{
    public class DREditor : IDisposable
    {
        private bool _disposed = false;

        public DREditor()
        {

        }
        ~DREditor() {
            Dispose(false);
        }

        public void Run()
        {
            Initialize();
        }

        private void Initialize() {
            Debug.LogDebug("Editor Initialize()");

            // Init app
            Application.Init();
            // Create window
            Window _window = new Window("DREditor beta");
            _window.Resize(200, 200);

            // Create label with some text
            Label tempLabel = new Label("Hello!");
            // Add it to window
            _window.Add(tempLabel);

            // Show everything
            _window.ShowAll();
            Application.Run();
        }

#region Resource Management

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (_disposed) return;
            if(disposing)
            {
                // TODO: Dispose managed resources.
                // ex: component.Dispose();
            }

            // TODO: Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.
            //CloseHandle(handle);
            //handle = IntPtr.Zero;

            // Note disposing has been done.
            _disposed = true;
        }
#endregion
    }
}
