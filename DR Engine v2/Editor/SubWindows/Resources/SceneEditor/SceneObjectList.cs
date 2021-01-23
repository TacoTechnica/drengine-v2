using System;
using System.Collections.Generic;
using DREngine.Editor.Components;
using DREngine.Game.Scene;
using Gtk;

namespace DREngine.Editor.SubWindows.Resources.SceneEditor
{
    public class SceneObjectList : VBox
    {
        private ListBox _items;

        public Action<Type> NewObjectAdded;
        public Action<int> ObjectSelected;

        private IChooser _currentChoice;

        private bool _skipSelectFlag = false;

        public SceneObjectList(DREditor editor)
        {
            // Initialize UI
            ScrolledWindow view = new ScrolledWindow();
            Button newButton = new Button();
            newButton.Image = new Image(editor.Icons.Add);
            _items = new ListBox();

            view.Add(_items);

            this.PackStart(view, true, true, 16);
            this.PackEnd(newButton, true, false, 16);

            newButton.Pressed += (sender, args) =>
            {
                PresentNewItemDropdown();
            };

            _items.RowSelected += (o, args) =>
            {
                if (_skipSelectFlag)
                {
                    _skipSelectFlag = false;
                    return;
                }
                ObjectSelected?.Invoke(args.Row.Index);
            };
        }

        public void Clear()
        {
            foreach (Widget child in _items.Children)
            {
                _items.Remove(child);
            }
        }

        public void LoadItems(IEnumerable<ISceneObject> objects)
        {
            Clear();

            foreach (ISceneObject sceneObject in objects)
            {
                if (sceneObject.Name == "") sceneObject.Name = null;
                AddItemVisual(sceneObject.Name ?? TypeToName(sceneObject.GetType()), sceneObject.GetType());
            }
        }

        public void RemoveItemVisual(int index)
        {
            _items.Remove(_items.Children[index]);
        }

        public void ForceSelect(int index)
        {
            _skipSelectFlag = true;
            _items.SelectRow(_items.GetRowAtIndex(index));
        }

        private void PresentNewItemDropdown()
        {
            _currentChoice?.Cancel();
            _currentChoice = new DropdownChooser();
            _currentChoice.MakeChoice<Type>(GetObjectTypes(), type =>
            {
                var newRow = AddItemVisual(TypeToName(type), type);
                NewObjectAdded.Invoke(type);
                //_skipSelectFlag = true;
                _items.SelectRow(newRow);
            }, TypeToName);
        }

        private ListBoxRow AddItemVisual(string name, Type type)
        {
            Row newRow = new Row(name, type);

            _items.Insert(newRow, _items.Children.Length);
            newRow.Show();

            return newRow;
        }

        private static string TypeToName(Type type)
        {
            return type.Name;
        }

        private IEnumerable<Type> GetObjectTypes()
        {
            yield return typeof(Cube);
            yield return typeof(Billboard);
        }

        public void RenameObject(int objectIndex, string name)
        {
            ((Row) _items.GetRowAtIndex(objectIndex)).Name = name;
        }

        public void ResetObjectName(int objectIndex)
        {
            ((Row) _items.GetRowAtIndex(objectIndex)).ResetDefaultName();
        }

        private class Row : ListBoxRow
        {

            public string Name
            {
                get => _label.Text;
                set => _label.Text = value;
            }
            
            private Label _label;
            private Type _type;

            public Row(string name, Type type)
            {
                _type = type;
                _label = new Label(name);
                Add(_label);
                _label.Show();
            }

            public void ResetDefaultName()
            {
                Name = TypeToName(_type);
            }
        }
    }
}
