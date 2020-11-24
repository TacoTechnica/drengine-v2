using GameEngine.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.DR
{
    public class Character : GameObjectRender3D
    {

        public Character(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {

        }
    }
}
