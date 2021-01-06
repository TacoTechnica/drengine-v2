using System;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.Tween
{
    /// <summary>
    ///     Represents an object that can do tweens, attached to a UI element.
    /// </summary>
    public class TweenerUI : Tweener
    {
        private readonly UIComponentBase _ui;

        public TweenerUI(GamePlus game, UIComponentBase ui) : base(game)
        {
            _ui = ui;
        }

        #region UIComponent Specific tween functions

        public Tween<Vector3> TweenPosition(Vector3 start, Vector3 end, float duration)
        {
            return TweenValue(start, end, val => { _ui.LocalTransform.Position = val; }, duration);
        }

        public Tween<Vector3> TweenPosition(Vector3 end, float duration)
        {
            return TweenPosition(_ui.LocalTransform.Position, end, duration);
        }

        public Tween<Vector3> TweenPositionDelta(Vector3 delta, float duration)
        {
            return TweenPosition(delta, Vector3.Zero, duration);
        }

        public Tween<float> TweenPositionX(float start, float end, float duration)
        {
            return TweenValue(start, end, val =>
            {
                var copy = _ui.LocalTransform.Position;
                copy.X = val;
                _ui.LocalTransform.Position = copy;
            }, duration);
        }

        public Tween<float> TweenPositionY(float start, float end, float duration)
        {
            return TweenValue(start, end, val =>
            {
                var copy = _ui.LocalTransform.Position;
                copy.Y = val;
                _ui.LocalTransform.Position = copy;
            }, duration);
        }

        public Tween<float> TweenPositionZ(float start, float end, float duration)
        {
            return TweenValue(start, end, val =>
            {
                var copy = _ui.LocalTransform.Position;
                copy.Z = val;
                _ui.LocalTransform.Position = copy;
            }, duration);
        }

        public Tween<float> TweenPositionX(float end, float duration)
        {
            return TweenPositionX(_ui.LocalTransform.Position.X, end, duration);
        }

        public Tween<float> TweenPositionY(float end, float duration)
        {
            return TweenPositionY(_ui.LocalTransform.Position.Y, end, duration);
        }

        public Tween<float> TweenPositionZ(float end, float duration)
        {
            return TweenPositionZ(_ui.LocalTransform.Position.Z, end, duration);
        }

        public Tween<float> TweenPositionXDelta(float delta, float duration)
        {
            return TweenPositionX(delta, 0, duration);
        }

        public Tween<float> TweenPositionYDelta(float delta, float duration)
        {
            return TweenPositionY(delta, 0, duration);
        }

        public Tween<float> TweenPositionZDelta(float delta, float duration)
        {
            return TweenPositionZ(delta, 0, duration);
        }

        public Tween<Quaternion> TweenRotation(Quaternion start, Quaternion end, float duration)
        {
            return TweenValue(start, end, val => { _ui.LocalTransform.Rotation = val; }, duration);
        }

        public Tween<Quaternion> TweenRotation(Quaternion end, float duration)
        {
            return TweenRotation(_ui.LocalTransform.Rotation, end, duration);
        }

        public Tween<Quaternion> TweenRotationDelta(Quaternion delta, float duration)
        {
            return TweenRotation(delta, Quaternion.Identity, duration);
        }

        public Tween<Quaternion> TweenRotationEulerDelta(float deltaPitch, float deltaYaw, float deltaRoll,
            float duration)
        {
            return TweenRotation(Math.FromEuler(deltaPitch, deltaYaw, deltaRoll), Quaternion.Identity, duration);
        }

        public Tween<Quaternion> TweenRotationPitchDelta(float delta, float duration)
        {
            return TweenRotationEulerDelta(delta, 0, 0, duration);
        }

        public Tween<Quaternion> TweenRotationYawDelta(float delta, float duration)
        {
            return TweenRotationEulerDelta(0, delta, 0, duration);
        }

        public Tween<Quaternion> TweenRotationRollDelta(float delta, float duration)
        {
            return TweenRotationEulerDelta(0, 0, delta, duration);
        }

        #endregion

        #region Specific Tweens

        public Tween<float> TweenBackgroundColor(Color color0, Color color1, Color color2, Color color3, float duration)
        {
            if (_ui is UIColoredRect rect)
            {
                Color c0 = rect.Color0,
                    c1 = rect.Color1,
                    c2 = rect.Color2,
                    c3 = rect.Color3;
                return TweenValue(0f, 1f, progress =>
                {
                    //Debug.Log($"PROG: {progress}");
                    rect.Color0 = Color.Lerp(c0, color0, progress);
                    rect.Color1 = Color.Lerp(c1, color1, progress);
                    rect.Color2 = Color.Lerp(c2, color2, progress);
                    rect.Color3 = Color.Lerp(c3, color3, progress);
                }, duration);
            }

            throw new InvalidOperationException(
                $"{_ui} is not a {typeof(UIColoredRect)}. Please don't call this tween function!");
        }

        public Tween<Color> TweenTextColor(Color color, float duration)
        {
            if (_ui is UIText text)
                return TweenValue(text.Color, color, currentColor => { text.Color = color; }, duration);
            throw new InvalidOperationException(
                $"{_ui} is not a {typeof(UIText)}. Please don't call this tween function!");
        }

        #endregion
    }
}