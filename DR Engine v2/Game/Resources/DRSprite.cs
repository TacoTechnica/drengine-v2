using GameEngine.Game;
using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;

namespace DREngine.Game.Resources
{
    public class DRSprite : Sprite
    {
        // Deserialize Constructor.
        public DRSprite() : base()
        {
            // Default pivot
            this.Pivot = new Vector2(0.5f, 1);
        }

        public DRSprite(DRGame game, Path path) : base(game, path)
        {
            // Default pivot
            this.Pivot = new Vector2(0.5f, 1);
        }
    }
}
