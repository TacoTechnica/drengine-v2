using System;
using Gtk;
using Microsoft.Xna.Framework;

namespace DREngine.Game.UI
{
    public class UIScrollView : UIComponent
    {
        /**
         * TODO: Make it so we can pass a scroll bar that uses POSITION rather than percentage.
         */

        private UIComponent _contents;

        private UIComponent _viewport;

        private UISlider _sliderVertical;
        private UISlider _sliderHorizontal;

        public enum MovementType
        {
            Unrestricted,
            Clamped,
            Elastic
        }

        public MovementType Movement = MovementType.Clamped;

        public float ElasticCoefficient = 10f;

        public bool ResizeSliderHandle = true;

        public bool MaintainContentOffsetOnResize = true;

        public UIScrollView(GamePlus game, UIComponent contents, UIComponent viewport = null, UISlider sliderVertical = null, UISlider sliderHorizontal = null, UIComponent parent = null) : base(game, parent)
        {
            _contents = contents;
            _viewport = viewport;
            _sliderVertical = sliderVertical;
            _sliderHorizontal = sliderHorizontal;

            if (_viewport == null)
            {
                _viewport = this;
            }

            if (_contents != null)
            {
                _contents.Layout.AnchorMin = Vector2.Zero;
                _contents.Layout.AnchorMax = Vector2.Zero;
            }

            if (_viewport != this)
            {
                AddChild(_viewport);
            }

            _viewport.AddChild(_contents);
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            if (screen.NeedToUpdateControl)
            {
                SetContentBySliders(_viewport.LayoutRect);

                //Vector2 targetMin = GetTargetMin(_viewport.LayoutRect, _contents.LayoutRect);
                //RestrictMovement(targetMin, _viewport.LayoutRect);
            }
        }

        void SetContentBySliders(Rect viewRect)
        {
            Vector2 viewMin = viewRect.Min,
                viewMax = viewRect.Max;
            Vector2 contentMin = _contents.LayoutRect.Min,
                contentMax = _contents.LayoutRect.Max;

            if (_sliderVertical != null)
            {
                UpdateSlider(_sliderVertical, viewMin.Y, viewMax.Y, contentMin.Y, contentMax.Y);
                float target = _sliderVertical.Value;

                _contents.Layout.Margin.SetY(target);

                if (ResizeSliderHandle)
                {
                    //Debug.LogSilent($"VERTICAL BULLSHIT: {contentMax.Y} => {contentMin.Y}: {contentMax.Y - contentMin.Y} / {viewMax.Y - viewMin.Y}");
                    _sliderVertical.HandleSizePercent =
                        GetViewPercent(viewMin.Y, viewMax.Y, contentMin.Y, contentMax.Y);
                }
            }

            if (_sliderHorizontal != null)
            {
                UpdateSlider(_sliderHorizontal, viewMin.X, viewMax.X, contentMin.X, contentMax.X);
                float target = _sliderHorizontal.Value;

                _contents.Layout.Margin.SetX(target);

                if (ResizeSliderHandle)
                {
                    _sliderHorizontal.HandleSizePercent =
                        GetViewPercent(viewMin.X, viewMax.X, contentMin.X, contentMax.X);
                }
            }
        }

        void UpdateSlider(UISlider slider, float viewMin, float viewMax, float contentMin,
            float contentMax)
        {
            slider.StartValue = 0;
            slider.EndValue = (contentMax - contentMin) - (viewMax - viewMin);
        }

        float GetViewPosFromSliderPercent(float sliderPercent, float viewMin, float viewMax, float contentMin,
            float contentMax)
        {
            float viewSize = viewMax - viewMin;
            float contentSize = contentMax - contentMin;
            if (viewSize >= contentSize)
            {
                return viewMin + (viewSize / 2f - contentSize / 2f);
            }

            float contentRange = contentSize - viewSize;
            return viewMin - (contentRange) * sliderPercent;
        }

        float GetViewPercent(float viewMin, float viewMax, float contentMin,
            float contentMax)
        {
            return Math.Clamp01((viewMax - viewMin) / (contentMax - contentMin));
        }

        void RestrictMovement(Vector2 targetMin, Rect viewRect)
        {
            switch (Movement)
            {
                case MovementType.Unrestricted:
                    // No restrictions.
                    break;
                case MovementType.Clamped:
                {
                    Vector2 delta = targetMin - _contents.LayoutRect.Min;
                    _contents.Layout.OffsetBy(delta);
                    break;
                }
                case MovementType.Elastic:
                {
                    Vector2 delta = ElasticCoefficient * (targetMin - _contents.LayoutRect.Min) * _game.UnscaledDeltaTime;
                    _contents.Layout.OffsetBy(delta);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Vector2 GetTargetMin(Rect targetRect, Rect contentRect)
        {
            Vector2 cmin = contentRect.Min,
                cmax = contentRect.Max;
            Vector2 csize = contentRect.Size;
            Vector2 min = targetRect.Min,
                max = targetRect.Max;
            bool smallX = csize.X < targetRect.Width,
                smallY = csize.Y < targetRect.Height;
            Vector2 targetMin = cmin;
            Vector2 center = (targetRect.Min + targetRect.Max) / 2f;
            // Position X
            if (smallX)
            {
                targetMin.X = center.X - csize.X / 2f;
            }
            else if (cmin.X > min.X)
            {
                targetMin.X = min.X;
            }
            else if (cmax.X < max.X)
            {
                targetMin.X = max.X - csize.X;
            }
            // Position Y
            if (smallY)
            {
                targetMin.Y = center.Y - csize.Y / 2f;
            }
            else if (cmin.Y > min.Y)
            {
                targetMin.Y = min.Y;
            }
            else if (cmax.Y < max.Y)
            {
                targetMin.Y = max.Y - csize.Y;
            }

            return targetMin;
        }
    }
    public class UIScrollViewMasked : UIScrollView
    {
        // Transparent background
        public UIScrollViewMasked(GamePlus game, UIComponent contents, UISlider sliderVertical = null, UISlider sliderHorizontal = null, UIComponent parent = null)
            : base(game, contents, new UIMaskRect(game).WithLayout(Layout.FullscreenLayout()), sliderVertical, sliderHorizontal, parent) {
        }
        // Color background
        public UIScrollViewMasked(GamePlus game, UIComponent contents, Color backgroundColor, UISlider sliderVertical = null, UISlider sliderHorizontal = null, UIComponent parent = null)
            : base(game, contents, new UIMaskRect(game, backgroundColor).WithLayout(Layout.FullscreenLayout()), sliderVertical, sliderHorizontal, parent) {
        }
    }
}
