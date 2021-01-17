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

        public BoundingBox WorldBox => new BoundingBox(GameObject.Transform.Position + Min, GameObject.Transform.Position + Max);
        public GameObjectRender3D GameObject { get; }

        public bool ContainsScreen(Camera3D cam, Vector2 screenPoint)
        {
            var r = cam.GetScreenRay(screenPoint);
            return r.Intersects(WorldBox).HasValue;
        }

        public Vector3 GetRoughCenterPosition()
        {
            return GameObject.Transform.Position + (Max - Min) / 2f;
        }

        public void DrawDebug(GamePlus game, Camera3D cam)
        {
            DebugDrawer.DrawAABB(game, cam, WorldBox);
        }
    }
}