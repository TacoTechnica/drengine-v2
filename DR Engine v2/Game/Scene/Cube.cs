//using System;

using GameEngine;
using GameEngine.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace DREngine.Game.Scene
{
    //[Serializable]
    public class Cube : SimpleMeshRenderer<VertexPositionColorTexture>, ISceneObject
    {
        public string Type { get; set; } = "Cube";

        private Vector3 _size = Vector3.One;

        private Color Blend;

        public Vector3 Size
        {
            get => _size;
            set
            {
                _size = value;

                Vertices = new[]
                {
                    #region V e r t
                    // Top face
                    P(0,1,1, 0,0 ),
                    P(0,1,0, 0,1 ),
                    P(1, 1, 0, 1, 1),

                    P(0, 1, 1, 0, 0),
                    P(1, 1, 0, 1, 1),
                    P(1, 1, 1, 1, 0),


                    // Front face
                    P(0,1,0, 0,0 ),
                    P(0,0,0, 0,1 ),
                    P(1, 0, 0, 1, 1),

                    P(0, 1, 0, 0, 0),
                    P(1, 0, 0, 1, 1),
                    P(1, 1, 0, 1, 0),

                    // Right face
                    P(1,1,0, 0,0 ),
                    P(1,0,0, 0,1 ),
                    P(1, 0, 1, 1, 1),

                    P(1, 1, 0, 0, 0),
                    P(1, 0, 1, 1, 1),
                    P(1, 1, 1, 1, 0),

                    // Back face
                    P(1,1,1, 0,0 ),
                    P(1,0,1, 0,1 ),
                    P(0, 0, 1, 1, 1),

                    P(1, 1, 1, 0, 0),
                    P(0, 0, 1, 1, 1),
                    P(0, 1, 1, 1, 0),

                    // Left face
                    P(0,1,1, 0,0 ),
                    P(0,0,1, 0,1 ),
                    P(0, 0, 0, 1, 1),

                    P(0, 1, 1, 0, 0),
                    P(0, 0, 0, 1, 1),
                    P(0, 1, 0, 1, 0),

                    // Bottom face
                    P(0,0,0, 0,0 ),
                    P(0,0,1, 0,1 ),
                    P(1, 0, 1, 1, 1),

                    P(0, 0, 0, 0, 0),
                    P(1, 0, 1, 1, 1),
                    P(1, 0, 0, 1, 0),
                    #endregion V e r t
                };
            }
        }

        private VertexPositionColorTexture P(float px, float py, float pz, float ux,
            float uy)
        {
            pz *= -1;
            return new VertexPositionColorTexture(new Vector3(px, py, pz) * Size, Blend, new Vector2(ux, uy) );
        }

        private Sprite _sprite = null;
        [JsonConverter(typeof(ProjectResourceConverter))]
        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                if (_sprite != null && !_sprite.Loaded)
                {
                    _game.LoadWhenSafe(() =>
                    {
                        Texture = _sprite.Texture;
                    });
                }
                else
                {
                    Texture = _sprite?.Texture;
                }
            }
        }

        public Cube(GamePlus game, Vector3 size, Sprite sprite, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
            PrimitiveType = PrimitiveType.TriangleList;
            Blend = Color.White;
            Size = size;
            Sprite = sprite;
        }

        // Required Empty constructor for deserializing
        public Cube() : this(ISceneObject.CurrentGame, Vector3.One, null, Vector3.Zero, Quaternion.Identity) {}

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            base.Draw(cam, g, transform);
        }
    }
}
