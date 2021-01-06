using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GameEngine;
using GameEngine.Util;
using Gdk;
using Gtk;

namespace DREngine.Editor
{
    public class GenericTreeView : VBox
    {

        // Only used for directory stuff
        private string _fpath = "";

        private TreeView _tree;
        private TreeStore _store;
        private Icons Icons;

        public Action<string, string> OnFileOpened;

        public Action<string, string> OnFileRightClicked;

        public GenericTreeView(Icons icons)
        {
            Icons = icons;

            _tree = new TreeView
            {
                EnableSearch = true,
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

            PackStart(_tree, true, true, 4);

            _tree.Show();

            // Double click to open
            _tree.ActivateOnSingleClick = false;

            _tree.RowActivated += TreeOnRowActivated;

            _tree.ButtonReleaseEvent += TreeOnButtonReleaseEvent;

        }

        private void TreeOnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            var evt = args.Event;
            if (evt.Type == EventType.ButtonRelease && evt.Button == 3)
            {
                TreeIter selected;
                _tree.Selection.GetSelected(out selected);
                string path = GetPathFromIter(ref selected, _store);
                OnFileRightClicked?.Invoke(path, _fpath + path);
            }
        }

        private void TreeOnRowActivated(object o, RowActivatedArgs args)
        {
            TreeIter selected;
            _store.GetIter(out selected, args.Path);

            string path = GetPathFromIter(ref selected, _store);
            OnFileOpened?.Invoke(path, _fpath + path);
        }

        public void Clear()
        {
            _store.Clear();
        }

        public void ExpandAll()
        {
            _tree.ExpandAll();
        }

        public void LoadDirectory(string fpath, Action<string> OnDirectroyLoad = null, Action<string> OnFileLoad = null)
        {
            _fpath = fpath;

            Queue<string> fqueue = new Queue<string>();

            fqueue.Enqueue(fpath);

            while (fqueue.Count != 0)
            {
                string path = fqueue.Dequeue();

                if (Ignore(path)) continue;

                bool root = (path == fpath);

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
                    if (Ignore(dir)) continue;
    
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
                    if (Ignore(file)) continue;
                    string relativePath = System.IO.Path.GetRelativePath(fpath, file);

                    string name = System.IO.Path.GetFileName(file);

                    if (root)
                    {
                        _store.AppendValues(name, GetFileIcon(relativePath, file));
                    }
                    else
                    {
                        _store.AppendValues(iter, name, GetFileIcon(relativePath, file));
                    }

                    OnFileLoad?.Invoke(file);
                }
            }
        }

        /// <summary>
        /// Add a file at a path and optionally add its parent directories.
        /// </summary>
        public void AddFile(string path, bool autoAddParentDirs = false)
        {
            AddItem(path, true, autoAddParentDirs);
        }

        public void AddFolder(string path, bool autoAddParentDirs = false)
        {
            AddItem(path, false, autoAddParentDirs);
        }
        
        private void AddItem(string path, bool isFile, bool autoAddParentDirs)
        {
            if (Ignore(path)) return;
            
            TreeIter iter;
            // This is stupid, why can't I set this to root or null
            // The value here is considered invalid now.
            _store.GetIterFirst(out iter);

            if (path.Length == 0) throw new InvalidOperationException("Can't add an empty path!");

            string[] splitted = path.Split("/");
            bool root = true;
            int counter = 0;
            foreach (string sub in splitted)
            {
                bool last = (counter++ == splitted.Length);

                bool addLast = (last && isFile);
                
                if (!GetTreeIterChild(ref iter, iter, _store, sub, root))
                {
                    if (autoAddParentDirs)
                    {
                        // Add this parent
                        if (root)
                        {
                            if (addLast)
                            {
                                iter = _store.AppendValues(sub, Icons.Folder);
                            }
                            else
                            {
                                iter = _store.InsertWithValues(0, sub, Icons.Folder);
                            }
                        }
                        else
                        {
                            if (addLast)
                            {
                                iter = _store.AppendValues(iter, sub, Icons.Folder);
                            }
                            else
                            {
                                iter = _store.InsertWithValues(iter, 0, sub, Icons.Folder);
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Failed to add File to our tree view. Directory \"{sub}\" does not exist for path \"{path}\"");
                    }
                }
                root = false;
            }

            _store.SetValue(iter, 1, isFile? GetFileIcon(path,splitted.Last()) : Icons.Folder);
        }

        /// <summary>
        /// Remove a file at a path and optionally delete all parent directories that became empty.
        /// </summary>
        public void RemoveFile(string path, bool deleteParentsIfEmpty = false)
        {
            TreeIter iter;
            if (!GetIterFromPath(out iter, _store, path))
            {
                throw new InvalidOperationException($"Tried to remove nonexistent file at {path}");
            }

            TreeIter parent;
            bool hasParent = _store.IterParent(out parent, iter);

            _store.Remove(ref iter);

            if (deleteParentsIfEmpty)
            {
                while (hasParent)
                {
                    bool shouldDelete = !_store.IterHasChild(parent);

                    // If we shouldn't delete this, we definitely won't delete its parents.
                    if (!shouldDelete) break;

                    TreeIter nextInLine;
                    hasParent = _store.IterParent(out nextInLine, parent);

                    // At this point, we're deleting (shouldDelete = true)
                    _store.Remove(ref parent);

                    parent = nextInLine;
                }
            }
        }

        public IEnumerable<string> GetAllPaths(bool onlyFiles)
        {
            TreeIter garbage = TreeIter.Zero;
            foreach (string s in GetAllPaths(garbage, _store, onlyFiles, true))
            {
                yield return s;
            }
        }

        // Basically a recursive helper.
        private static IEnumerable<string> GetAllPaths(TreeIter iter, TreeStore store, bool onlyFiles, bool root)
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
                    // We're out of range.
                    //store.GetIterFirst(out iter);
                    yield break;
                }

