using System;
using System.Collections.Generic;
using GameEngine.Game;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.Splash
{
    /// <summary>
    /// The
    /// </summary>
    public class SplashScene : BaseSceneLoader
    {
        private const string SCENE_NAME = "__SPLASH_SCREEN__";

        public SplashScene(DRGame game) : base(game, SCENE_NAME)
        {
        }

        public override void LoadScene()
        {

        }

        class UIHandler : GameObject
        {
            private UIProjectPicker _projectPicker;
            public UIHandler(GamePlus game) : base(game)
            {

            }

            public override void OnDestroy()
            {
                _projectPicker.DestroyImmediate();
            }
        }

        /// <summary>
        ///     Picks a project from a list of options, displaying some info.
        /// </summary>
        class UIProjectPicker : UIComponent
        {
            public Action<string> OnProjectPick;

            private MenuList _menu;

            public UIProjectPicker(DRGame game) : base(game)
            {
                _menu = new MenuList(game, game.MenuControls.Select, game.MenuControls.MouseSelect, game.MenuControls.MoveDown, game.MenuControls.MoveUp);
            }

            public void ClearProjects()
            {
                List<IMenuItem> toDestroy = new List<IMenuItem>(_menu.Children);
                foreach (IMenuItem item in toDestroy)
                {
                    if (item is UIComponent itemUI)
                    {
                        itemUI.DestroyImmediate();
                    }
                    _menu.RemoveChild(item);
                }
            }

            public void AddProjectOption(string projectName, string projectAuthor, string projectPath, Sprite icon)
            {
            }

            protected override void Draw(UIScreen screen, Rect targetRect)
            {

            }

            class UIProjectOptionButton : UIMenuButtonBase
            {
                private Color _deselectTextColor = Color.White;
                private Color _deselectBackgroundColor = Color.DarkSlateBlue;
                private Color _selectTextColor = Color.SandyBrown;
                private Color _selectBackgroundColor = Color.Cornsilk;
                private Color _pressTextColor = Color.LightGoldenrodYellow;
                private Color _pressBackgroundColor = Color.LightSlateGray;

                private UIText _nameText;
                private UIText _authorText;
                private UIColoredRect _background;

                public UIProjectOptionButton(GamePlus game, SpriteFont font, string name, string author, Sprite icon, UIComponent parent = null) : base(game, parent)
                {
                    _background = new UIColoredRect(game, _deselectBackgroundColor, false, this);

                    float padX = 8,
                        padY = 8;
                    float dx = padX,
                        dy = padY;

                    float spriteMaxWidth = 64;
                    // TODO: Add sprite, offset DX by sprite width (that should be fixed!).

                    dx += spriteMaxWidth;

                    // Position text
                    _nameText = (UIText) new UIText(game, font, name, _deselectTextColor, this)
                        .WithoutWordWrap()
                        .WithLayout(Layout.CornerLayout(Layout.TopLeft).OffsetBy(dx, dy));
                    dy += _nameText.Font.Spacing;
                    _authorText = (UIText) new UIText(game, font, "by " + author, _deselectTextColor, this)
                        .WithoutWordWrap()
                        .WithLayout(Layout.CornerLayout(Layout.TopLeft).OffsetBy(dx, dy));


                    SetVisualState(_deselectTextColor, _deselectBackgroundColor);
                }

                protected override void Draw(UIScreen screen, Rect targetRect)
                {
                    // Handled by children.
                }

                protected override void OnSelectVisual()
                {
                    SetVisualState(_selectTextColor, _selectBackgroundColor);
                }

                protected override void OnDeselectVisual()
                {
                    SetVisualState(_deselectTextColor, _deselectBackgroundColor);
                }

                protected override void OnPressVisual()
                {
                    SetVisualState(_pressTextColor, _pressBackgroundColor);
                }

                protected override void OnDepressVisual()
                {
                    SetVisualState(_selectTextColor, _selectBackgroundColor);
                }

                private void SetVisualState(Color textColor, Color backgroundColor)
                {
                    // Shade the bottom part for fun.
                    Color tinted = Color.Lerp(backgroundColor, Color.Black, 0.2f);
                    _background.Color0 = backgroundColor;
                    _background.Color1 = backgroundColor;
                    _background.Color2 = tinted;
                    _background.Color3 = tinted;
                    // Text
                    _nameText.Color = textColor;
                    _authorText.Color = textColor;
                }
            }
        }

    }
}
