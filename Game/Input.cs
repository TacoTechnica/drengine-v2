using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using YamlDotNet.Core.Tokens;

namespace DREngine.Game
{

    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public class Input
    {

        private static KeyboardState _currState;
        private static KeyboardState _prevState;

        private static MouseState _currMouseState;
        private static MouseState _prevMouseState;

        public static bool KeyPressing(Keys k)
        {
            return _currState.IsKeyDown(k);
        }

        public static bool KeyPressed(Keys k)
        {
            return _currState.IsKeyDown(k) && !_prevState.IsKeyDown(k);
        }

        public static bool KeyReleased(Keys k)
        {
            return !_currState.IsKeyDown(k) && _prevState.IsKeyDown(k);
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
            return Mouse.GetState().Position.ToVector2();
        }


        // MUST be called at start of game loop.
        public static void UpdateState()
        {
            _prevState = _currState;
            _currState = Keyboard.GetState();
            _prevMouseState = _currMouseState;
            _currMouseState = Mouse.GetState();
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
