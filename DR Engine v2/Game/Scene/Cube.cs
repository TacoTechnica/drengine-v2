//using System;

using System.Diagnostics.CodeAnalysis;
using DREngine.Editor.SubWindows.FieldWidgets;
using DREngine.Game.Resources;
using DREngine.ResourceLoading;
using GameEngine.Game.Collision;
using GameEngine.Game.Objects.Rendering;
using GameEngine.Game.Resources;
using GameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using static DREngine.Game.IDependentOnResourceData;

namespace DREngine.Game.Scene
{
    //[Serializable]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Cube : SimpleMeshRenderer<VertexPositionColorTexture>, ISceneObject, IDependentOnResourceData
    {
        private Vector3 _size = Vector3.One;

        private DRSprite _sprite;

        private readonly Color _blend;

        private BoxCollider _boxCollider;

        public Cube(DRGame game, Vector3 size, DRSprite sprite, Vector3 position, Quaternion rotation) : base(game,
            position, rotation)
        {
            Game = game; // TODO: Why?
            PrimitiveType = PrimitiveType.TriangleList;
            _blend = Color.White;
            Size = size;
            Sprite = sprite;

            //Debug.LogDebug("CUBE CREATED");
        }

        // Required Empty constructor for deserializing
        public Cube() : this(CurrentGame, Vector3.One, null, Vector3.Zero, Quaternion.Identity)
        {
            if (Game != null)
            {
                Sprite = Game.ResourceLoader.GetResource<DRSprite>(Game.GameData.OverridableResources.DefaultSurfaceTexture, Game);
            }
        }

        public Vector3 Size
        {
            get => _size;
            set
            {
                _size = value;

                if (_boxCollider == null)
                {
                    _boxCollider = new BoxCollider(this, Vector3.Zero, Vector3.Zero);
                }

                _boxCollider.Max = _boxCollider.Min + new Vector3(_size.X, _size.Y, -1 * _size.Z);

                Vertices = new[]
                {
                    #region V e r t

                    // Top face
                    P(0, 1, 1, 0, 0),
                    P(0, 1, 0, 0, 1),
                    P(1, 1, 0, 1, 1),

                    P(0, 1, 1, 0, 0),
                    P(1, 1, 0, 1, 1),
                    P(1, 1, 1, 1, 0),


                    // Front face
                    P(0, 1, 0, 0, 0),
                    P(0, 0, 0, 0, 1),
                    P(1, 0, 0, 1, 1),

                    P(0, 1, 0, 0, 0),
                    P(1, 0, 0, 1, 1),
                    P(1, 1, 0, 1, 0),

                    // Right face
                    P(1, 1, 0, 0, 0),
                    P(1, 0, 0, 0, 1),
                    P(1, 0, 1, 1, 1),

                    P(1, 1, 0, 0, 0),
                    P(1, 0, 1, 1, 1),
                    P(1, 1, 1, 1, 0),

                    // Back face
                    P(1, 1, 1, 0, 0),
                    P(1, 0, 1, 0, 1),
                    P(0, 0, 1, 1, 1),

                    P(1, 1, 1, 0, 0),
                    P(0, 0, 1, 1, 1),
                    P(0, 1, 1, 1, 0),

                    // Left face
                    P(0, 1, 1, 0, 0),
                    P(0, 0, 1, 0, 1),
                    P(0, 0, 0, 1, 1),

                    P(0, 1, 1, 0, 0),
                    P(0, 0, 0, 1, 1),
                    P(0, 1, 0, 1, 0),

                    // Bottom face
                    P(0, 0, 0, 0, 0),
                    P(0, 0, 1, 0, 1),
                    P(1, 0, 1, 1, 1),

                    P(0, 0, 0, 0, 0),
                    P(1, 0, 1, 1, 1),
                    P(1, 0, 0, 1, 0),

                    #endregion V e r t
                };
            }
        }

        [JsonConverter(typeof(ProjectResourceConverter))]
        public DRSprite Sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                if (_sprite != null && !_sprite.Loaded)
                {
                    CurrentData?.LoadWhenSafe(() => { Texture = _sprite.Texture; });
                }
                else
                {
                    Texture = _sprite?.Texture;
                }
            }
        }

        [FieldIgnore]
        [JsonIgnore]
        public new DRGame Game { get; set; }

        [FieldIgnore]
        public string Type { get; } = "Cube";
        public string Name { get; set; }

        [FieldIgnore]
        public Vector3 FocusCenter => Transform.Position + 0.5f * Math.RotateVector(Size, Transform.Rotation);
        [FieldIgnore]
        public float FocusDistance => 5f + Size.Length() / 2;

        private VertexPositionColorTexture P(float px, float py, float pz, float ux,
            float uy)
        {
            pz *= -1;
            return new VertexPositionColorTexture(new Vector3(px, py, pz) * Size, _blend, new Vector2(ux, uy));
        }
    }
}