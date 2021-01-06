using System;
using GameEngine.Game.Input;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.UI
{
    public class UISlider : UIMenuButtonBase
    {
        public enum SliderDirection
        {
            LeftToRight,
            TopToBottom
        }

        private readonly UIComponent _background;

        private Rect _cachedBounds;
        private float _endValue;

        private readonly UISliderHandle _handle;

        private float _handleSizePercent;
        private float _slidePercent;

        private float _startValue;


        public UISlider(GamePlus game, SliderDirection direction = SliderDirection.TopToBottom,
            UISliderHandle handle = null, UIComponent background = null, UIComponent parent = null) : base(game, parent)
        {
            // Defaults for slider handle and background
            if (background == null)
                background = new UIColoredRect(game, Color.Lerp(Color.Firebrick, Color.Black, 0.5f));
            if (handle == null) handle = new UISliderHandleDefault(game);


            _handle = handle;
            _background = background;
            Direction = direction;


            background.WithLayout(Layout.FullscreenLayout());

            AddChild(background);
            AddChild(handle);

            HandleSizePercent = 0.1f;
            SlidePercent = 0f;
        }

        public SliderDirection Direction { get; }

        public float HandleSizePercent
        {
            get => _handleSizePercent;
            set
            {
                var target = Math.Clamp01(value);
                if (System.Math.Abs(target - _handleSizePercent) > 0.001f)
                {
                    _handleSizePercent = target;
                    UpdateHandleLayout();
                }

                _handleSizePercent = target;
            }
        }

        public float SlidePercent
        {
            get => _slidePercent;
            set
            {
                _slidePercent = Math.Clamp01(value);
                UpdateHandleLayout();
            }
        }

        // Control by value & range, not percentage.

        public float Value
        {
            get => StartValue + SlidePercent * (EndValue - StartValue);
            set => SlidePercent = EndValue - StartValue <= 0 ? 0 : (value - StartValue) / (EndValue - StartValue);
        }

        public float StartValue
        {
            get => _startValue;
            set
            {
                // Change our range and make our value stay the same.
                var prevVal = Value;
                _startValue = value;
                if (_startValue > _endValue) _startValue = _endValue;
                Value = prevVal;
            }
        }

        public float EndValue
        {
            get => _endValue;
            set
            {
                // Change our range and make our value stay the same.
                var prevVal = Value;
                _endValue = value;
                if (_endValue < _startValue) _endValue = _startValue;
                Value = prevVal;
            }
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            _cachedBounds = targetRect;
            // default background
            if (_background == null) screen.DrawRect(targetRect, Color.SlateGray);
        }

        protected override void OnSelectVisual()
        {
            // Selection maybe?
        }

        protected override void OnDeselectVisual()
        {
            // Selection maybe?
        }

        protected override void OnPressVisual()
        {
            // CONVERT MOUSE POSITION -> PERCENT, THEN SET PERCENT
            // If we're on the handle, don't force anything!
            if (_handle.CursorSelected) return;

            // Now, we must move our percent to where our mouse is.
            var mousePos = _game.CurrentCursor.Position;
            float targetPos;
            float minRectPos;
            float maxRectPos;
            switch (Direction)
            {
                case SliderDirection.LeftToRight:
                    targetPos = mousePos.X;
                    minRectPos = _cachedBounds.Min.X;
                    maxRectPos = _cachedBounds.Max.X;
                    break;
                case SliderDirection.TopToBottom:
                    targetPos = mousePos.Y;
                    minRectPos = _cachedBounds.Min.Y;
                    maxRectPos = _cachedBounds.Max.Y;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var minPos = minRectPos;
            var maxPos = maxRectPos - (maxRectPos - minRectPos) * HandleSizePercent;
            SlidePercent = (targetPos - minPos) / (maxPos - minPos) - HandleSizePercent / 2f;

            // If we move the mouse fast we'll go outside of dragging range which will make the slider stutter.
            _handle.ForceDrag();
        }

        protected override void OnDepressVisual()
        {
            // Do nothing.
        }

        private void UpdateHandleLayout()
        {
            float startPercent = (1 - HandleSizePercent) * SlidePercent,
                endPercent = startPercent + HandleSizePercent;
            switch (Direction)
            {
                case SliderDirection.LeftToRight:
                    _handle.WithLayout(new Layout
                    {
                        AnchorMin = Vector2.UnitX * startPercent,
                        AnchorMax = Vector2.UnitX * endPercent + Vector2.UnitY
                    });
                    break;
                case SliderDirection.TopToBottom:
                    _handle.WithLayout(new Layout
                    {
                        AnchorMin = Vector2.UnitY * startPercent,
                        AnchorMax = Vector2.UnitX + Vector2.UnitY * endPercent
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void ChangeSliderPosition(Vector2 mouseDelta)
        {
            float deltaPos;
            float minRectPos;
            float maxRectPos;
            switch (Direction)
            {
                case SliderDirection.LeftToRight:
                    deltaPos = mouseDelta.X;
                    minRectPos = _cachedBounds.Min.X;
                    maxRectPos = _cachedBounds.Max.X;
                    break;
                case SliderDirection.TopToBottom:
                    deltaPos = mouseDelta.Y;
                    minRectPos = _cachedBounds.Min.Y;
                    maxRectPos = _cachedBounds.Max.Y;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var minPos = minRectPos;
            var maxPos = maxRectPos - (maxRectPos - minRectPos) * HandleSizePercent;
            var deltaPercent = deltaPos / (maxPos - minPos);
            SlidePercent += deltaPercent;
        }

        public abstract class UISliderHandle : UIMenuButtonBase
        {
            private bool _dragging;
            private Vector2 _dragMousePrev;

            public UISliderHandle(GamePlus game, UISlider parent = null) : base(game, parent)
            {
            }

            private UISlider _parentSlider => (UISlider) _parent;

            private Vector2 mousePos => _game.CurrentCursor.Position;
            private bool mousePressing => RawInput.MousePressing(MouseButton.Left);

            protected override void Draw(UIScreen screen, Rect targetRect)
            {
                if (screen.NeedToUpdateControl)
                {
                    if (mousePressing)
                    {
                        if (CursorSelected && !_dragging)
                        {
                            _dragging = true;
                            _dragMousePrev = mousePos;
                        }
                    }
                    else // mouse NOT pressing
                    {
                        if (_dragging)
                            // Released.
                            _dragging = false;
                    }

                    if (_dragging)
                    {
                        _parentSlider?.ChangeSliderPosition(mousePos - _dragMousePrev);

                        _dragMousePrev = mousePos;
                    }
                }

                DrawHandle(screen, targetRect, _dragging);
            }

            protected override void OnSelectVisual()
            {
            }

            protected override void OnDeselectVisual()
            {
            }

            protected override void OnPressVisual()
            {
            }

            protected override void OnDepressVisual()
            {
            }

            internal void ForceDrag()
            {
                CursorSelected = true;
                //_dragging = true;
                //_dragMousePrev = mousePos;
            }

            protected abstract void DrawHandle(UIScreen screen, Rect targetRect, bool dragging);
        }

        protected class UISliderHandleDefault : UISliderHandle
        {
            public UISliderHandleDefault(GamePlus game, UISlider parent = null) : base(game, parent)
            {
            }

            protected override void DrawHandle(UIScreen screen, Rect targetRect, bool dragging)
            {
                screen.DrawRect(targetRect, Color.Bisque);
            }
        }
    }
}