using System;
using Gtk;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.UI
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

        public bool AutoHideSlider = true;

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
            if (AutoHideSlider)
            {
                slider.Active = (slider.StartValue < slider.EndValue);
            }
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
        public UIScrollView WithContentLayout(Layout layout)
        {
            _contents.WithLayout(layout);
            return this;
        }

        /// <summary>
        ///     Given a rect, move the view to fit that rect. Does no centering.
        ///     For example, if the target rect is below the viewport, the viewport will move such that
        ///     the target rect appears at the bottom.
        ///
        ///     You can also give it some padding around the rect that the view will allocate for it.
        /// </summary>
        public void FitRectInView(Rect rect, float padLeft = 0, float padRight = 0, float padTop = 0, float padBottom = 0) {
            float left = rect.Left - padLeft;
            float right = rect.Right + padRight;
            float top = rect.Top - padTop;
            float bot = rect.Bottom + padBottom;

            Rect view = _viewport.LayoutRect;

            float dx = 0;
            float dy = 0;
            // Fit X
            if (padLeft + padRight + rect.Width > view.Width)
            {
                // rect is too big
                dx = ((left + right) / 2f) - ((view.Left + view.Right) / 2f);
            }
            else if (right > view.Right)
            {
                // We're too far right
                dx = right - view.Right;
            }
            else if (left < view.Left)
            {
                // We're too far left
                dx = left - view.Left;
            }

            // Fit Y
            if (padTop + padBottom + rect.Height > view.Height)
            {
                // rect is too big
                dy = ((top + bot) / 2f) - ((view.Top + view.Bottom) / 2f);
            }
            else if (bot > view.Bottom)
            {
                // We're too far right
                dy = bot - view.Bottom;
            }
            else if (top < view.Top)
            {
                // We're too far left
                dy = top - view.Top;
            }

            _contents.Layout.OffsetBy(dx, dy);

            if (_sliderHorizontal != null)
            {
                _sliderHorizontal.Value += dx;
            }
            if (_sliderVertical != null)
            {
                _sliderVertical.Value += dy;
            }
        }

        public void FitRectInView(Rect rect, Padding padding)
        {
            FitRectInView(rect, padding.Left, padding.Right, padding.Top, padding.Bottom);
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
