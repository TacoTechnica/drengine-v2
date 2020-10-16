namespace DREngine
{
    public interface IGameStarter
    {
        public void Initialize();
        public void Update(float deltaTime);
        public void Draw();
    }
}
