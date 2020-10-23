using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game.Input
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
        Y
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

    public class RawInput
    {

        private static KeyboardState _currKeyboardState;
        private static KeyboardState _prevKeyboardState;

        private static MouseState _currMouseState;
        private static MouseState _prevMouseState;

        private static GamePadState _currGamepadState;
        private static GamePadState _prevGamepadState;

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
            Mouse.SetPosition((int)pos.X, (int)pos.Y);
            _currMouseState = Mouse.GetState();
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

        // MUST be called at start of game loop.
        public static void UpdateState()
        {
            _prevKeyboardState = _currKeyboardState;
            _currKeyboardState = Keyboard.GetState();
            _prevMouseState = _currMouseState;
            _currMouseState = Mouse.GetState();
            _prevGamepadState = _currGamepadState;
            _currGamepadState = GamePad.GetState(PlayerIndex.One);
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
