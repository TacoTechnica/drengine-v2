using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class TestTriangleObject : GameObjectRender3D
    {

        // TODO: Some of these will exist in some base class.

        private Vector3 _position = Vector3.Zero;

        // Object world matrix. TODO: Add to GameObject3D class or something
        private Matrix _worldMat;

        // Geometric info
        private VertexPositionColor[] _triangleVerts;
        private VertexBuffer _vertexBuffer;
        // Shader abstraction
        private BasicEffect _effect;

        public TestTriangleObject(GamePlus game, Vector3 pos) : base(game)
        {
            this._position = pos;
        }

        public override void Start()
        {
            _worldMat = Matrix.CreateWorld(_position, Vector3.Forward, Vector3.Up);

            // Shader Abstraction Handling
            _effect = new BasicEffect(_game.GraphicsDevice);
            _effect.Alpha = 1.0f;
            _effect.VertexColorEnabled = true; // TODO: Alter when doing textures
            _effect.LightingEnabled = false; // TODO: Add when we add lighting

            // TODO: Move to GameObject3D or something
            // Create triangle
            _triangleVerts = new[]
            {
                new VertexPositionColor(new Vector3(0, 20, 0), Color.Red),
                new VertexPositionColor(new Vector3(-20, -20, 0), Color.Green),
                new VertexPositionColor(new Vector3(20,-20 , 0), Color.Blue)
            };
            _vertexBuffer = new VertexBuffer(_game.GraphicsDevice, typeof(VertexPositionColor), _triangleVerts.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(_triangleVerts);
        }

        public override void Draw(Camera3D cam, GraphicsDevice g)
        {
            _effect.Projection = cam.ProjectionMatrix;
            _effect.View = cam.ViewMatrix;
            _effect.World = _worldMat;

            // Render verts
            g.SetVertexBuffer(_vertexBuffer);

            // Turn off backface culling for our own purposes
            g.RasterizerState = RasterizerState.CullNone;

            // Go through all passes, apply and draw the triangle for each pass.
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                g.DrawPrimitives(PrimitiveType.TriangleList, 0, _triangleVerts.Length);
            }
        }
    }
}
