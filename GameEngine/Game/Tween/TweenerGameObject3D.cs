using Microsoft.Xna.Framework;

namespace GameEngine.Game.Tween
{
    /// <summary>
    /// A tweener with extra functions specific to a gameobject3D
    /// </summary>
    public class TweenerGameObject3D : Tweener
    {
        private GameObjectRender3D _object;

        public TweenerGameObject3D(GamePlus game, GameObjectRender3D obj) : base(game)
        {
            _object = obj;
        }

        #region GameObject3D Specific tween functions

        public Tween<Vector3> TweenPosition(Vector3 start, Vector3 end, float duration)
        {
            return TweenValue(start, end, val =>
            {
                _object.Transform.Position = val;
            }, duration);
        }
        public Tween<Vector3> TweenPosition(Vector3 end, float duration)
        {
            return TweenPosition(_object.Transform.Position, end, duration);
        }

        public Tween<float> TweenPositionX(float start, float end, float duration)
        {
            return TweenValue(start, end, val =>
            {
                var copy = _object.Transform.Position;
                copy.X = val;
                _object.Transform.Position = copy;
            }, duration);
        }
        public Tween<float> TweenPositionY(float start, float end, float duration)
        {
            return TweenValue(start, end, val =>
            {
                var copy = _object.Transform.Position;
                copy.Y = val;
                _object.Transform.Position = copy;
            }, duration);
        }
        public Tween<float> TweenPositionZ(float start, float end, float duration)
        {
            return TweenValue(start, end, val =>
            {
                var copy = _object.Transform.Position;
                copy.Z = val;
                _object.Transform.Position = copy;
            }, duration);
        }

        public Tween<float> TweenPositionX(float end, float duration)
        {
            return TweenPositionX(_object.Transform.Position.X, end, duration);
        }
        public Tween<float> TweenPositionY(float end, float duration)
        {
            return TweenPositionY(_object.Transform.Position.Y, end, duration);
        }
        public Tween<float> TweenPositionZ(float end, float duration)
        {
            return TweenPositionZ(_object.Transform.Position.Z, end, duration);
        }

        public Tween<Quaternion> TweenRotation(Quaternion start, Quaternion end, float duration)
        {
            return TweenValue(start, end, val =>
            {
                _object.Transform.Rotation = val;
            }, duration);
        }
        public Tween<Quaternion> TweenRotation(Quaternion end, float duration)
        {
            return TweenRotation(_object.Transform.Rotation, end, duration);
        }

        #endregion
    }
}
