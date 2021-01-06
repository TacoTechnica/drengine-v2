using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace GameEngine.Game.Objects.Rendering
{
    public abstract class BaseMeshRenderer<VType> : GameObjectRender3D where VType : struct, IVertexType
    {
        protected VertexBuffer _vertexBuffer;

        private VType[] _vertices = new VType[0];

        [JsonIgnore] public PrimitiveType PrimitiveType = PrimitiveType.TriangleList;

        protected BaseMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position,
            rotation)
        {
        }

        [JsonIgnore]
        public VType[] Vertices
        {
            get => _vertices;
            set
            {
                _vertices = value;
                if (_vertexBuffer == null)
                    _vertexBuffer = new VertexBuffer(_game.GraphicsDevice, typeof(VType),
                        Vertices.Length, BufferUsage.WriteOnly);
                _vertexBuffer.SetData(Vertices);
            }
        }

        protected abstract Effect PrepareEffectForDraw(Camera3D cam, GraphicsDevice g, Transform3D transform);

        protected abstract void ResetGraphicsPostDraw(GraphicsDevice g);

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            if (Vertices.Length == 0) return;

            var e = PrepareEffectForDraw(cam, g, transform);

            // Render verts
            g.SetVertexBuffer(_vertexBuffer);

            // Go through all passes, apply and draw the triangle for each pass.
            foreach (var pass in e.CurrentTechnique.Passes)
            {
                pass.Apply();
                g.DrawPrimitives(PrimitiveType, 0, Vertices.Length / 3);
            }

            ResetGraphicsPostDraw(g);
        }
    }
}