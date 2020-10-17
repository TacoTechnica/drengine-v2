using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public abstract class GameObjectRender3D : GameObjectRender
    {
        public GameObjectRender3D(GamePlus game) : base(game)
        {
        }

        public override void Draw(GraphicsDevice g)
        {
            foreach (Camera3D cam in _game.GetCameras())
            {
                Draw(cam, g);
            }
        }

        public abstract void Draw(Camera3D cam, GraphicsDevice g);
    }
}
