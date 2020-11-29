using Newtonsoft.Json;

namespace DREngine.Game.VN
{
    public class VNState
    {
        [JsonConverter(typeof(ProjectResourceConverter))]
        public VNScript CurrentScript;
    }
}
