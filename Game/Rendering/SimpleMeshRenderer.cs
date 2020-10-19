using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class SimpleMeshRenderer<VType> : BaseMeshRenderer<VType> where VType : struct, IVertexType
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

        public bool CullingEnabled = true;

        #endregion

        private BasicEffect _effect;

        public SimpleMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation) { }

        public override void Start()
        {
            // Shader Abstraction Handling
            _effect = new BasicEffect(_game.GraphicsDevice);
            _effect.Alpha = 1.0f;
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;
        }

        protected override Effect PrepareEffectForDraw(Camera3D cam, GraphicsDevice g, Matrix worldMat)
        {
            _effect.Projection = cam.ProjectionMatrix;
            _effect.View = cam.ViewMatrix;
            _effect.World = worldMat;

            g.RasterizerState = CullingEnabled ? RasterizerState.CullClockwise : RasterizerState.CullNone;

            return _effect;
        }

    }
}
