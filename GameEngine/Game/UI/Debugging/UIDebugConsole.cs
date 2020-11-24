
using System;
using System.Text;
using Gtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.UI.Debugging
{
    /// <summary>
    /// The visual part of the debug console.
    /// </summary>
    public class UIDebugConsole : UIComponent
    {
        private UIText _log;
        private UITextInput _input;
        private UISlider _slider;

        private int _dropToBottomFlag = 0;

        public SpriteFont Font => _log.Font;

        public string OutputText
        {
            get => _log.Text;
            set => _log.Text = value;
        }

        public string InputText
        {
            get => _input.Text;
            set => _input.Text = value;
        }

        private float _outputViewHeight;

        public UIDebugConsole(GamePlus game, SpriteFont font, float outputHeight, UIComponent parent = null) : base(game, parent)
        {
            _outputViewHeight = outputHeight;

            // LAYOUTS
            float padding = 4f;
            float textPadding = 2f;
            float sliderWidth = 12f;

            // Text input
            float inputHeight = font.LineSpacing + textPadding * 2;
            _input = (UITextInput) new UITextInput(game, font, Color.White);

            UIComponent textBackground = new UIMaskRect(game, Color.Black, this) // new UIColoredRect(game, Color.Black, false, this) //
                .WithLayout(Layout.SideStretchLayout(Layout.Bottom, inputHeight, padding))
                .WithChild(_input);

            // Output
            _slider = (UISlider) new UISlider(game, UISlider.SliderDirection.TopToBottom)
                .WithLayout(Layout.SideStretchLayout(Layout.Right, sliderWidth));
            _log = (UIText) new UIText(game, font, "TEST OUTPUT", Color.White)
                .WithLayout(Layout.CornerLayout(Layout.TopLeft, 0, outputHeight));
            _log.Layout.Margin.Right = sliderWidth;
            UIComponent logViewport = new UIScrollViewMasked(game, _log, Color.Black, _slider) // new UIScrollView(game, _log, null, outputSlider)//
                .WithLayout(Layout.FullscreenLayout());

            _log.Layout.AnchorMax.X = 1f;

            //new UIContainer(game, this)
            new UIColoredRect(game, Color.Red, true, this)
                .WithLayout(Layout.SideStretchLayout(Layout.Bottom, outputHeight, padding, inputHeight + padding))
                .WithChild(logViewport)
                .WithChild(_slider);
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Resize text
            float logHeight = Math.Max(_outputViewHeight, _log.CachedDrawnTextHeight);
            _log.Layout.Margin.Bottom = -logHeight - _log.Layout.Margin.Top;

            // If we need to move to the bottom, do so at a delayed pace.
            if (_dropToBottomFlag-- > 0)
            {
                _slider.SlidePercent = 1f;
            }
            //Debug.LogSilent($"HEIGHT: {logHeight} => {_log.LayoutRect.Max.Y - _log.LayoutRect.Min.Y}");
        }

        public void SetActive(bool active)
        {
            Active = active;
            _input.Active = active;
        }

        public void SetFocused()
        {
            _input.Select();
        }

        public void MoveLogToBottom()
        {
            _slider.SlidePercent = 1f;
            _dropToBottomFlag = 3; // I thought 2 would be enough. We need three. This is silly.
        }

        public bool IsLogAtBottom()
        {
            return (_slider.EndValue <= _slider.StartValue || _slider.SlidePercent > 0.999f);
        }
    }
}
