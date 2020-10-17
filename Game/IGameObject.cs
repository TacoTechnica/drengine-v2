namespace DREngine.Game
{
    public interface IGameObject
    {
        void Awake();

       void Start();

       /// <summary>
       ///
       /// </summary>
       /// <param name="dt"> delta time in seconds </param>
       void Update(float dt);

       void OnDestroy();
    }
}
