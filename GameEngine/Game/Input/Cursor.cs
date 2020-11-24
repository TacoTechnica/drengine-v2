
using Microsoft.Xna.Framework;

namespace GameEngine.Game.Input
{
    /// <summary>
    ///     This abstracts away all cursor handling so that many devices can use a cursor.
    ///
    ///     For instance, a cursor may use both mouse and a joystick to move itself.
    /// </summary>
    public abstract class Cursor
    {
        public Vector2 Position;

        public bool MovedLastFrame { get; protected set; } = true;

        public void DoUpdate(GamePlus _game)
        {
            UpdateCursorPosition(_game);
        }

        protected abstract void UpdateCursorPosition(GamePlus _game);

    }
}
