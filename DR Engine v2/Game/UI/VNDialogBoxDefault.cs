using System.Collections;
using DREngine.Game.Resources;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game.UI
{

    public interface IVNDialogueBox
    {
        IEnumerator RunDialog(string name, string text);
        void Open();
        void Close();
    }

    public class VNDialogBoxDefault : UIComponent, IVNDialogueBox
    {
        private UISprite _background;
        private UISprite _nameplate;

        private UIText _dialogue;
        private UIText _name;

        public VNDialogBoxDefault(DRGame game, UIComponent parent = null) : base(game, parent)
        {
            DRSprite bg = game.ResourceLoader.GetResource<DRSprite>(
                game.GameData.OverridableResources.DialogueBackground, game
            );
            DRSprite nameplate = game.ResourceLoader.GetResource<DRSprite>(
                game.GameData.OverridableResources.DialogueNameplate, game
            );
            Font dialogFont = game.ResourceLoader.GetResource<Font>(
                game.GameData.OverridableResources.DialogFont, game
            );
            Color dialogueColor = game.GameData.Settings.DialogueTextColor;
            AddChild(_background = (UISprite) new UISprite(game, bg)
                .WithLayout(Layout.CustomLayout(0, 0.7f, 1, 1,
                    16, 0, 16, 16))
            );
            Vector2 minMargin = bg.GetPin(DRSprite.PinType.DialogueTextStart, Vector2.Zero);
            Vector2 maxMargin = new Vector2(bg.Width, bg.Height) - bg.GetPin(DRSprite.PinType.DialogueTextStart, new Vector2(bg.Width, bg.Height));

            _dialogue = (UIText)new UIText(game, dialogFont, "", dialogueColor, _background)
                .WithLayout(Layout.FullscreenLayout(minMargin, maxMargin));

            Close();
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Do nothing.
        }

        // TODO: Text visible adjustment.
        public IEnumerator RunDialog(string name, string text)
        {
            Open();
            _dialogue.Text = text;
            while (!RawInput.KeyPressed(Keys.Space))
            {
                yield return null;
            }
        }

        // TODO: Animate
        public void Open()
        {
            _background.Active = true;
        }

        public void Close()
        {
            _background.Active = false;
        }
    }
}