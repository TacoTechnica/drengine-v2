using Microsoft.Xna.Framework;

namespace DREngine.Game
{
    public interface ICollider
    {
        bool ContainsScreen(Camera3D cam, Vector2 screenPoint);

        Vector3 GetRoughCenterPosition();

        void DrawDebug(GamePlus _game, Camera3D cam);

        public GameObject? GameObject { get; }
    }
}
