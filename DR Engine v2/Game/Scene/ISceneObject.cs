using DREngine.Editor.SubWindows.FieldWidgets;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DREngine.Game.Scene
{
    public interface ISceneObject
    {
        // This can be removed (as it's not used now) but I'll keep it here in case if we decide to switch to this later for compatibility reasons.
        [FieldIgnore]
        public string Type { get; }

        public string Name { get; set; }

        [JsonIgnore]
        [FieldIgnore]
        public Vector3 FocusCenter { get;}
        [JsonIgnore]
        [FieldIgnore]
        public float FocusDistance => 10f;

    }
}
