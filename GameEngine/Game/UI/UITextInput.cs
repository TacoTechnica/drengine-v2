using System;
using System.Linq;
using GameEngine.Game.Resources;
using Gtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TextCopy;

namespace GameEngine.Game.UI
{
    public class UITextInput : UITextInputBase
    {

        private Font _font;
        private UIText _text;

        public int SelectBegin { get; private set; } = -1;
        public int SelectEnd { get; private set; } = -1;
        private bool Selecting => SelectBegin != -1 && SelectEnd != -1;

        private UIColoredRect _selectRect;

        public Color SelectColor;

        public override string Text
        {
            get => _text.Text;
            set
            {
                _text.Text = value;
                SetCursor(_cursorPos);
            }
        }

        private int _cursorPos;

        public UITextInput(GamePlus game, Font font, Color color, UIComponent parent = null) : base(game, parent)
        {
            _font = font;

            SelectColor = new Color(255 - color.R, 255 - color.G, 255 - color.B, color.A);

            // Select rect goes beneath text
            _selectRect = (UIColoredRect) new UIColoredRect(game, SelectColor, false, this)
                .WithLayout(Layout.CornerLayout(Layout.TopLeft, 0, 0));

            // Text
            _text = new UIText(game, font, "", this);
            _text.Color = color;

        }
        public UITextInput(GamePlus game, Font font, UIComponent parent = null) : this(game, font, Color.Black, parent) {}

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            if (!Selected) return;
            // TODO: Make cursor blink (use a function of time)
            string sub = Text.Substring(0, _cursorPos);
            try
            {
                Vector2 delta = _text.GetSize(sub);
                Vector2 cursorSize = new Vector2(2, _text.Font.SpriteFont.LineSpacing);
                screen.DrawRect(targetRect.Position + delta - cursorSize, cursorSize, _text.Color);
            }
            catch (ArgumentException)
            {
                Debug.LogError($"INVALID TEXT: \"{sub}\"");
            }
        }

        protected override void OnSelectVisual()
        {
            // We don't care
        }

        protected override void OnDeselectVisual()
        {
            // We don't care
        }

        protected override void OnPressVisual()
        {
            Select();
        }

        protected override void OnDepressVisual()
        {
            Deselect();
        }

        protected override void OnCharacterInput(char c)
        {
            if (Selecting)
            {
                SetCursor(SelectBegin);
                DeleteSelected();
            }
            // Add to text
            InsertText(c.ToString());
            SetCursor(_cursorPos + 1);
        }

        protected override void OnControlInput(char c)
        {
            // Ctrl+A, Ctrl+C, Ctrl+V
            switch (c)
            {
                case 'a':
                    SelectBegin = 0;
                    SelectEnd = Text.Length;
                    UpdateSelectVisual();
                    break;
                case 'v':
                    if (Selecting)
                    {
                        SetCursor(SelectBegin);
                        DeleteSelected();
                    }

                    string toPaste = ClipboardService.GetText();
                    InsertText(toPaste);
                    SetCursor(_cursorPos + toPaste.Length);
                    break;
                case 'c':
                case 'x':
                    if (Selecting)
                    {
                        string toCopy = Text.Substring(SelectBegin, SelectEnd - SelectBegin);
                        ClipboardService.SetText(toCopy);
                        if (c == 'x')
                        {
                            SetCursor(SelectBegin);
                            DeleteSelected();
                        }
                    }
                    break;
            }
        }

        protected override void OnSubmit()
        {
            // Nothing for now...
        }

        protected override void OnBackspace(bool ctrl)
        {
            if (Selecting)
            {
                SetCursor(SelectBegin);
                DeleteSelected();
            }
            else
            {
                int count = 1;
                if (ctrl)
                {
                    int lastEndWord = GetPrevControlSplit();
                    if (lastEndWord == -1)
                    {
                        count = _cursorPos;
                    }
                    else
                    {
                        count = _cursorPos - lastEndWord;
                    }
                }

                int prevPos = _cursorPos;
                DeleteTextBack(count);
                SetCursor(prevPos - count); // - count);
            }
        }

        protected override void OnLeft(bool ctrl, bool shift)
        {
            int newPos = _cursorPos - 1;
            if (ctrl)
            {
                newPos = GetPrevControlSplit();
            }

            if (shift)
            {
                SelectChange(_cursorPos, newPos);
            }
            else
            {
                if (Selecting)
                {
                    newPos = SelectBegin;
                    RemoveSelection();
                }
            }

            SetCursor(newPos);
        }

