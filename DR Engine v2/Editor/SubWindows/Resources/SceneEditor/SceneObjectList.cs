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

        public Action<Type> NewItemAdded;
        public Action<int> ItemSelected;

        private IChooser _currentChoice;

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
                ItemSelected?.Invoke(args.Row.Index);
            };
        }

        public void Clear()
        {
            foreach (Widget child in _items.Children)
            {
                _items.Remove(child);
            }
        }

        public void SetItemsVisual(IEnumerable<string> names)
        {
            Clear();

            foreach (string name in names)
            {
                AddItemVisual(name);
            }
        }

        public void RemoveItemVisual(int index)
        {
            _items.Remove(_items.Children[index]);
        }

        private void PresentNewItemDropdown()
        {
            _currentChoice?.Cancel();
            _currentChoice = new DropdownChooser();
            _currentChoice.MakeChoice<Type>(GetObjectTypes(), (type =>
            {
                string name = type.ToString();
                var newRow = AddItemVisual(name);
                _items.SelectRow(newRow);
            }));
        }

        private ListBoxRow AddItemVisual(string name)
        {
            ListBoxRow newRow = new ListBoxRow();

            Label label = new Label(name);
            newRow.Add(label);
            label.Show();

            _items.Insert(newRow, _items.Children.Length);

            return newRow;
        }

        private IEnumerable<Type> GetObjectTypes()
        {
            yield return typeof(Cube);
            yield return typeof(Billboard);
        }
    }
}
