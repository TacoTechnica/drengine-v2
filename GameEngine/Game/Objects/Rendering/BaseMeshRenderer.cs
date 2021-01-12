using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace GameEngine.Game.Objects.Rendering
{
    public abstract class BaseMeshRenderer<TVertex> : GameObjectRender3D where TVertex : struct, IVertexType
    {
        protected VertexBuffer VertexBuffer;

        private TVertex[] _vertices = new TVertex[0];

        [JsonIgnore] public PrimitiveType PrimitiveType = PrimitiveType.TriangleList;

        protected BaseMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position,
            rotation)
        {
        }

        [JsonIgnore]
        public TVertex[] Vertices
        {
            get => _vertices;
            set
            {
                _vertices = value;
                if (Game != null)
                {
                    if (VertexBuffer == null)
                    {
                        VertexBuffer = new VertexBuffer(Game.GraphicsDevice, typeof(TVertex),
                            Vertices.Length, BufferUsage.WriteOnly);
                    }

                    VertexBuffer.SetData(Vertices);
                }
            }
        }

        protected abstract Effect PrepareEffectForDraw(Camera3D cam, GraphicsDevice g, Transform3D transform);

        protected abstract void ResetGraphicsPostDraw(GraphicsDevice g);

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            if (Vertices.Length == 0) return;

            var e = PrepareEffectForDraw(cam, g, transform);

            // Render verts
            g.SetVertexBuffer(VertexBuffer);

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