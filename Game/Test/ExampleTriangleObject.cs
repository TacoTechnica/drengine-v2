using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class ExampleTriangleObject : SimpleMeshRenderer<VertexPositionColor>
    {
        public override void Start()
        {
            base.Start();
            // Create triangle
            Mesh.Vertices = new[]
            {
                new VertexPositionColor(new Vector3(0, 20, 0), Color.Red),
                new VertexPositionColor(new Vector3(-20, -20, 0), Color.Green),
                new VertexPositionColor(new Vector3(20,-20 , 0), Color.Blue)
            };
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {

            // Turn off backface culling for our own purposes
            g.RasterizerState = RasterizerState.CullNone;

            base.Draw(cam, g, transform);
        }

        public ExampleTriangleObject(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation) { }
    }
}
