using System;
using GameEngine;
using GameEngine.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace DREngine.Game.Scene
{
    [Serializable]
    //[JsonConverter(typeof(SceneObjectJsonConverter))]
    public class Billboard : GameObjectRender3D, ISceneObject
    {
        public string Type { get; set; } = "Billboard";

        public Billboard(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
        }
        // Required empty constructor for deserialization.
        public Billboard() : this(ISceneObject.CurrentGame, Vector3.Zero, Quaternion.Identity) {}


        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            //Debug.Log("TODO: Render Billboard");
        }

    }
}
