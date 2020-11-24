namespace GameEngine.Game.Tween
{
    /// <summary>
    /// CREDITS TO JEFFREY LANTERS
    /// https://github.com/elraccoone/unity-tweens
    /// For making his awesome lib open source
    /// </summary>
    public interface ITween
    {
        bool RunUpdate();
        void Cancel();
    }
}
