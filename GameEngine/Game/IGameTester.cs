namespace GameEngine.Game
{
    public interface IGameTester
    {
        public void Initialize(GamePlus game);
        public void Update(float deltaTime);
        public void Draw();
    }
}