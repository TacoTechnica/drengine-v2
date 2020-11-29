﻿using System;
using GameEngine;
using GameEngine.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace DREngine.Game.Scene
{
    [Serializable]
    //[JsonConverter(typeof(SceneObjectJsonConverter))]
    public class Billboard : GameObjectRender3D, ISceneObject, IDependentOnDRGame
    {
        public string Type { get; set; } = "Billboard";

        [JsonIgnore]
        public DRGame Game { get; set; }

        public Billboard(DRGame game, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
            Game = game;
        }
        // Required empty constructor for deserialization.
        public Billboard() : this(IDependentOnDRGame.CurrentGame, Vector3.Zero, Quaternion.Identity) {}


        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            //Debug.Log("TODO: Render Billboard");
        }

    }
}
