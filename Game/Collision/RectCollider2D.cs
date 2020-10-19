using Microsoft.Xna.Framework;

namespace DREngine.Game
{
    public class RectCollider2D : ICollider
    {
        private Rectangle _rect;

        public float X
        {
            get => _rect.X;
            set => _rect.X = (int) value;
        }
        public float Y
        {
            get => _rect.Y;
            set => _rect.Y = (int) value;
        }
        public float Width
        {
            get => _rect.Width;
            set => _rect.Width = (int) value;
        }
        public float Height
        {
            get => _rect.Height;
            set => _rect.Height = (int) value;
        }

        public GameObject? GameObject { get; }

        public RectCollider2D(GameObject obj, Rectangle rect)
        {
            _rect = rect;
            GameObject = obj;
            obj.AddCollider(this);
        }
        public RectCollider2D(GameObject obj, float x, float y, float width, float height) : this(obj, new Rectangle((int)x, (int)y, (int)width, (int)height)) {}

        public bool ContainsScreen(Camera3D cam, Vector2 point)
        {
            return _rect.Contains(point);
        }

        // Not applicable for this type, since it's 2D.
        public Vector3 GetRoughCenterPosition()
        {
            return Vector3.Zero;
        }

        public void DrawDebug(GamePlus _game, Camera3D cam)
        {
            throw new System.NotImplementedException();
        }

    }
}
