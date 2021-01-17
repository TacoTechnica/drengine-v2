using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DREngine.Game.Scene
{
    public interface ISceneObject
    {
        // This can be removed (as it's not used now) but I'll keep it here in case if we decide to switch to this later for compatibility reasons.
        public string Type { get; set; }

        [JsonIgnore]
        public Vector3 FocusCenter { get;}
        [JsonIgnore]
        public float FocusDistance => 10f;

    }
}
