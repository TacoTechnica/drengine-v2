using DREngine.ResourceLoading;
using Newtonsoft.Json;

namespace DREngine.Game.VN
{
    public class VNStackFrame
    {
        [JsonConverter(typeof(ProjectResourceConverter))]
        public VNScript CurrentScript;

        public int CommandIndex = 0;

        [JsonIgnore] public bool IsFinished => CommandIndex >= CurrentScript.CommandCount;

        public VNCommand GetCurrentCommand()
        {
            return CurrentScript.Get(CommandIndex);
        }

        public void ProgressOneLine()
        {
            CommandIndex++;
        }
        
        // Local Variables go here, if you choose to add them.

        public VNStackFrame(VNScript script, int commandIndex = 0)
        {
            CurrentScript = script;
            CommandIndex = commandIndex;
        }
    }
}