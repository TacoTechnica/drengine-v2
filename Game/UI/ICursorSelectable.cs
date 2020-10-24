
namespace DREngine.Game.UI
{
    public interface ICursorSelectable
    {
        bool Selected { get; set; }
        bool __ChildWasSelected { get; set; }
        bool ChildrenSelectFirst { get; set; }
        void OnSelect();
        void OnDeselect();
    }
}
