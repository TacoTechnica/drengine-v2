using Microsoft.Xna.Framework;

namespace DREngine
{
    public abstract class GameObject : GameComponent
    {
        public GameObject() : base(DRGame.Instance) {
            Awake();
        }

        /// <summary>
        ///
        /// </summary>
        protected abstract void Awake();
    }
}
