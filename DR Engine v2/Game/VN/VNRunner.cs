using System.Text.Json.Serialization;

namespace DREngine.Game.VN
{
    /// <summary>
    /// An entity that begins running VN Scripts and it don't stop till ya drop.er
    /// </summary>
    public class VNRunner
    {
        public VNState State;

        private DRGame _game;

        public VNRunner(DRGame game)
        {
            _game = game;
            State = new VNState();
            State.CurrentScript = null;

        }

        /// <summary>
        /// Called after a load is done.
        /// </summary>
        public void OnLoad()
        {
            // TODO: Start next script?
        }
    }
}
