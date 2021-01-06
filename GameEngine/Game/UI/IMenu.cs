namespace GameEngine.Game.UI
{
    public interface IMenu
    {
        public bool UseMouse { get; set; }
        void SetSelected(IMenuItem item);
    }
}