        protected override void OnRight(bool ctrl, bool shift)
        {
            int newPos = _cursorPos + 1;
            if (ctrl)
            {
                newPos = GetNextControlSplit();
            }

            if (shift)
            {
                SelectChange(_cursorPos, newPos);
            }
            else
            {
                if (Selecting)
                {
                    newPos = SelectEnd + 1;
                    RemoveSelection();
                }

            }

            SetCursor(newPos);
        }

        protected override void OnTab()
        {
            InsertText("\t");
            SetCursor(_cursorPos + 1);
        }

        protected override void OnHome(bool shift)
        {
            if (shift)
            {
                SelectChange(_cursorPos, 0);
            }
            else
            {
                RemoveSelection();
            }
            SetCursor(0);
        }

        protected override void OnEnd(bool shift)
        {
            if (shift)
            {
                SelectChange(_cursorPos, Text.Length);
            }
            else
            {
                RemoveSelection();
            }
            SetCursor(Text.Length);
        }

        private void SetCursor(int pos)
        {
            _cursorPos = Math.Clamp(pos, 0, Text.Length);
        }

        private void InsertText(string text)
        {
            Text = Text.Substring(0, _cursorPos) + text + Text.Substring(_cursorPos);
        }

        private void DeleteTextBack(int count)
        {
            if (_cursorPos - count < 0)
            {
                Text = Text.Substring(_cursorPos);
            }
            else
            {
                Text = Text.Substring(0, _cursorPos - count) + Text.Substring(_cursorPos);
            }
        }

        private void SelectChange(int prevPosition, int newPosition)
        {
            if (!Selecting)
            {
                SelectBegin = Math.Min(prevPosition, newPosition);
                SelectEnd = Math.Max(prevPosition, newPosition);
            } else if (newPosition > SelectEnd)
            {
                // Push End
                SelectEnd = newPosition;
            }
            else if (newPosition < SelectBegin)
            {
                // Pull Begin
                SelectBegin = newPosition;
            }
            else if (SelectBegin < newPosition && newPosition < SelectEnd)
            {
                // We're within the range, so either pull End or push Begin
                if (prevPosition == SelectBegin)
                {
                    // Push beginning
                    SelectBegin = newPosition;
                }
                else if (prevPosition == SelectEnd)
                {
                    SelectEnd = newPosition;
                }
                else
                {
                    Debug.LogError($"Invalid selection range: [{SelectBegin}, {SelectEnd}] for transition from {prevPosition} => {newPosition}");
                    SelectBegin = -1;
                    SelectEnd = -1;
                }
            } else if (SelectBegin == prevPosition && SelectEnd == newPosition)
            {
                // We moved from the start to the end, we are now at the end.
                SelectBegin = SelectEnd;
            } else if (SelectBegin == newPosition && SelectEnd == prevPosition)
            {
                // We moved from the start to the end, we are now at the end.
                SelectEnd = SelectBegin;
            }


            SelectBegin = Math.Clamp(SelectBegin, 0, Text.Length);
            SelectEnd = Math.Clamp(SelectEnd, 0, Text.Length);

            UpdateSelectVisual();

            //Debug.Log($"[{SelectBegin}, {SelectEnd}]");

            //Debug.Log($"DELT: {startPos} => {endPos}");
        }

        private void UpdateSelectVisual()
        {
            Vector2 startPos = _text.TextMin + _text.GetSize(Text.Substring(0, SelectBegin)),
                endPos = _text.TextMin + _text.GetSize(Text.Substring(0, SelectEnd));//SelectBegin, SelectEnd - SelectBegin));
            startPos.Y -= _font.SpriteFont.LineSpacing;
            // TODO: Multi line support
            _selectRect.Layout.Margin = new Margin(startPos.X, startPos.Y,  -endPos.X, -endPos.Y);

        }

        private void DeleteSelected()
        {
            Text = Text.Substring(0, SelectBegin) + Text.Substring(SelectEnd);
            RemoveSelection();
        }

        private void RemoveSelection()
        {
            SelectBegin = -1;
            SelectEnd = -1;
            _selectRect.Layout.Margin = new Margin(0, 0, 0, 0);
        }

        private int GetPrevControlSplit()
        {
            int ind = Text.Substring(0, _cursorPos).LastIndexOfAny(new[] {' ', ',', '.', '_'});
            if (ind == -1) return 0;
            return ind;
        }

        private int GetNextControlSplit()
        {
            if (_cursorPos + 1 >= Text.Length) return Text.Length;
            int ind = Text.IndexOfAny(new[] {' ', ',', '.', '_'}, _cursorPos + 1);
            if (ind == -1)
            {
                return Text.Length;
            }
            return ind;
        }
    }
}
