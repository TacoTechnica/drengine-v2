namespace DREngine.Game.UI
{
    public class DRGameUI
    {

        public IVNDialogueBox VNDialogueBox;

        public DRGameUI(DRGame game)
        {
            VNDialogueBox = (IVNDialogueBox) new VNDialogBoxDefault(game).AddToRoot();
        }
    }
}
