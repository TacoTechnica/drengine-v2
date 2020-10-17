using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public abstract class GameObjectRender : DrawableGameComponent, IGameObject
    {
        protected GamePlus _game;
        private bool _gottaStart = true;
        public GameObjectRender(GamePlus game) : base(game)
        {
            _game = game;
            game.Components.Add(this);
            _gottaStart = true;
            Awake();
        }

        ~GameObjectRender()
        {
            _game.Components.Remove(this);
        }


#region GameComponent Hookups

        public override void Update(GameTime gameTime)
        {
            // Start before first tick
            if (_gottaStart)
            {
                Start();
                _gottaStart = false;
            }
            Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            // Start before first draw
            if (_gottaStart)
            {
                Start();
                _gottaStart = false;
            }
            Draw(GraphicsDevice);
        }

        #endregion

#region Interface
        public virtual void Awake()
        {

        }

        public  virtual void Start()
        {

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dt"> delta time in seconds </param>
        public  virtual void Update(float dt)
        {

        }

        public virtual void OnDestroy()
        {

        }

        /// <summary>
        /// Draws. Abstract cause we don't want to
        /// mistakenly make tons of game-object-renderers without
        /// ensuring that its rendering is actually taken
        /// advantage of.
        /// </summary>
        /// <param name="g"></param>
        public abstract void Draw(GraphicsDevice g);


#endregion
    }
}
