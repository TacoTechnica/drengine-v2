using System;
using DREngine.Game.Input;
using Microsoft.Xna.Framework;

namespace DREngine.Game.UI
{
    public class UISlider : UIMenuButtonBase
    {

        private UISliderHandle _handle;
        private UIComponent _background;

        private float _handleSizePercent;
        private float _slidePercent;

        private Rect _cachedBounds;

        public enum SliderDirection
        {
            LeftToRight,
            TopToBottom
        }

        public SliderDirection Direction { get; private set; }

        public float HandleSizePercent
        {
            get => _handleSizePercent;
            set
            {
                float target = Math.Clamp01(value);
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
            set => SlidePercent = (EndValue - StartValue <= 0)? 0 : (value - StartValue) / (EndValue - StartValue);
        }

        private float _startValue = 0;
        private float _endValue = 0;

        public float StartValue
        {
            get => _startValue;
            set
            {
                // Change our range and make our value stay the same.
                float prevVal = Value;
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
                float prevVal = Value;
                _endValue = value;
                if (_endValue < _startValue) _endValue = _startValue;
                Value = prevVal;
            }
        }


        public UISlider(GamePlus game, SliderDirection direction = SliderDirection.TopToBottom, UISliderHandle handle = null, UIComponent background = null, UIComponent parent = null) : base(game, parent)
        {
            // Defaults for slider handle and background
            if (handle == null)
            {
                handle = new UISliderHandleDefault(game, this);
            }
            if (background == null)
            {
                background = new UIColoredRect(game, Color.Firebrick, true);
            }


            _handle = handle;
            _background = background;
            Direction = direction;


            background.WithLayout(Layout.FullscreenLayout());

            AddChild(handle);
            AddChild(background);

            HandleSizePercent = 0.1f;
            SlidePercent = 0f;
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            _cachedBounds = targetRect;
            // default background
            if (_background == null)
            {
                screen.DrawRect(targetRect, Color.SlateGray);
            }
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
            Vector2 mousePos = _game.CurrentCursor.Position;
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
            float minPos = minRectPos;
            float maxPos = maxRectPos - ((maxRectPos - minRectPos) * HandleSizePercent);
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
                    _handle.WithLayout(new Layout()
                    {
                        AnchorMin = Vector2.UnitX * startPercent,
                        AnchorMax = Vector2.UnitX * endPercent + Vector2.UnitY
                    });
                    break;
                case SliderDirection.TopToBottom:
                    _handle.WithLayout(new Layout()
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

            float minPos = minRectPos;
            float maxPos = maxRectPos - ((maxRectPos - minRectPos) * HandleSizePercent);
            float deltaPercent = deltaPos / (maxPos - minPos);
            SlidePercent += deltaPercent;
        }

        public abstract class UISliderHandle : UIMenuButtonBase
        {
            private UISlider _parent;
            private Vector2 _dragMousePrev;
            private bool _dragging = false;

            private Vector2 mousePos => _game.CurrentCursor.Position;
            private bool mousePressing => RawInput.MousePressing(MouseButton.Left);

            public UISliderHandle(GamePlus game, UISlider parent = null) : base(game, parent)
            {
                _parent = parent;
            }

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
                        {
                            // Released.
                            _dragging = false;
                        }
                    }

                    if (_dragging)
                    {
                        _parent.ChangeSliderPosition(mousePos - _dragMousePrev);

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
