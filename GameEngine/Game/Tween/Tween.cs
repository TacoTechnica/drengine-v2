using System;

namespace GameEngine.Game.Tween
{
    /// <summary>
    /// CREDITS TO JEFFREY LANTERS
    /// https://github.com/elraccoone/unity-tweens
    /// For making his awesome lib open source
    /// </summary>
    /// <summary>
    /// This object actually does the tweening.
    /// </summary>
    public abstract class Tween<T> : ITween
    {
        private bool _finished = false;
        private bool _started;

        private Tweener _parent;

        private T _start;
        private T _end;
        private float _duration;
        private Func<float, T> _tweenFunction;
        private Action<T> _onTween;

        private Func<float, float> _customUserFunction;
        private float _delay;
        private bool _useUnscaledTime;

        private bool _useOvershooting;
        private float _overshoot = 0;

        private Action _onComplete = null;
        private Action _onCancel = null;

        private float _startTime;

        private EaseType _ease;

        internal Tween(Tweener parent, T start, T end, float duration, Action<T> onTween, Func<float, T> tweenFunction)
        {
            _parent = parent;
            _start = start;
            _end = end;
            _duration = duration;
            _tweenFunction = tweenFunction;
            _onTween = onTween;

            _delay = 0;
            _useUnscaledTime = false;
            _ease = EaseType.Linear;

            _started = false;
            _finished = false;

            parent.AddTween(this);
        }

        // If false, we are done.
        public bool RunUpdate()
        {
            if (_finished) return false;

            if (!_started)
            {
                _started = true;
                RunStart();
            }

            float timePassed = GetTime() - _startTime;
            if (timePassed > _delay)
            {
                timePassed -= _delay;

                float progress = Math.Clamp01(timePassed / _duration);

                if (_ease == EaseType.Custom)
                {
                    progress = _customUserFunction.Invoke(progress);
                }
                else
                {
                    float prev = progress;
                    progress = Interpolation.Apply(_ease, progress);
                }

                // We might want to overshoot, so damp above 1 a little.
                if (_useOvershooting)
                {
                    if (progress > 1)
                    {
                        progress = progress - (progress - 1) / (_overshoot + 1);
                    }
                }
                else
                {
                    progress = Math.Clamp01(progress);
                }

                T value = _tweenFunction(progress);
                _onTween.Invoke(value);

                Update(value);

                if (timePassed > _duration)
                {
                    Finish();
                }
            }

            return !_finished;
        }

        private void RunStart()
        {
            _startTime = GetTime();
            Start();
        }

        protected void Finish()
        {
            _finished = true;
            _onComplete?.Invoke();
            _onComplete = null;
            _onCancel = null;
        }

        public void Cancel()
        {
            _finished = true;
            _onCancel?.Invoke();
            _onCancel = null;
            _onComplete = null;
        }

        public Tween<T> Immediate()
        {
            T value = _tweenFunction(0);
            _onTween.Invoke(value);
            Update(value);

            return this;
        }

        public Tween<T> SetFrom(T start)
        {
            _start = start;
            return this;
        }

        public Tween<T> SetOnComplete(Action onComplete)
        {
            _onComplete = onComplete;
            return this;
        }

        public Tween<T> SetOnCancel(Action onCancel)
        {
            _onCancel = onCancel;
            return this;
        }

        public Tween<T> SetDelay(float delay)
        {
            _delay = delay;
            return this;
        }

        public Tween<T> SetDuration(float time)
        {
            _duration = time;
            return this;
        }

        public Tween<T> SetUseUnscaledTime(bool use = true)
        {
            _useUnscaledTime = use;
            return this;
        }

        public Tween<T> SetEase(EaseType ease)
        {
            _ease = ease;
            return this;
        }

        public Tween<T> SetEaseCustom(Func<float, float> customFunction)
        {
            _customUserFunction = customFunction;
            _ease = EaseType.Custom;
            return this;
        }

        public Tween<T> SetOvershoot(float overshoot)
        {
            _useOvershooting = true;
            _overshoot = overshoot;
            return this;
        }

        private float GetTime()
        {
            if (_useUnscaledTime)
            {
                return _parent.Game.UnscaledTime;
            }
            return _parent.Game.Time;
        }

        #region Easing Shorthands

        /// Sets the ease for this tween to Linear.
        public Tween<T> SetEaseLinear () {
          _ease = EaseType.Linear;
          return this;
        }

        /// Sets the ease for this tween to Sine In.
        public Tween<T> SetEaseSineIn () {
          _ease = EaseType.SineIn;
          return this;
        }

        /// Sets the ease for this tween to Sine Out.
        public Tween<T> SetEaseSineOut () {
          _ease = EaseType.SineOut;
          return this;
        }

