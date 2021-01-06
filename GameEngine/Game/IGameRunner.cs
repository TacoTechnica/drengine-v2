namespace GameEngine.Game
{
    public interface IGameRunner
    {
        public void Initialize(GamePlus game);
        public void Update(float deltaTime);
        public void Draw();
    }
}