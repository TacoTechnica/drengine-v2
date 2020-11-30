using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace GameEngine.Game
{
    public abstract class BaseMeshRenderer<VType> : GameObjectRender3D where VType : struct, IVertexType
    {

        [JsonIgnore]
        public PrimitiveType PrimitiveType = PrimitiveType.TriangleList;

        protected VertexBuffer _vertexBuffer = null;

        private VType[] _vertices = new VType[0];
        [JsonIgnore]
        public VType[] Vertices
        {
            get => _vertices;
            set
            {
                _vertices = value;
                if (_vertexBuffer == null)
                {
                    _vertexBuffer = new VertexBuffer(_game.GraphicsDevice, typeof(VType),
                        Vertices.Length, BufferUsage.WriteOnly);
                }
                _vertexBuffer.SetData<VType>(Vertices);
            }
        }

        protected BaseMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
        }

        protected abstract Effect PrepareEffectForDraw(Camera3D cam, GraphicsDevice g, Transform3D transform);

        protected abstract void ResetGraphicsPostDraw(GraphicsDevice g);

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            if (Vertices.Length == 0) return;

            Effect e = PrepareEffectForDraw(cam, g, transform);

            // Render verts
            g.SetVertexBuffer(_vertexBuffer);

            // Go through all passes, apply and draw the triangle for each pass.
            foreach (EffectPass pass in e.CurrentTechnique.Passes)
            {
                pass.Apply();
                g.DrawPrimitives(PrimitiveType, 0, Vertices.Length / 3);
            }

            ResetGraphicsPostDraw(g);
        }

    }
}
