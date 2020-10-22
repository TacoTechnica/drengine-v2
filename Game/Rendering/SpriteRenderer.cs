using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class SpriteRenderer : SimpleMeshAlphaTestRenderer<VertexPositionColorTexture>
    {
        private Sprite _sprite;
        private Color _blend = Color.White;

        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                UpdateVertices();
            }
        }
        public Color Blend
        {
            get => _blend;
            set
            {
                _blend = value;
                // TODO: Just update the colors, not the whole thing.
                UpdateVertices();
            }
        }

        public SpriteRenderer(GamePlus game, Sprite sprite, Vector3 position=default(Vector3), Quaternion rotation = default(Quaternion)) : base(game, position, rotation)
        {
            _sprite = sprite;
            PrimitiveType = PrimitiveType.TriangleList;
        }

        public override void Start()
        {
            base.Start();

            UpdateVertices();


        }

        private void UpdateVertices()
        {
            Texture = _sprite.Texture ?? throw new InvalidOperationException("Sprite Renderer's Sprite was not initialized yet!");

            Vector3 up = Vector3.Up * _sprite.Height * _sprite.Scale;
            Vector3 right = Vector3.Right * _sprite.Width * _sprite.Scale;
            Vector3 pivot = _sprite.Pivot.X * right + _sprite.Pivot.Y * up;

            Vector3 topLeft = up - pivot,
                topRight = topLeft + right,
                bottomLeft = -pivot,
                bottomRight = bottomLeft + right;

            // We rotate the order so the default rotation faces US, ( so technically it's backwards )
            Vertices = new[]
            {
                new VertexPositionColorTexture(topLeft, _blend, new Vector2(0, 0) ),
                new VertexPositionColorTexture(bottomRight, _blend, new Vector2(1, 1) ),
                new VertexPositionColorTexture(bottomLeft, _blend, new Vector2(0, 1) ),

                new VertexPositionColorTexture(bottomRight, _blend, new Vector2(1, 1) ),
                new VertexPositionColorTexture(topLeft, _blend, new Vector2(0, 0) ),
                new VertexPositionColorTexture(topRight, _blend, new Vector2(1, 0) )
            };
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            g.BlendState = BlendState.AlphaBlend;
            Texture = _sprite.Texture;
            base.Draw(cam, g, transform);
        }
    }
}
