
namespace GameEngine.Game
{
    /// <summary>
    ///     Defines a resource that the game might want to load from somewhere.
    /// </summary>
    public interface IGameResource
    {
        string Path { get; set; }
        void Load(GamePlus game);
        void Unload(GamePlus game);
    }
}