        /// Sets the ease for this tween to Sine In Out.
        public Tween<T> SetEaseSineInOut () {
          _ease = EaseType.SineInOut;
          return this;
        }

        /// Sets the ease for this tween to Quad In.
        public Tween<T> SetEaseQuadIn () {
          _ease = EaseType.QuadIn;
          return this;
        }

        /// Sets the ease for this tween to Quad Out.
        public Tween<T> SetEaseQuadOut () {
          _ease = EaseType.QuadOut;
          return this;
        }

        /// Sets the ease for this tween to Quad In Out.
        public Tween<T> SetEaseQuadInOut () {
          _ease = EaseType.QuadInOut;
          return this;
        }

        /// Sets the ease for this tween to Cubic In.
        public Tween<T> SetEaseCubicIn () {
          _ease = EaseType.CubicIn;
          return this;
        }

        /// Sets the ease for this tween to Cubic Out.
        public Tween<T> SetEaseCubicOut () {
          _ease = EaseType.CubicOut;
          return this;
        }

        /// Sets the ease for this tween to Cubic In Out.
        public Tween<T> SetEaseCubicInOut () {
          _ease = EaseType.CubicInOut;
          return this;
        }

        /// Sets the ease for this tween to Quart In.
        public Tween<T> SetEaseQuartIn () {
          _ease = EaseType.QuartIn;
          return this;
        }

        /// Sets the ease for this tween to Quart Out.
        public Tween<T> SetEaseQuartOut () {
          _ease = EaseType.QuartOut;
          return this;
        }

        /// Sets the ease for this tween to Quart In Out.
        public Tween<T> SetEaseQuartInOut () {
          _ease = EaseType.QuartInOut;
          return this;
        }

        /// Sets the ease for this tween to Quint In.
        public Tween<T> SetEaseQuintIn () {
          _ease = EaseType.QuintIn;
          return this;
        }

        /// Sets the ease for this tween to Quint Out.
        public Tween<T> SetEaseQuintOut () {
          _ease = EaseType.QuintOut;
          return this;
        }

        /// Sets the ease for this tween to Quint In Out.
        public Tween<T> SetEaseQuintInOut () {
          _ease = EaseType.QuintInOut;
          return this;
        }

        /// Sets the ease for this tween to Expo In.
        public Tween<T> SetEaseExpoIn () {
          _ease = EaseType.ExpoIn;
          return this;
        }

        /// Sets the ease for this tween to Expo Out.
        public Tween<T> SetEaseExpoOut () {
          _ease = EaseType.ExpoOut;
          return this;
        }

        /// Sets the ease for this tween to Expo In Out.
        public Tween<T> SetEaseExpoInOut () {
          _ease = EaseType.ExpoInOut;
          return this;
        }

        /// Sets the ease for this tween to Circ In.
        public Tween<T> SetEaseCircIn () {
          _ease = EaseType.CircIn;
          return this;
        }

        /// Sets the ease for this tween to Circ Out.
        public Tween<T> SetEaseCircOut () {
          _ease = EaseType.CircOut;
          return this;
        }

        /// Sets the ease for this tween to Circ In Out.
        public Tween<T> SetEaseCircInOut () {
          _ease = EaseType.CircInOut;
          return this;
        }

        /// Sets the ease for this tween to Back In.
        public Tween<T> SetEaseBackIn () {
          _ease = EaseType.BackIn;
          return this;
        }

        /// Sets the ease for this tween to Back Out.
        public Tween<T> SetEaseBackOut () {
          _ease = EaseType.BackOut;
          return this;
        }

        /// Sets the ease for this tween to Back In Out.
        public Tween<T> SetEaseBackInOut () {
          _ease = EaseType.BackInOut;
          return this;
        }

        /// Sets the ease for this tween to Elastic In.
        public Tween<T> SetEaseElasticIn () {
          _ease = EaseType.ElasticIn;
          return this;
        }

        /// Sets the ease for this tween to Elastic Out.
        public Tween<T> SetEaseElasticOut () {
          _ease = EaseType.ElasticOut;
          return this;
        }

        /// Sets the ease for this tween to Elastic In Out.
        public Tween<T> SetEaseElasticInOut () {
          _ease = EaseType.ElasticInOut;
          return this;
        }

        /// Sets the ease for this tween to Bounce In.
        public Tween<T> SetEaseBounceIn () {
          _ease = EaseType.BounceIn;
          return this;
        }

        /// Sets the ease for this tween to Bounce Out.
        public Tween<T> SetEaseBounceOut () {
          _ease = EaseType.BounceOut;
          return this;
        }

        /// Sets the ease for this tween to Bounce In Out.
        public Tween<T> SetEaseBounceInOut () {
          _ease = EaseType.BounceInOut;
          return this;
        }

        #endregion

        protected virtual void Start() {}
        protected virtual void Update(T target) {}
    }
}
