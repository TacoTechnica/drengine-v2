using GameEngine.Game.Objects;
using GameEngine.Game.Objects.Rendering;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.Collision
{
    public interface ICollider
    {
        public GameObjectRender3D GameObject { get; }
        bool ContainsScreen(Camera3D cam, Vector2 screenPoint);

        Vector3 GetRoughCenterPosition();

        void DrawDebug(GamePlus game, Camera3D cam);
    }
}