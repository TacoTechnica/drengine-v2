using GameEngine.Game;
using GameEngine.Game.Objects;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;

namespace DREngine.Game.CoreScenes
{
    /// <summary>
    ///     This scene appears while we're waiting for the editor to connect to the game.
    ///     Usually the user won't ever see this scene, even for a frame, since connection happens
    ///     near instantaneously.
    /// </summary>
    public class EditorConnectionScene : BaseSceneLoader
    {
        private const string SCENE_NAME = "__CONNECTION_SCREEN__";

        private readonly DRGame _game;

        public EditorConnectionScene(GamePlus game) : base(game, SCENE_NAME)
        {
            _game = (DRGame) game;
        }

        public override void LoadScene()
        {
            new UIObject(_game);
        }

        private class UIObject : GameObject
        {
            private readonly UIComponent _root;

            public UIObject(DRGame game) : base(game)
            {
                var font = game.ResourceLoader.GetResource<Font>(game.GameProjectData.OverridableResources.MenuFont
                    .GetFullPath(game));

                var background = Color.Black;
                var foreground = Color.White;

                // Background with Prompt text
                _root = new UIColoredRect(_game, background).AddToRoot();
                _root.AddChild(
                    new UIText(_game, font,
                            "Waiting to connect to editor...\nThis must be run from the editor, else it will wait forever.",
                            foreground)
                        .WithHAlign(UIText.TextHAlignMode.Center)
                        .WithVAlign(UIText.TextVAlignMode.Middle)
                        .WithLayout(Layout.CustomLayout(0, 0.5f, 1, 0.5f))
                );
            }

            public override void OnDestroy()
            {
                _root.DestroyImmediate();
            }
        }
    }
}