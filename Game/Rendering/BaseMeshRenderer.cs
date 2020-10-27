using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public abstract class BaseMeshRenderer<VType> : GameObjectRender3D where VType : struct, IVertexType
    {

        public Mesh<VType> Mesh { get; private set; }

        protected BaseMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
        }

        protected abstract Effect PrepareEffectForDraw(Camera3D cam, GraphicsDevice g, Transform3D transform);
        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            if (Mesh.Vertices.Length == 0) return;

            Effect e = PrepareEffectForDraw(cam, g, transform);

            // Render verts
            g.SetVertexBuffer(Mesh.VertexBuffer);

            // Go through all passes, apply and draw the triangle for each pass.
            foreach (EffectPass pass in e.CurrentTechnique.Passes)
            {
                pass.Apply();
                g.DrawPrimitives(Mesh.PrimitiveType, 0, Mesh.Vertices.Length / 3);
            }
        }
    }
}
