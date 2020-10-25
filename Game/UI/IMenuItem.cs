namespace DREngine.Game.UI
{
    public interface IMenuItem
    {
        IMenu ParentMenu { get; set; }
        bool MenuSelected { get; set; }
        void OnMenuSelect();
        void OnMenuDeselect();

        void OnMenuPress(bool mouse);
    }
}
