using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game.Input
{
    public class GenericCursor : Cursor
    {
        public bool UseMouse = true;

        //private List<InputActionAxis2D> _overrides = new List<InputActionAxis2D>();
        private InputActionAxis2D _override = null;
        public float OverrideScale = 1f;

        public enum TimeScaleType
        {
            None, DeltaTime, UnscaledDeltaTime
        }

        public TimeScaleType OverrideTimeScaleMode = TimeScaleType.UnscaledDeltaTime;

        protected override void UpdateCursorPosition(GamePlus _game)
        {
            // While we're inside, do some funky stuff.
            if (InBounds(_game, Position))
            {
                // TODO: If we decide for SURE that we won't be using a list delete this.
                foreach (InputActionAxis2D ov in new InputActionAxis2D[] {_override})
                {
                    if (ov != null && ov.Active)
                    {
                        Vector2 input = ov.Value;
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
                        Vector2 delta = input * OverrideScale;
                        // Slide along border if we're not in bounds.
                        if (InBounds(_game, Position + delta.X * Vector2.UnitX))
                        {
                            Position += delta.X * Vector2.UnitX;
                        }
                        if (InBounds(_game, Position + delta.Y * Vector2.UnitY))
                        {
                            Position += delta.Y * Vector2.UnitY;
                        }
                    }
                }
            }

            if (UseMouse)
            {
                Vector2 mouseDelta = RawInput.GetMouseDelta();
                Position += mouseDelta;
                RawInput.SetMousePos(Position);
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
