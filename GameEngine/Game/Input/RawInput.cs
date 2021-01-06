using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Game.Input
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public enum MouseAxis
    {
        X,
        Y,
        DX,
        DY
    }

    public enum GamepadAxis
    {
        LTrigger,
        RTrigger,
        LStickX,
        LStickY,
        RStickX,
        RStickY
    }

    public static class RawInput
    {
        private static KeyboardState _currKeyboardState;
        private static KeyboardState _prevKeyboardState;

        private static MouseState _currMouseState;
        private static MouseState _prevMouseState;

        private static GamePadState _currGamepadState;
        private static GamePadState _prevGamepadState;

        public static Action<Keys[]> OnKeysPressed;

        private static readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>();

        private static bool _mouseLocked;

        // Dang... I really didn't want to do this but for some situations it's required.
        private static GamePlus _game;

        public static void SetGame(GamePlus game)
        {
            _game = game;
        }

        public static bool KeyPressing(Keys k)
        {
            return _currKeyboardState.IsKeyDown(k);
        }

        public static bool KeyPressed(Keys k)
        {
            return _currKeyboardState.IsKeyDown(k) && !_prevKeyboardState.IsKeyDown(k);
        }

        public static bool KeyReleased(Keys k)
        {
            return !_currKeyboardState.IsKeyDown(k) && _prevKeyboardState.IsKeyDown(k);
        }

        public static bool MousePressing(MouseButton b)
        {
            return CheckMouseState(_currMouseState, b);
        }

        public static bool MousePressed(MouseButton b)
        {
            return CheckMouseState(_currMouseState, b) && !CheckMouseState(_prevMouseState, b);
        }

        public static bool MouseReleasd(MouseButton b)
        {
            return !CheckMouseState(_currMouseState, b) && CheckMouseState(_prevMouseState, b);
        }

        public static Vector2 GetMousePosition()
        {
            return _currMouseState.Position.ToVector2();
        }

        public static Vector2 GetMouseDelta()
        {
            return _currMouseState.Position.ToVector2() - _prevMouseState.Position.ToVector2();
        }

        public static void SetMousePos(Vector2 pos)
        {
            Mouse.SetPosition((int) pos.X, (int) pos.Y);
            _currMouseState = Mouse.GetState();
        }

        public static void SetMouseLock(bool enabled)
        {
            _mouseLocked = enabled;
            SetMouseVisibility(!enabled);
        }

        public static bool IsMouseLocked()
        {
            return _mouseLocked;
        }

        public static void SetMouseVisibility(bool visible)
        {
            _game.IsMouseVisible = visible;
        }

        public static bool GetMouseVisibility()
        {
            return _game.IsMouseVisible;
        }

        public static bool GamepadPressing(Buttons b)
        {
            return _currGamepadState.IsButtonDown(b);
        }

        public static bool GamepadPressed(Buttons b)
        {
            return _currGamepadState.IsButtonDown(b) && !_prevGamepadState.IsButtonDown(b);
        }

        public static bool GamepadReleased(Buttons b)
        {
            return !_currGamepadState.IsButtonDown(b) && _prevGamepadState.IsButtonDown(b);
        }

        public static Vector2 GetGamepadLS()
        {
            return _currGamepadState.ThumbSticks.Left;
        }

        public static Vector2 GetGamepadRS()
        {
            return _currGamepadState.ThumbSticks.Left;
        }

        public static float GetGamepadAxis(GamepadAxis axis)
        {
            switch (axis)
            {
                case GamepadAxis.LTrigger:
                    return _currGamepadState.Triggers.Left;
                case GamepadAxis.RTrigger:
                    return _currGamepadState.Triggers.Left;
                case GamepadAxis.LStickX:
                    return GetGamepadLS().X;
                case GamepadAxis.LStickY:
                    return GetGamepadLS().Y;
                case GamepadAxis.RStickX:
                    return GetGamepadRS().X;
                case GamepadAxis.RStickY:
                    return GetGamepadRS().Y;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        /// <summary>
        ///     Converts a key to a char.
        /// </summary>
        /// <param name="Key">They key to convert.</param>
        /// <param name="Shift">Whether or not shift is pressed.</param>
        /// <returns>The key in a char.</returns>
        public static char ToChar(this Keys Key, bool Shift = false)
        {
            /* It's the space key. */
            if (Key == Keys.Space) return ' ';

            var String = Key.ToString();

            /* It's a letter. */
            if (String.Length == 1)
            {
                var Character = char.Parse(String);
                var Byte = Convert.ToByte(Character);

                if (
                    Byte >= 65 && Byte <= 90 ||
                    Byte >= 97 && Byte <= 122
                )
                    return (!Shift ? Character.ToString().ToLower() : Character.ToString())[0];
            }

            /*
                 *
                 * The only issue is, if it's a symbol, how do I know which one to take if the user isn't using United States international?
                 * Anyways, thank you, for saving my time
                 * down here:
                 */

            #region Credits :  http: //roy-t.nl/2010/02/11/code-snippet-converting-keyboard-input-to-text-in-xna.html for saving my time.

            switch (Key)
            {
                case Keys.D0:
                    if (Shift)
                        return ')';
                    else
                        return '0';
                case Keys.D1:
                    if (Shift)
                        return '!';
                    else
                        return '1';
                case Keys.D2:
                    if (Shift)
                        return '@';
                    else
                        return '2';
                case Keys.D3:
                    if (Shift)
                        return '#';
                    else
                        return '3';
                case Keys.D4:
                    if (Shift)
                        return '$';
                    else
                        return '4';
                case Keys.D5:
                    if (Shift)
                        return '%';
                    else
                        return '5';
                case Keys.D6:
                    if (Shift)
                        return '^';
                    else
                        return '6';
                case Keys.D7:
                    if (Shift)
                        return '&';
                    else
                        return '7';
                case Keys.D8:
                    if (Shift)
                        return '*';
                    else
                        return '8';
                case Keys.D9:
                    if (Shift)
                        return '(';
                    else
                        return '9';

                case Keys.NumPad0: return '0';
                case Keys.NumPad1: return '1';
                case Keys.NumPad2: return '2';
                case Keys.NumPad3: return '3';
                case Keys.NumPad4: return '4';
                case Keys.NumPad5: return '5';
                case Keys.NumPad6: return '6';
                case Keys.NumPad7:
                    return '7';
                    ;
                case Keys.NumPad8: return '8';
                case Keys.NumPad9: return '9';

                case Keys.OemTilde:
                    if (Shift)
                        return '~';
                    else
                        return '`';
                case Keys.OemSemicolon:
                    if (Shift)
                        return ':';
                    else
                        return ';';
                case Keys.OemQuotes:
                    if (Shift)
                        return '"';
                    else
                        return '\'';
                case Keys.OemQuestion:
                    if (Shift)
                        return '?';
                    else
                        return '/';
                case Keys.OemPlus:
                    if (Shift)
                        return '+';
                    else
                        return '=';
                case Keys.OemPipe:
                    if (Shift)
                        return '|';
                    else
                        return '\\';
                case Keys.OemPeriod:
                    if (Shift)
                        return '>';
                    else
                        return '.';
                case Keys.OemOpenBrackets:
                    if (Shift)
                        return '{';
                    else
                        return '[';
                case Keys.OemCloseBrackets:
                    if (Shift)
                        return '}';
                    else
                        return ']';
                case Keys.OemMinus:
                    if (Shift)
                        return '_';
                    else
                        return '-';
                case Keys.OemComma:
                    if (Shift)
                        return '<';
                    else
                        return ',';
            }

            #endregion

            return (char) 0;
        }

        // MUST be called at start of game loop.
        public static void UpdateState()
        {
            _prevKeyboardState = _currKeyboardState;
            _currKeyboardState = Keyboard.GetState();
            _prevMouseState = _currMouseState;
            _currMouseState = Mouse.GetState();
            _prevGamepadState = _currGamepadState;
            _currGamepadState = GamePad.GetState(PlayerIndex.One);

            if (_mouseLocked)
            {
                var r = _game.GraphicsDevice.Viewport.Bounds;
                // We must make sure our prev state has a centered mouse.
                var center = r.Center; //new Vector2(, _game.GraphicsDevice.Viewport.Height / 2);
                Mouse.SetPosition(center.X, center.Y);
                _prevMouseState = Mouse.GetState();
            }

            var currentKeys = _currKeyboardState.GetPressedKeys();
            if (currentKeys.Length != 0)
            {
                var newKeys = new List<Keys>();
                foreach (var key in currentKeys)
                    if (!_pressedKeys.Contains(key))
                        // This is new!
                        newKeys.Add(key);
                // Now we only have new keys here.
                OnKeysPressed?.Invoke(newKeys.ToArray());
                _pressedKeys.Clear();
                // Pressed keys now contains all keys pressed
                foreach (var key in currentKeys) _pressedKeys.Add(key);
            }
            else
            {
                _pressedKeys.Clear();
            }
        }

        private static bool CheckMouseState(MouseState m, MouseButton b)
        {
            switch (b)
            {
                case MouseButton.Left:
                    return m.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return m.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return m.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }
    }
}