using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.UI.Debugging
{
    /// <summary>
    ///     The visual part of the debug console.
    /// </summary>
    public class UIDebugConsole : UIComponent
    {
        private int _dropToBottomFlag;
        private readonly UITextInput _input;
        private readonly UIText _log;

        private readonly float _outputViewHeight;
        private readonly UISlider _slider;

        public UIDebugConsole(GamePlus game, Font font, float outputHeight, UIComponent parent = null) : base(game,
            parent)
        {
            _outputViewHeight = outputHeight;

            // LAYOUTS
            var padding = 4f;
            var textPadding = 2f;
            var sliderWidth = 12f;

            // Text input
            var inputHeight = font.SpriteFont != null ? font.SpriteFont.LineSpacing + textPadding * 2 : 18;
            _input = (UITextInput) new UITextInput(game, font, Color.White)
                .WithLayout(Layout.FullscreenLayout());

            var textBackground =
                new UIMaskRect(game, Color.Black, this) // new UIColoredRect(game, Color.Black, false, this) //
                    .WithLayout(Layout.SideStretchLayout(Layout.Bottom, inputHeight, padding))
                    .WithChild(_input);

            // Output
            _slider = (UISlider) new UISlider(game)
                .WithLayout(Layout.SideStretchLayout(Layout.Right, sliderWidth));
            _log = (UIText) new UIText(game, font, "TEST OUTPUT", Color.White)
                .WithLayout(Layout.CornerLayout(Layout.TopLeft, 0, outputHeight));
            _log.Layout.Margin.Right = sliderWidth;
            var logViewport =
                new UIScrollViewMasked(game, _log, Color.Black,
                        _slider) // new UIScrollView(game, _log, null, outputSlider)//
                    .WithLayout(Layout.FullscreenLayout());

            _log.Layout.AnchorMax.X = 1f;

            //new UIContainer(game, this)
            new UIColoredRect(game, Color.Red, true, this)
                .WithLayout(Layout.SideStretchLayout(Layout.Bottom, outputHeight, padding, inputHeight + padding))
                .WithChild(logViewport)
                .WithChild(_slider);
        }

        public Font Font => _log.Font;

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

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Resize text
            var logHeight = Math.Max(_outputViewHeight, _log.CachedDrawnTextHeight);
            _log.Layout.Margin.Bottom = -logHeight - _log.Layout.Margin.Top;

            // If we need to move to the bottom, do so at a delayed pace.
            if (_dropToBottomFlag-- > 0) _slider.SlidePercent = 1f;
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
            return _slider.EndValue <= _slider.StartValue || _slider.SlidePercent > 0.999f;
        }
    }
}