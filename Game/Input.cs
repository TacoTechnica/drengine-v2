using Microsoft.Xna.Framework.Input;
using YamlDotNet.Core.Tokens;

namespace DREngine.Game
{
    public class Input
    {

        private static KeyboardState _currState;
        private static KeyboardState _prevState;

        public static bool IsPressing(Keys k)
        {
            return _currState.IsKeyDown(k);
        }

        public static bool IsPressed(Keys k)
        {
            return _currState.IsKeyDown(k) && !_prevState.IsKeyDown(k);
        }

        public static bool IsReleased(Keys k)
        {
            return !_currState.IsKeyDown(k) && _prevState.IsKeyDown(k);
        }

        // MUST be called at start of game loop.
        public static void UpdateState()
        {
            _prevState = _currState;
            _currState = Keyboard.GetState();
        }
    }
}
