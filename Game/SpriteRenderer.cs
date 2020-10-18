using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class SpriteRenderer : GameObjectRender3D
    {
        public Sprite Sprite { get; private set; }

        public SpriteRenderer(GamePlus game, Sprite sprite, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
            Sprite = sprite;
        }

        public SpriteRenderer(GamePlus game, Sprite sprite) : base(game)
        {
            Sprite = sprite;
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Matrix worldMat)
        {

        }
    }
}