                // Check if our value is ok
                string path = (string)store.GetValue(child, 0);

                bool isDirectory = store.IterHasChild(child);

                if (!isDirectory || !onlyFiles)
                {
                    yield return path;
                }

                // If path has children, iterate through all of its children.
                if (isDirectory)
                {
                    foreach (string s in GetAllPaths(child, store, onlyFiles, false))
                    {
                        yield return path + "/" + s;
                    }
                }
                
                ++counter;
            }
        }

        /// <summary>
        /// I kinda hate how we can't grab a "root" iter and we can't set one iter to another. This is needlessly stupid.
        /// </summary>
        private static bool GetIterFromPath(out TreeIter iter, TreeStore store, string path)
        {
            // This is stupid, why can't I set this to root or null
            // The value here is considered invalid now.
            iter = TreeIter.Zero;
            bool root = true;
            foreach (string sub in path.Split("/"))
            {
                if (!GetTreeIterChild(ref iter, iter, store, sub, root))
                {
                    return false;
                }

                root = false;
            }
            return true;
        }

        private static string GetPathFromIter(ref TreeIter iter, TreeStore store)
        {
            string path = (string)store.GetValue(iter, 0);

            // Construct path
            while (true)
            {
                bool success = store.IterParent(out iter, iter);

                if (!success)
                {
                    break;
                }

                path = (string) store.GetValue(iter, 0) + "/" + path;
            }

            path = "/" + path;

            return path;
        }
        
        /// <summary>
        /// Given a treeiter, get the next treeiter that has a subpath. 
        /// </summary>
        /// <returns></returns>
        private static bool GetTreeIterChild(ref TreeIter iter, TreeIter parent, TreeStore store, string subPath, bool root = false)
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
                    valid = store.IterNthChild(out child, parent, counter);
                }

                if (!valid)
                {
                    // We're out of range.
                    //store.GetIterFirst(out iter);
                    return false;
                }

                // Check if our value is ok
                string name = (string)store.GetValue(child, 0);

                if (name == subPath)
                {
                    break;
                }
                    
                ++counter;
            }

            if (root)
            {
                store.IterNthChild(out iter, counter);
            }
            else
            {
                store.IterNthChild(out iter, parent, counter);
            }

            return true;
        }

        protected virtual bool Ignore(string path)
        {
            return path.EndsWith(".extra");
        }

        private Pixbuf GetFileIcon(string relativePath, string fullPath)
        {
            if (relativePath == "project.json")
            {
                return Icons.ProjectFile;
            }

            if (relativePath == "icon.png")
            {
                Pixbuf buf = new Pixbuf(fullPath);
                Pixbuf result = Icons.ScaleToRegularSize(buf);
                buf.Dispose();
                return result;
            }

            string extension = System.IO.Path.GetExtension(fullPath);
            if (extension.StartsWith(".")) extension = extension.Substring(1);

            switch (extension)
            {
                case "png":
                    return Icons.ImageFile;
                case "wav":
                    return Icons.AudioFile;
                case "ttf":
                    return Icons.FontFile;
                case "txt":
                case "json":
                    return Icons.TextFile;
            }

            return Icons.UnknownFile;
        }

    }
}