using System.Collections.Generic;
using DREngine.Editor.SubWindows.FieldWidgets;
using GameEngine.Game;
using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;

namespace DREngine.Game.Resources
{
    public class DRSprite : Sprite
    {

        [ExtraData]
        [FieldPinEditor]
        public Dictionary<PinType, Vector2> Pins = new Dictionary<PinType,Vector2>();

        // Deserialize Constructor.
        public DRSprite() : base()
        {
            // Default pivot
            this.Pivot = new Vector2(0.5f, 1);
        }

        public DRSprite(DRGame game, Path path) : base(game, path)
        {
            // Default pivot
            this.Pivot = new Vector2(0.5f, 1);
        }

        // Getting pins
        public Vector2 GetPin(PinType type, Vector2 defaultValue)
        {
            if (Pins.ContainsKey(type)) return Pins[type];
            return defaultValue;
        }
        public Vector2 GetPin(PinType type)
        {
            return GetPin(type, Vector2.Zero);
        }

        public enum PinType
        {
            DialogueNameplatePosition,
            CharacterFace
        }
    }
}
