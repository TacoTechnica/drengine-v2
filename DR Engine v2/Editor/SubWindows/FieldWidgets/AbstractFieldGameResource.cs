using System;
using System.Collections.Generic;
using System.Reflection;
using GameEngine;
using GameEngine.Game;
using Gtk;

namespace DREngine.Editor.SubWindows.FieldWidgets
{
    public abstract class AbstractFieldGameResource<T> : FieldWidget<T>
    {
        protected override T Data
        {
            get
            {
                return ResourceData;
            }
            set
            {
                _button.Label = ResourceToString(value);
                ResourceData = value;
            }
        }
        
        protected abstract T ResourceData { get; set; }

        private DREditor _editor;
        
        private Button _button;
        private ChooserWindow<T> _chooser;
//        private Menu _menu;

        private Type _type;

        public AbstractFieldGameResource(DREditor editor, Type type)
        {
            _editor = editor;
            _type = type;
        }


        protected override void Initialize(FieldInfo field, HBox content)
        {
            _button = new Button();
            _button.Label = "(not loaded)";
            _button.Image = null;
    
            _chooser = new ChooserWindow<T>(_editor,this, _type, "Choose Resource");
            _chooser.DeleteEvent += (o, args) =>
            {
                this.Window.Focus(0);
            };
            _chooser.PathSelected += s =>
            {
                if (s.StartsWith("/")) s = s.Substring(1);
                OnPathSelected(s);
                _chooser.Close();
                _chooser.Hide();
            };

            _button.Pressed += (sender, args) =>
            {
                if (_chooser.IsOpen)
                {
                    _chooser.Present();
                }
                else
                {
                    _chooser.Initialize();
                    _chooser.Show();
                }
            };

            content.PackStart(_button, true, true, 4);
            _button.Show();
        }

        protected abstract string ResourceToString(T resource);

        protected abstract void OnPathSelected(string path);

        protected abstract IEnumerable<string> GetExtraResults(Type t, string path);

        protected abstract bool AcceptPath(ProjectPath path);

        class ChooserWindow<T> : SavableWindow
        {
            // hmmm this is only here for one extra function.
            private AbstractFieldGameResource<T> _parent;

            private TextView _searchBar;

            private GenericTreeView _tree;

            private readonly DREditor _editor;

            private Type _type;

            public Action<string> PathSelected;

            public ChooserWindow(DREditor editor, AbstractFieldGameResource<T> parent, Type type, string title = "") : base(editor, null, false)
            {
                _editor = editor;
                _parent = parent;
                Title = "Choose Resource";
                _type = type;
            }

            protected override void OnInitialize(Box container)
            {
                HBox b = new HBox();
                Label searchLabel = new Label("Search:");
                _searchBar = new TextView();
                _tree = new GenericTreeView(_editor.Icons);

                b.PackStart(searchLabel, false, false, 16);
                b.PackEnd(_searchBar, true, true, 16);
                
                _searchBar.TooltipText = "Search here...";

                _tree.OnFileOpened += (s, s1) =>
                {
                    ProjectPath pp = new ProjectPath(_editor, s);
                    if (_parent.AcceptPath(pp))
                    {
                        PathSelected.Invoke(s);
                    }
                };

                
                searchLabel.Show();
                _searchBar.Show();
                _tree.Show();

                b.Show();

                container.PackStart(b, false, false, 4);
                container.PackStart(_tree, true, true, 4);

                Add(container);
                container.Show();

                _searchBar.Buffer.Changed += (sender, args) =>
                {
                    OnSearch(_type, _searchBar.Buffer.Text);
                };

                // Kick search off
                OnSearch(_type, "");
            }


            protected override void OnOpen(Path path, Box container)
            {
                throw new InvalidOperationException("Shouldn't open.");
            }

            protected override void OnSave(Path path)
            {
                throw new InvalidOperationException("Shouldn't save.");
            }

            protected override void OnLoadError(bool fileExists, Exception exception)
            {
                throw exception;
            }

            protected override void OnClose()
            {
                // Nothing.
            }

            private void OnSearch(Type type, string search)
            {
                //Debug.Log($"SEARCHED: \"{search}\" for type {type}");
                // Update search
                HashSet<string> toAdd = new HashSet<string>();

                IEnumerable<string> needed = GetResults(type, search);
                foreach (string need in needed)
                {
                    toAdd.Add(need);
                }

                List<string> toDelete = new List<string>();

                // Go through everything and remove if needed
                foreach (string path in _tree.GetAllPaths(true))
                {
                    if (toAdd.Contains(path))
                    {
                        // We were already added. No action here.
                        toAdd.Remove(path);
                    }
                    else
                    {
                        // We exist but should not. DELETE!
                        toDelete.Add(path);
                    }
                }
                // Add toAdd
                foreach (string path in toAdd)
                {
                    AddItem(path);
                }
                // Delete toDelete
                foreach (string path in toDelete)
                {
                    RemoveItem(path);
                }

                _tree.ExpandAll();
            }

            private IEnumerable<string> GetResults(Type type, string search)
            {
                foreach (string s in GetExtraResults(type, search))
                {
                    yield return s;
                }
                foreach (string s in _editor.ResourceNameCache.GetPathsMatchingSearch(type, search))
                {
                    yield return s;
                }
            }

            protected IEnumerable<string> GetExtraResults(Type type, string search)
            {
                foreach (string st in _parent.GetExtraResults(type, search))
                {
                    yield return st;
                }
            }

            private void RemoveItem(string path)
            {
                _tree.RemoveFile(path, true);
            }

            private void AddItem(string path)
            {
                _tree.AddFile(path, true);
            }
        }
    }
}
