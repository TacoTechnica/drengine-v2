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

        private TreeStore _store;
        private Icons Icons => DREditor.Instance.Window.Icons;

        public ResourceView()
        {
            TreeView tree = new TreeView
            {
                //EnableSearch = true,
                //HeadersClickable = false,
                //HeadersVisible = false
            };

            // Set up generic tree stuff
            TreeViewColumn fileName = new TreeViewColumn {Title = "filename", Visible = true};
            CellRendererText fileNameCell = new CellRendererText ();
            CellRendererPixbuf iconCell = new CellRendererPixbuf();
            fileName.PackStart(iconCell, false);
            fileName.PackStart(fileNameCell, true); // false
            tree.AppendColumn(fileName);
            fileName.AddAttribute(fileNameCell, "text", 0);
            fileName.AddAttribute(iconCell, "pixbuf", 1);

            _store = new TreeStore(typeof(string), typeof(Pixbuf));

            tree.Model = _store;

            Label bottom = new Label() {Text = "TODO: Add search here"};

            PackStart(tree, true, true, 4);
            tree.Show();
            //b.PackEnd(bottom, false, false, 4);
            //bottom.Show();
        }



        public void Clear()
        {
            _store.Clear();
        }

        public void LoadDirectory(string fpath, Action<string> OnDirectroyLoad = null, Action<string> OnFileLoad = null)
        {
            Queue<string> fqueue = new Queue<string>();

            fqueue.Enqueue(fpath);

            while (fqueue.Count != 0)
            {
                string path = fqueue.Dequeue();
                bool root = (path == fpath);

                string dirname = System.IO.Path.GetFileName( path );

                TreeIter iter;
                if (root)
                {
                    iter = _store.AppendValues(dirname, Icons.Folder);
                }
                else
                {
                    string parentPath = System.IO.Directory.GetParent(path).FullName;
                    string subPath = System.IO.Path.GetRelativePath(fpath, parentPath);
                    TreeIter parent;
                    //Debug.Log($"PARENT: {path}=>{parentPath} with regards to {fpath}: {subPath}");
                    if (subPath == ".")
                    {
                        // parent is root
                        _store.GetIterFirst(out parent);
                    }
                    else
                    {
                        _store.GetIter(out parent, new TreePath(subPath));
                    }

                    iter = _store.AppendValues(parent, dirname, Icons.Folder);
                }

                foreach (string dir in Directory.GetDirectories(path))
                {
                    string name = System.IO.Path.GetFileName( dir );
                    TreeIter sub;
                    fqueue.Enqueue(dir);
                    OnDirectroyLoad?.Invoke(dir);
                }

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
            /*
            Queue<Pair<string, TreeIter>> fqueue = new Queue<Pair<string, TreeIter>>();

            fqueue.Enqueue(new Pair<string, TreeIter>(null, TreeIter.Zero));

            while (fqueue.Count != 0)
            {
                var p = fqueue.Dequeue();
                string path = p.First;
                TreeIter iter = p.Second;
                bool root = (path == null);
                if (root)
                {
                    path = fpath;
                }

                string dirname = System.IOHelper.Path.GetFileName( path );
                Debug.Log($"PATH: {dirname}");

                foreach (string dir in Directory.GetDirectories(path))
                {
                    string name = System.IOHelper.Path.GetFileName( dir );
                    TreeIter sub;
                    if (root)
                    {
                        sub = _store.AppendValues(name);
                    }
                    else
                    {
                        sub = _store.AppendValues(iter, name);
                    }
                    fqueue.Enqueue(new Pair<string, TreeIter>(dir, sub));
                    OnDirectroyLoad?.Invoke(dir);
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    string name = System.IOHelper.Path.GetFileName(path);
                    if (root)
                    {
                        _store.AppendValues(name);
                    }
                    else
                    {
                        _store.AppendValues(iter, name);
                    }
                    OnFileLoad?.Invoke(file);
                }
            }
            */

            // TODO: Standardize into functions and turn into a more defined class.
            /*
            TreeIter iter = store.AppendValues("Rooms");
            store.AppendValues(iter, "floor0.room");
            store.AppendValues(iter, "my_room.room");
            store.AppendValues(iter, "trial.room");

            iter = store.AppendValues("Characters");
            store.AppendValues(iter, "billy");
            store.AppendValues(iter, "you");

            iter = store.AppendValues("Sprites");
            store.AppendValues(iter, "billy_sad");
            store.AppendValues(iter, "billy_happy");
            TreeIter nagitoSub = store.AppendValues(iter, "nagito");
            store.AppendValues(nagitoSub, "nagito1");
            store.AppendValues(nagitoSub, "nagito2");
            store.AppendValues(iter, "obama");
            */


        }

    }
}
