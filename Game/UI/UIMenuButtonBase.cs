using System;

namespace DREngine.Game.UI
{
    public abstract class UIMenuButtonBase : UiComponent, ICursorSelectable, IMenuItem
    {

        public Action Pressed;

        public UIMenuButtonBase(GamePlus game, UiComponent parent = null) : base(game, parent)
        {
            // Do nothing for now.
        }

        #region Interface

        public IMenu ParentMenu { get; set; }
        public bool CursorSelected { get; set; }
        public bool MenuSelected { get; set; }
        public bool __ChildWasSelected { get; set; }
        public bool ChildrenSelectFirst { get; set; }

        public void OnCursorSelect()
        {
        }

        public void OnCursorDeselect()
        {
        }

        public void OnMenuSelect()
        {
            OnSelectVisual();
        }

        public void OnMenuDeselect()
        {
            OnDeselectVisual();
        }

        public void OnMenuPress(bool mouse)
        {
            if (mouse && !CursorSelected) return;
            Pressed?.Invoke();
            OnPressVisual();
        }

        #endregion

        protected abstract void OnSelectVisual();
        protected abstract void OnDeselectVisual();
        protected abstract void OnPressVisual();


    }
}
