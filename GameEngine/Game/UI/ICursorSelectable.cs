namespace GameEngine.Game.UI
{
    public interface ICursorSelectable
    {
        bool CursorSelected { get; set; }
        // ReSharper disable once InconsistentNaming
        bool __ChildWasSelected { get; set; }
        bool ChildrenSelectFirst { get; set; }
        void OnCursorSelect();
        void OnCursorDeselect();
    }
}