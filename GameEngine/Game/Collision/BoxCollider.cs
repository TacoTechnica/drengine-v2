using GameEngine.Game.Objects;
using GameEngine.Game.Objects.Rendering;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.Collision
{
    public class BoxCollider : ICollider
    {
        private BoundingBox _box;

        public BoxCollider(GameObjectRender3D obj, BoundingBox b)
        {
            _box = b;
            GameObject = obj;
            obj.AddCollider(this);
        }

        public BoxCollider(GameObjectRender3D obj, Vector3 min, Vector3 max) : this(obj, new BoundingBox(min, max))
        {
        }

        public Vector3 Min
        {
            get =>  _box.Min;
            set => _box.Min = value;
        }

        public Vector3 Max
        {
            get => _box.Max;
            set => _box.Max = value;
        }

        //public BoundingBox WorldBox => new BoundingBox(GameObject.Transform.Position + Min, GameObject.Transform.Position + Max);
        public GameObjectRender3D GameObject { get; }

        public bool ContainsScreen(Camera3D cam, Vector2 screenPoint)
        {
            Matrix invTransform = GameObject.Transform.GetInverse();

            var worldRay = cam.GetScreenRay(screenPoint);

            var localRay = new Ray(Vector3.Transform(worldRay.Position, invTransform), Vector3.TransformNormal(worldRay.Direction, invTransform));

            return localRay.Intersects(_box).HasValue;
        }

        public Vector3 GetRoughCenterPosition()
        {
            Matrix invTransform = GameObject.Transform.GetInverse();
            Vector3 max = Vector3.Transform(_box.Max, invTransform),
                min = Vector3.Transform(_box.Min, invTransform);
            return GameObject.Transform.Position + (max - min) / 2f;
        }

        public void DrawDebug(GamePlus game, Camera3D cam)
        {
            DebugDrawer.World = GameObject.Transform.Local;
            DebugDrawer.DrawAABB(game, cam, _box);
            DebugDrawer.World = Matrix.Identity;
        }
    }
}