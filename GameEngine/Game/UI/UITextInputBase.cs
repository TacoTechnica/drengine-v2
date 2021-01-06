using System;
using GameEngine.Game.Input;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Game.UI
{
    public abstract class UITextInputBase : UIMenuButtonBase
    {
        public Action<string> Submitted;

        public UITextInputBase(GamePlus game, UIComponent parent = null) : base(game, parent)
        {
            RawInput.OnKeysPressed += OnInput;
            Selected = false;
        }

        public abstract string Text { get; set; }

        public bool Selected { get; private set; }

        ~UITextInputBase()
        {
            // ReSharper disable once DelegateSubtraction
            RawInput.OnKeysPressed -= OnInput;
        }

        public void Select()
        {
            Selected = true;
        }

        public void Deselect()
        {
            Selected = false;
        }

        private void OnInput(Keys[] obj)
        {
            if (!Active) return;
            if (!Selected) return;
            // TODO: Caps lock?
            var shift = RawInput.KeyPressing(Keys.LeftShift) || RawInput.KeyPressing(Keys.RightShift);
            var ctrl = RawInput.KeyPressing(Keys.LeftControl) || RawInput.KeyPressing(Keys.RightControl);
            // We got keys boys
            foreach (var key in obj)
                // Check for special keys
                switch (key)
                {
                    case Keys.LeftShift:
                    case Keys.RightShift:
                    case Keys.RightControl:
                    case Keys.LeftControl:
                        continue;
                    case Keys.Enter:
                        Submitted?.Invoke(Text);
                        OnSubmit();
                        break;
                    case Keys.Back:
                        OnBackspace(ctrl);
                        break;
                    case Keys.Left:
                        OnLeft(ctrl, shift);
                        break;
                    case Keys.Right:
                        OnRight(ctrl, shift);
                        break;
                    case Keys.Tab:
                        OnTab();
                        break;
                    case Keys.Home:
                        OnHome(shift);
                        break;
                    case Keys.End:
                        OnEnd(shift);
                        break;
                    default:
                        var c = key.ToChar(shift);
                        if (c != (char) 0)
                        {
                            if (ctrl)
                                OnControlInput(c);
                            else
                                OnCharacterInput(c);
                        }

                        break;
                }
        }


        protected abstract void OnCharacterInput(char c);
        protected abstract void OnControlInput(char c);
        protected abstract void OnSubmit();
        protected abstract void OnBackspace(bool ctrl);
        protected abstract void OnLeft(bool ctrl, bool shift);
        protected abstract void OnRight(bool ctrl, bool shift);
        protected abstract void OnTab();
        protected abstract void OnHome(bool shift);
        protected abstract void OnEnd(bool shift);
    }
}