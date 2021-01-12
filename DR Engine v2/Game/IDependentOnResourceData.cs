using GameEngine.Game.Resources;

namespace DREngine.Game
{
    public interface IDependentOnResourceData
    {
        public static ResourceLoaderData CurrentData;
        public static DRGame CurrentGame;
    }
}