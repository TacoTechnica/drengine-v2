namespace GameEngine.Game.Objects
{
    public interface IGameObject
    {
        void Awake();

        void Start();

        /// <summary>
        /// </summary>
        /// <param name="dt"> delta time in seconds </param>
        void Update(float dt);

        void PreUpdate(float dt);
        void PostUpdate(float dt);

        void OnDestroy();
    }
}