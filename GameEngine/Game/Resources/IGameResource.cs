
namespace GameEngine.Game
{
    /// <summary>
    ///     Defines a resource that the game might want to load from somewhere.
    /// </summary>
    public interface IGameResource
    {
        Path Path { get; set; }
        void Load(GamePlus game);
        void Save(Path path);
        void Unload(GamePlus game);
    }
}
