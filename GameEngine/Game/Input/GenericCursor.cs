using System;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.Input
{
    public class GenericCursor : Cursor
    {
        public enum TimeScaleType
        {
            None,
            DeltaTime,
            UnscaledDeltaTime
        }

        //private List<InputActionAxis2D> _overrides = new List<InputActionAxis2D>();
        private InputActionAxis2D _override;
        public float OverrideScale = 1f;

        public TimeScaleType OverrideTimeScaleMode = TimeScaleType.UnscaledDeltaTime;
        public bool UseMouse = true;

        protected override void UpdateCursorPosition(GamePlus _game)
        {
            var delta = Vector2.Zero;
            ;

            var viewportRect = _game.GraphicsDevice.Viewport.Bounds;

            Position.X = Math.Clamp(Position.X, viewportRect.Left, viewportRect.Right);
            Position.Y = Math.Clamp(Position.Y, viewportRect.Top, viewportRect.Bottom);

            // While we're inside, do some funky stuff.
            if (true || InBounds(_game, Position))
            {
                // TODO: If we decide for SURE that we won't be using a list delete this.
                foreach (var ov in new[] {_override})
                    if (ov != null && ov.Active)
                    {
                        var input = ov.Value;
                        switch (OverrideTimeScaleMode)
                        {
                            case TimeScaleType.None:
                                break;
                            case TimeScaleType.DeltaTime:
                                input *= _game.DeltaTime;
                                break;
                            case TimeScaleType.UnscaledDeltaTime:
                                input *= _game.UnscaledDeltaTime;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        // Up is negative here.
                        input.Y *= -1;
                        var axisDelta = input * OverrideScale;
                        // Slide along border if we're not in bounds.
                        if (InBounds(_game, Position + delta.X * Vector2.UnitX)) delta += axisDelta.X * Vector2.UnitX;
                        if (InBounds(_game, Position + delta.Y * Vector2.UnitY)) delta += axisDelta.Y * Vector2.UnitY;
                    }

                var mouseInBounds = InBounds(_game, RawInput.GetMousePosition());
                var mouseDelta = RawInput.GetMouseDelta();
                var mouseMoved = mouseDelta.LengthSquared() > 1;

                /*
                if (UseMouse && mouseInBounds)
                {
                    delta += mouseDelta;
                }
                */
                if (UseMouse && mouseInBounds && mouseMoved)
                {
                    Position = RawInput.GetMousePosition();
                    MovedLastFrame = true;
                }
                else
                {
                    Position += delta;

                    // Whether we were moving. Also check if we clicked something.
                    MovedLastFrame = delta.LengthSquared() > 1;
                }
            }
        }

        private bool InBounds(GamePlus _game, Vector2 pos)
        {
            return _game.GraphicsDevice.Viewport.Bounds.Contains(pos);
        }

        public void SetOverride(InputActionAxis2D action, float scale)
        {
            _override = action;
            OverrideScale = scale;
            //_overrides.Add(action);
        }

        public void ClearOverrides()
        {
            _override = null;
            //_overrides.Clear();
        }
    }
}