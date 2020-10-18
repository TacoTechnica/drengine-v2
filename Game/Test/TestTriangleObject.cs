using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class TestTriangleObject : SimpleMeshRenderer<VertexPositionColor>
    {

        public override void Start()
        {
            base.Start();
            // Create triangle
            Vertices = new[]
            {
                new VertexPositionColor(new Vector3(0, 20, 0), Color.Red),
                new VertexPositionColor(new Vector3(-20, -20, 0), Color.Green),
                new VertexPositionColor(new Vector3(20,-20 , 0), Color.Blue)
            };
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Matrix worldMat)
        {

            // Turn off backface culling for our own purposes
            g.RasterizerState = RasterizerState.CullNone;

            base.Draw(cam, g, worldMat);
        }

        public TestTriangleObject(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation) { }
    }
}
