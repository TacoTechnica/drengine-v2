using System;
using Microsoft.Xna.Framework;

// ReSharper disable ConvertToLambdaExpression

// There are multiple data types we want to tween, and this is our exhaustive list of all of them.

namespace GameEngine.Game.Tween
{
    /// CREDITS TO JEFFREY LANTERS
    /// https://github.com/elraccoone/unity-tweens
    /// For making his awesome lib open source
    public class TweenFloat : Tween<float>
    {
        public TweenFloat(Tweener parent, float start, float end, float duration, Action<float> onTween) : base(parent,
            start, end, duration, onTween, progress => { return start + (end - start) * progress; })
        {
        }
    }

    public class TweenVector3 : Tween<Vector3>
    {
        public TweenVector3(Tweener parent, Vector3 start, Vector3 end, float duration, Action<Vector3> onTween) : base(
            parent, start, end, duration, onTween, progress => { return start + (end - start) * progress; })
        {
        }
    }

    public class TweenVector2 : Tween<Vector2>
    {
        public TweenVector2(Tweener parent, Vector2 start, Vector2 end, float duration, Action<Vector2> onTween) : base(
            parent, start, end, duration, onTween, progress => { return start + (end - start) * progress; })
        {
        }
    }

    public class TweenQuaternion : Tween<Quaternion>
    {
        public TweenQuaternion(Tweener parent, Quaternion start, Quaternion end, float duration,
            Action<Quaternion> onTween) : base(parent, start, end, duration, onTween,
            progress => { return start + (end - start) * progress; })
        {
        }
    }

    public class TweenColor : Tween<Color>
    {
        public TweenColor(Tweener parent, Color start, Color end, float duration, Action<Color> onTween) : base(parent,
            start, end, duration, onTween,
            progress => { return Color.Lerp(start, end, progress); })
        {
        }
    }
}