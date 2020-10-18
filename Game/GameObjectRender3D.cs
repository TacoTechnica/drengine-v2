using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public abstract class GameObjectRender3D : GameObjectRender
    {

        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;

        public GameObjectRender3D(GamePlus game, Vector3 position, Quaternion rotation) : base(game)
        {
            this.Position = position;
        }

        public GameObjectRender3D(GamePlus game) : this(game, Vector3.Zero, Quaternion.Identity) { }

        public override void Draw(GraphicsDevice g)
        {

            Matrix rotMat = Matrix.CreateFromQuaternion(Rotation);
            Matrix worldMat = rotMat * Matrix.CreateWorld(Position, Vector3.Forward, Vector3.Up);
            _game.SceneManager.Cameras.LoopThroughAll((cam) =>
            {
                Draw(cam, g, worldMat);
            });
        }


        public abstract void Draw(Camera3D cam, GraphicsDevice g, Matrix worldMat);
    }
}
