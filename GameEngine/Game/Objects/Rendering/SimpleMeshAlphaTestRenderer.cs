using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.Objects.Rendering
{
    public class SimpleMeshAlphaTestRenderer<VType> : BaseMeshRenderer<VType> where VType : struct, IVertexType
    {
        private RasterizerState _cachedOgRasterizerState;


        // Shader abstraction
        private AlphaTestEffect _effect;

        public SimpleMeshAlphaTestRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position,
            rotation)
        {
        }

        public override void Start()
        {
            // Shader Abstraction Handling
            _effect = new AlphaTestEffect(_game.GraphicsDevice);
            _effect.Alpha = 1.0f;
            _effect.VertexColorEnabled = true;
        }

        protected override Effect PrepareEffectForDraw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            _effect.Projection = cam.ProjectionMatrix;
            _effect.View = cam.ViewMatrix;
            _effect.World = transform.Local;

            _cachedOgRasterizerState = g.RasterizerState;
            g.RasterizerState = CullingEnabled ? RasterizerState.CullClockwise : RasterizerState.CullNone;

            return _effect;
        }

        protected override void ResetGraphicsPostDraw(GraphicsDevice g)
        {
            g.RasterizerState = _cachedOgRasterizerState;
        }

        #region Public access to our effect

        public bool VertexColorEnabled
        {
            get => _effect.VertexColorEnabled;
            set => _effect.VertexColorEnabled = value;
        }

        public Texture2D Texture
        {
            get => _effect.Texture;
            set => _effect.Texture = value;
        }

        public bool CullingEnabled = true;

        #endregion
    }
}