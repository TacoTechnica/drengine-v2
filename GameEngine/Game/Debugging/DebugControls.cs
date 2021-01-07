using GameEngine.Game.Input;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Game.Debugging
{
    public class DebugControls : Controls
    {
        public InputActionButton ConsoleClose;
        public InputActionButton ConsoleOpen;
        public InputActionButton ConsoleSubmit;

        public DebugControls(GamePlus game) : base(game)
        {
            ConsoleOpen = new InputActionButton(this, Keys.OemTilde);
            ConsoleClose = new InputActionButton(this, Keys.Escape);
            ConsoleSubmit = new InputActionButton(this, Keys.Enter);
        }
    }
}