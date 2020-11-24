
using System;
using System.Collections.Generic;
using GameEngine.Game.Input;

namespace GameEngine.Game.UI
{
    /// <summary>
    /// A simple menu list that lets you use the keyboard to select our elements.
    /// </summary>
    public class MenuList : IMenu
    {
        public bool UseMouse { get; set; }

        private IMenuItem _selected
        {
            get
            {
                if (_selectedIndex == -1) return null;
                return _items[_selectedIndex];
            }
        }
        private int _selectedIndex = -1;
        private IMenuItem _pressed = null;

        private List<IMenuItem> _items = new List<IMenuItem>();

        public IEnumerable<IMenuItem> Children => _items;

        public MenuList(GamePlus game, InputActionButton selectAction, InputActionButton mouseSelect, InputActionButton nextAction, InputActionButton prevAction)
        {
            selectAction.Pressed += OnSelectionPressKeyboard;
            if (mouseSelect != null)
            {
                UseMouse = true;
                mouseSelect.Pressed += OnSelectionPressMouse;
            }
            else
            {
                UseMouse = false;
            }

            nextAction.Pressed += OnPressedNext;
            prevAction.Pressed += OnPressedPrev;
        }
        // Constructor without mouse
        public MenuList(GamePlus game, InputActionButton selectAction, InputActionButton nextAction, InputActionButton prevAction) : this(game, selectAction, null, nextAction, prevAction) {}

        public MenuList AddChild(IMenuItem item)
        {
            item.ParentMenu = this;
            _items.Add(item);
            return this;
        }

        public void RemoveChild(IMenuItem item)
        {
            _items.Remove(item);
            if (_items.Count != 0)
            {
                if (_selectedIndex == _items.Count)
                {
                    // If we're at the end, our "next" item will be at 0.
                    _selectedIndex = 0;
                    SelectItem(0);
                }
                else
                {
                    // Select the next item that is now at the same index.
                    SelectItem(_selectedIndex);
                }
            }
            else
            {
                // We removed our last child
                _selectedIndex = -1;
            }
        }

        public void SetSelected(IMenuItem item)
        {
            int newIndex = _items.IndexOf(item);
            if (newIndex == -1) throw new InvalidOperationException("Cannot select menu item that wasn't added to the menu list!");
            // Deselect previous
            DeselectItem(_selectedIndex);
            // Select new
            // We assume that item is in the list.
            _selectedIndex = newIndex;
            SelectItem(newIndex);
        }

        private void DeselectItem(int index)
        {
            if (index == -1) return;
            IMenuItem item = _items[index];
            if (item.MenuSelected)
            {
                item.MenuSelected = false;
                item.OnMenuDeselect();
            }
        }
        private void SelectItem(int index)
        {
            IMenuItem item = _items[index];
            if (!item.MenuSelected)
            {
                item.MenuSelected = true;
                item.OnMenuSelect();
            }
        }

        private void DeselectCursorItem(int index)
        {
            if (index == -1) return;
            IMenuItem item = _items[index];
            // If we can be selected by a cursor, deselect that.
            if (item is ICursorSelectable cursorSelectable && cursorSelectable.CursorSelected)
            {
                cursorSelectable.CursorSelected = false;
                cursorSelectable.OnCursorDeselect();
            }
        }

        private void OnSelectionPressKeyboard(InputActionButton obj)
        {
            if (_pressed != _selected) _pressed?.OnMenuDepress(false);
            _selected?.OnMenuPress(false);
            _pressed = _selected;
        }

        private void OnSelectionPressMouse(InputActionButton obj)
        {
            if (_pressed != _selected) _pressed?.OnMenuDepress(false);
            _selected?.OnMenuPress(true);
            _pressed = _selected;
        }

        private void OnPressedNext(InputActionButton obj)
        {
            ChangeSelectIndex(+1);
        }
        private void OnPressedPrev(InputActionButton obj)
        {
            ChangeSelectIndex(-1);
        }

        private void ChangeSelectIndex(int delta)
        {
            int targetIndex = Math.Mod(_selectedIndex + delta, _items.Count);
            DeselectItem(_selectedIndex);
            DeselectCursorItem(_selectedIndex);
            _selectedIndex = targetIndex;
            SelectItem(targetIndex);
            DeselectCursorItem(targetIndex);

        }
    }
}
