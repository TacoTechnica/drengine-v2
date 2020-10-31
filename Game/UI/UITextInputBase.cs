using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.UI
{
    public abstract class UITextInputBase : UIMenuButtonBase
    {
        private UIMask _outerMask;
        private UIText _textRenderer;

        private string _text = "";

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _textRenderer.Text = value;
            }
        }

        public UITextInputBase(GamePlus game, SpriteFont font, UiComponent parent = null) : base(game, parent)
        {
            _outerMask = new UIMaskRect(game, this);
            _textRenderer = new UIText(game, font, "");
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {

        }

        protected override void OnSelectVisual()
        {
            OnSelectVisualInput();
        }

        protected abstract void OnSelectVisualInput();

        /*
        protected override void OnDeselectVisual()
        {
        }

        protected override void OnPressVisual()
        {
        }
        */
    }
}
