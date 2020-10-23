using System;
using DREngine.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Random = DREngine.Util.Random;

namespace DREngine.Game
{
    class Obama : SpriteRenderer
    {
        private static Path sprPath = new EnginePath("projects/test_project/Sprites/Obama.png");
        public float RotationVelocity = 0f;
        private ICollider collider;

        public Obama(GamePlus game, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion)) :
            base(game, new Sprite(game, sprPath, Vector2.UnitX * 0.5f), position, rotation)
        {
            CullingEnabled = false;
        }

        // TODO: Cleaner collider creation.
        public override void Start()
        {
            base.Start();
            Vector3 size = Vector3.Up * Sprite.Height * Sprite.Scale + Vector3.Right * Sprite.Width * Sprite.Scale +
                           Vector3.Forward * 1f;
            Vector3 min = Transform.Position - Vector3.UnitX * size.X / 2 - Vector3.UnitZ * size.Z / 2;
            collider = new BoxCollider(this, min, min + size);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            // Rotate speed
            Vector3 euler = Math.ToEuler(Transform.Rotation);
            euler.Y += RotationVelocity * dt;
            Transform.Rotation = Math.FromEuler(euler);

            // Deaccelerate with some extra damping.
            float deaccel = (180f / 2.5f) * dt;
            RotationVelocity = (deaccel > Math.Abs(RotationVelocity)) ? 0 : RotationVelocity - Math.Sign(RotationVelocity)*deaccel;
            RotationVelocity -= (RotationVelocity * 1.4f) * dt;
        }
    }

    public class TestMouseCollider : IGameRunner
    {
        private GamePlus _game;
        private Camera3D _cam;

        private int NumObamas = 20;

        public void Initialize(GamePlus game)
        {
            _game = game;
            _cam = new Camera3D(_game, Vector3.Backward * 10);

            Vector3 min = new Vector3(-10, -4, -6);
            Vector3 max = new Vector3(10, -3, 20);

            // Create the obamas
            for (int i = 0; i < NumObamas; ++i)
            {
                Vector3 pos = Random.GetRange(min, max);
                new Obama(_game, pos);
            }
        }

        public void Update(float deltaTime)
        {
            if (RawInput.MousePressed(MouseButton.Left)) {
                Debug.Log("Press!");
                //Vector2 pos = RawInput.GetMousePosition();
                Vector2 pos = _game.CurrentCursor.Position;
                ICollider c = _game.CollisionManager.ScreenCollisionCheckNearest(_cam, pos);
                if (c != null)
                {
                    Debug.Log("SPEEN");
                    if (c.GameObject is Obama obama)
                    {
                        obama.RotationVelocity += 2.3f*180f;
                    }
                }
            }
        }

        public void Draw()
        {

        }
    }
}
