using GameEngine.Game.Objects;
using GameEngine.Game.Objects.Rendering;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.Collision
{
    public class BoxCollider : ICollider
    {
        private BoundingBox _box;
        private Rectangle _cachedScreenRect = Rectangle.Empty;

        private bool _needsToUpdateScreen = false;

        public BoxCollider(GameObject obj, BoundingBox b)
        {
            _box = b;
            GameObject = obj;
            obj.AddCollider(this);
        }

        public BoxCollider(GameObject obj, Vector3 min, Vector3 max) : this(obj, new BoundingBox(min, max))
        {
        }

        public Vector3 Min
        {
            get => _box.Min;
            set => _box.Min = value;
        }

        public Vector3 Max
        {
            get => _box.Max;
            set => _box.Max = value;
        }

        public GameObject? GameObject { get; }

        public bool ContainsScreen(Camera3D cam, Vector2 screenPoint)
        {
            var r = cam.GetScreenRay(screenPoint);
            return r.Intersects(_box).HasValue;
        }

        public Vector3 GetRoughCenterPosition()
        {
            return (Max - Min) / 2f;
        }

        public void DrawDebug(GamePlus _game, Camera3D cam)
        {
            DebugDrawer.DrawAABB(_game, cam, _box);
        }
    }
}