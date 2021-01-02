
using GameEngine.Game.Resources;

namespace GameEngine.Game
{
    /// <summary>
    ///     Defines a resource that the game might want to load from somewhere.
    /// </summary>
    public interface IGameResource
    {
        Path Path { get; set; }
        void Load(ResourceLoaderData data);
        void Save(Path path);
        void Unload();
    }
}
