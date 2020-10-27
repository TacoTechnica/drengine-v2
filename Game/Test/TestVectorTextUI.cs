using DREngine.Game.UI;
using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static DREngine.Game.Input.RawInput;

namespace DREngine.Game
{
    public class TestVectorTextUI : IGameRunner
    {
        private VectorFont vfont;

        private VectorTextUI _text;

        public void Initialize(GamePlus game)
        {
            vfont = new VectorFont(game, new EnginePath("default_resources/Fonts/SourceSansPro/SourceSansPro-Bold.ttf"));

            _text = new VectorTextUI(game, vfont);
            _text.Layout.Pivot = Vector2.Zero;
            _text.Size = 14;
            _text.Text = "The quick brown fox jumped over the lazy dog.\nHello there!\nABCDEFGHIJKLMNOPQRSTUVWXYZ\nabcdefghijklmnopqrstuvwxyz\n.,!?@#$%^&*()-+-=\"';:\\|/<>";
            _text.AddToRoot();
        }

        public void Update(float deltaTime)
        {
            // We good
            Vector3 r = Math.ToEuler(_text.LocalTransform.Rotation);
            if (KeyPressing(Keys.Right))
            {
                r.Z += 120 * deltaTime;
            }
            if (KeyPressing(Keys.Left))
            {
                r.Z -= 120 * deltaTime;
            }
            _text.LocalTransform.Rotation = Math.FromEuler(r);
        }

        public void Draw()
        {
            // We good
        }
    }
}
