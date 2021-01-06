using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.Tween
{
    /// <summary>
    ///     CREDITS TO JEFFREY LANTERS
    ///     https://github.com/elraccoone/unity-tweens
    ///     For making his awesome lib open source
    /// </summary>
    /// <summary>
    ///     Represents an object that can update and do tweens. That's it.
    /// </summary>
    public class Tweener
    {
        private readonly List<ITween> _toDelete = new List<ITween>(5);

        // TODO: Add Pause/Unpause functionality that will actually pause our own time.

        private readonly List<ITween> _tweens = new List<ITween>();

        public Tweener(GamePlus game)
        {
            Game = game;
        }

        public GamePlus Game { get; }

        public void RunUpdate()
        {
            _toDelete.Clear();
            foreach (var t in _tweens)
                if (!t.RunUpdate())
                    _toDelete.Add(t);

            foreach (var t in _toDelete) _tweens.Remove(t);
        }

        internal void AddTween(ITween t)
        {
            _tweens.Add(t);
        }

        #region Tween Handling Functions

        public Tween<float> TweenValue(float start, float end, Action<float> onTween, float duration)
        {
            return new TweenFloat(this, start, end, duration, onTween);
        }

        public Tween<Vector3> TweenValue(Vector3 start, Vector3 end, Action<Vector3> onTween, float duration)
        {
            return new TweenVector3(this, start, end, duration, onTween);
        }

        public Tween<Vector2> TweenValue(Vector2 start, Vector2 end, Action<Vector2> onTween, float duration)
        {
            return new TweenVector2(this, start, end, duration, onTween);
        }

        public Tween<Quaternion> TweenValue(Quaternion start, Quaternion end, Action<Quaternion> onTween,
            float duration)
        {
            return new TweenQuaternion(this, start, end, duration, onTween);
        }

        public Tween<Color> TweenValue(Color start, Color end, Action<Color> onTween, float duration)
        {
            return new TweenColor(this, start, end, duration, onTween);
        }

        public void CancelAll()
        {
            foreach (var t in _tweens) t.Cancel();
            _tweens.Clear();
        }

        #endregion
    }
}