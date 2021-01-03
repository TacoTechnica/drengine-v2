using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEngine;
using GameEngine.Util;
using Gdk;
using Gtk;

namespace DREngine.Editor
{
    public class ResourceView : VBox
    {

        private TreeView _tree;
        private TreeStore _store;
        private Icons Icons => DREditor.Instance.Window.Icons;

        public Action<string, string> OnFileOpened;

        private string _fpath = "";

        public ResourceView()
        {
            _tree = new TreeView
            {
                //EnableSearch = true,
                //HeadersClickable = false,
                HeadersVisible = false
            };

            // Set up generic tree stuff
            TreeViewColumn fileName = new TreeViewColumn {Title = "filename", Visible = true};
            CellRendererText fileNameCell = new CellRendererText ();
            CellRendererPixbuf iconCell = new CellRendererPixbuf();
            fileName.PackStart(iconCell, false);
            fileName.PackStart(fileNameCell, true); // false
            _tree.AppendColumn(fileName);
            fileName.AddAttribute(fileNameCell, "text", 0);
            fileName.AddAttribute(iconCell, "pixbuf", 1);

            _store = new TreeStore(typeof(string), typeof(Pixbuf));

            _tree.Model = _store;

            Label bottom = new Label {Text = "TODO: Add search here"};

            PackStart(_tree, true, true, 4);

            _tree.Show();

            // Double click to open
            _tree.ActivateOnSingleClick = false;

            _tree.RowActivated += TreeOnRowActivated;
        }

        private void TreeOnRowActivated(object o, RowActivatedArgs args)
        {
            TreeIter selected;
            _store.GetIter(out selected, args.Path);
            string path = (string)_store.GetValue(selected, 0);

            // Construct path
            while (true)
            {
                bool success = _store.IterParent(out selected, selected);

                path = (string) _store.GetValue(selected, 0) + "/" + path;
                if (!success)
                {
                    break;
                }
            }
            Debug.Log($"GOT {path}");

            OnFileOpened?.Invoke(path, _fpath + path);
        }

        public void Clear()
        {
            _store.Clear();
        }

        public void LoadDirectory(string fpath, Action<string> OnDirectroyLoad = null, Action<string> OnFileLoad = null)
        {
            _fpath = fpath;

            Queue<string> fqueue = new Queue<string>();

            fqueue.Enqueue(fpath);

            while (fqueue.Count != 0)
            {
                string path = fqueue.Dequeue();
                bool root = (path == fpath);

                string dirname = System.IO.Path.GetFileName( path );

                TreeIter iter = default(TreeIter);
                if (root)
                {
                    // No root node
                    //_store.GetIter(out iter, new TreePath(IntPtr.Zero));
                    //iter = _store.AppendValues(dirname, Icons.Folder);
                }
                else
                {
                    // Find where we are based on our path. Kinda annoying with this framework but whatever.
                    string subPath = System.IO.Path.GetRelativePath(fpath, path);
                    //Debug.Log($"SUBPATH: {subPath}");
                    GetIterFromPath(out iter, _store, subPath);
                }

                // Add directories
                foreach (string dir in Directory.GetDirectories(path))
                {
                    fqueue.Enqueue(dir);
                    string name = System.IO.Path.GetFileName( dir );
                    
                    if (root)
                    {
                        _store.AppendValues(name, Icons.Folder);
                    }
                    else
                    {
                        _store.AppendValues(iter, name, Icons.Folder);
                    }
                    
                    OnDirectroyLoad?.Invoke(dir);
                }

                // Add files
                foreach (string file in Directory.GetFiles(path))
                {
                    string name = System.IO.Path.GetFileName(file);

                    if (root)
                    {
                        _store.AppendValues(name, Icons.File);
                    }
                    else
                    {
                        _store.AppendValues(iter, name, Icons.File);
                    }

                    OnFileLoad?.Invoke(file);
                }
            }
        }

        /// <summary>
        /// I kinda hate how we can't grab a "root" iter and we can't set one iter to another. This is needlessly stupid.
        /// </summary>
        private static bool GetIterFromPath(out TreeIter iter, TreeStore store, string path)
        {
            // This is stupid, why can't I set this to root or null
            // The value here is considered invalid now.
            store.GetIterFirst(out iter);
            bool root = true;
            foreach (string sub in path.Split("/"))
            {
                TreeIter child;
                int counter = 0;
                while (true)
                {
                    bool valid;
                    if (root)
                    {
                        valid = store.IterNthChild(out child, counter);
                    }
                    else
                    {
                        valid = store.IterNthChild(out child, iter, counter);
                    }

                    if (!valid)
                    {
                        store.GetIterFirst(out iter);
                        // We failed
                        return false;
                    }
                    
                    // Check if our value is ok
                    string name = (string)store.GetValue(child, 0);

                    if (name == sub)
                    {
                        break;
                    }
                    
                    ++counter;
                }

                // We found our value at our parent's counter thing. Redo our last work. Yes.
                if (root)
                {
                    store.IterNthChild(out iter, counter);
                }
                else
                {
                    store.IterNthChild(out iter, iter, counter);
                }

                root = false;
            }

            return true;
        }
    }
}
