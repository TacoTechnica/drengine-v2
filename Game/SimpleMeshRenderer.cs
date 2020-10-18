using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class SimpleMeshRenderer<VType> : GameObjectRender3D where VType : struct, IVertexType
    {

        #region Public access to our effect

        public bool VertexColorEnabled
        {
            get => _effect.VertexColorEnabled;
            set => _effect.VertexColorEnabled = value;
        }
        public bool LightingEnabled
        {
            get => _effect.LightingEnabled;
            set => _effect.LightingEnabled = value;
        }
        public bool TextureEnabled
        {
            get => _effect.TextureEnabled;
            set => _effect.TextureEnabled = value;
        }

        public Texture2D Texture
        {
            get => _effect.Texture;
            set => _effect.Texture = value;
        }

        #endregion

        private VType[] _vertices = new VType[0];
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

        public PrimitiveType PrimitiveType = PrimitiveType.TriangleList;

        private VertexBuffer _vertexBuffer = null;
        // Shader abstraction
        private BasicEffect _effect;

        public SimpleMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation) { }
        public SimpleMeshRenderer(GamePlus game) : base(game) { }

        public override void Start()
        {
            // Shader Abstraction Handling
            _effect = new BasicEffect(_game.GraphicsDevice);
            _effect.Alpha = 1.0f;
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Matrix worldMat)
        {
            if (Vertices.Length == 0) return;

            _effect.Projection = cam.ProjectionMatrix;
            _effect.View = cam.ViewMatrix;
            _effect.World = worldMat;

            // Render verts
            g.SetVertexBuffer(_vertexBuffer);

            // Go through all passes, apply and draw the triangle for each pass.
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                g.DrawPrimitives(PrimitiveType, 0, Vertices.Length);
            }
        }

    }
}
