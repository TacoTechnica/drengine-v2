using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Util;
using Microsoft.Xna.Framework;

namespace GameEngine.Game
{
    /// <summary>
    /// This registers and confirms collisions in a space.
    /// For now this will include simple stuff, like whether a point
    /// collides with a rect or cube.
    /// </summary>
    public class CollisionManager
    {
        private List<ICollider> _colliders = new List<ICollider>();

        internal void RegisterCollider(ICollider collider)
        {
            _colliders.Add(collider);
        }

        internal void DeregisterCollider(ICollider collider)
        {
            _colliders.Remove(collider);
        }

        public ICollider ScreenCollisionCheckFirst(Camera3D cam, Vector2 mousePos)
        {
            return ScreenCollisionCheckAll(cam, mousePos).FirstOrDefault();
        }

        /// <summary>
        /// Gets the CLOSEST collider that we collide with.
        /// </summary>
        public ICollider ScreenCollisionCheckNearest(Camera3D cam, Vector2 mousePos)
        {
            float minDistSq = float.PositiveInfinity;
            return ScreenCollisionCheckAll(cam, mousePos).GetMinOrDefault(collider =>
                (collider.GetRoughCenterPosition() - cam.Position).LengthSquared()
            ).First;
        }

        public IEnumerable<ICollider> ScreenCollisionCheckAll(Camera3D cam, Vector2 mousePos)
        {
            // Return all that are colliding.
            return _colliders.Where(collider => collider.ContainsScreen(cam, mousePos));
        }

        public void DeregisterAll()
        {
            _colliders.Clear();
        }

        public void DrawDebugColliders(GamePlus _game, Camera3D cam)
        {
            foreach (ICollider c in _colliders)
            {
                c.DrawDebug(_game, cam);
            }
        }
    }
}
