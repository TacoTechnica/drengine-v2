using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public abstract class GameObjectRender3D : GameObjectRender
    {

        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;

        private Matrix _cachedWorldMat = Matrix.Identity;

        public GameObjectRender3D(GamePlus game, Vector3 position, Quaternion rotation) : base(game)
        {
            this.Position = position;
        }

        public GameObjectRender3D(GamePlus game) : this(game, Vector3.Zero, Quaternion.Identity) { }

        public override void PreDraw(GraphicsDevice g)
        {
            Matrix rotMat = Matrix.CreateFromQuaternion(Rotation);
            _cachedWorldMat = rotMat * Matrix.CreateWorld(Position, Vector3.Forward, Vector3.Up);
            _game.SceneManager.Cameras.LoopThroughAll((cam) =>
            {
                PreDraw(cam, g, _cachedWorldMat);
            });
        }

        public override void Draw(GraphicsDevice g)
        {
            _game.SceneManager.Cameras.LoopThroughAll((cam) =>
            {
                Draw(cam, g, _cachedWorldMat);
            });
        }
        public override void PostDraw(GraphicsDevice g)
        {
            _game.SceneManager.Cameras.LoopThroughAll((cam) =>
            {
                PostDraw(cam, g, _cachedWorldMat);
            });
        }


        public abstract void Draw(Camera3D cam, GraphicsDevice g, Matrix worldMat);

        public virtual void PreDraw(Camera3D cam, GraphicsDevice g, Matrix worldMat)
        {

        }
        public virtual void PostDraw(Camera3D cam, GraphicsDevice g, Matrix worldMat)
        {

        }
    }
}
