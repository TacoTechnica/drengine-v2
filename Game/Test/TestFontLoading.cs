using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game
{
    /// <summary>
    /// Press F2 to save the project file to temp.yaml
    /// Press F3 to load the project file from temp.yaml
    ///
    /// If you modify the font overrides in temp.yaml and press F3, the
    /// changes should appear immediately.
    /// </summary>
    public class TestFontLoading : IGameRunner
    {
        private DRGame _game;

        // This is a mouthful but it's about as succinct as we can get it.
        // Using redefinitions like this will help.
        private SpriteFont _textFont => _game.GameProjectData.OverridableResources.DialogueFont.Font;

        private SpriteBatch b;


        public void Initialize(GamePlus game)
        {
            _game = game as DRGame;
            b = new SpriteBatch(_game.GraphicsDevice);
        }

        public void Update(float deltaTime)
        {
            // You should be able to load new fonts
            if (Input.KeyPressed(Keys.F2))
            {
                Debug.Log("SAVED PROJECT TEST");
                ProjectData.WriteToFile(new Path("temp.yaml"), _game.GameProjectData );
            }
            if (Input.KeyPressed(Keys.F3))
            {
                Debug.Log("LOAD PROJECT TEST");
                _game.LoadProject(new Path("temp.yaml"));
            }
        }

        public void Draw()
        {
            // TODO: Use _game.UIScreen.whatever
            b.Begin();

            b.DrawString(_textFont, "Big boner down the lane", new Vector2(100, 100), Color.Black);

            b.End();
        }
    }
}
