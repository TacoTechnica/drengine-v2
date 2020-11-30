using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace GameEngine.Game
{
    public class SimpleMeshRenderer<VType> : BaseMeshRenderer<VType> where VType : struct, IVertexType
    {

        #region Public access to our effect


        [JsonIgnore]
        public bool VertexColorEnabled
        {
            get => Effect.VertexColorEnabled;
            set => Effect.VertexColorEnabled = value;
        }
        [JsonIgnore]
        public bool LightingEnabled
        {
            get => Effect.LightingEnabled;
            set => Effect.LightingEnabled = value;
        }
        [JsonIgnore]
        public bool TextureEnabled
        {
            get => Effect.TextureEnabled;
            set => Effect.TextureEnabled = value;
        }

        [JsonIgnore]
        public Texture2D Texture
        {
            get => Effect.Texture;
            set => Effect.Texture = value;
        }

        [JsonIgnore]
        public bool CullingEnabled = true;

        #endregion

        private BasicEffect _effect = null;

        private RasterizerState _cachedOgRasterizerState = null;

        private BasicEffect Effect
        {
            get
            {
                if (_effect == null)
                {
                    Assert.IsNotNull(_game.GraphicsDevice);
                    _effect = new BasicEffect(_game.GraphicsDevice)
                    {
                        Alpha = 1.0f,
                        VertexColorEnabled = true,
                        LightingEnabled = false,
                        TextureEnabled = true
                    };
                }
                return _effect;
            }
            set => _effect = value;
        }

        public SimpleMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation) { }

        public override void Start()
        {

        }

        protected override Effect PrepareEffectForDraw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            Effect.Projection = cam.ProjectionMatrix;
            Effect.View = cam.ViewMatrix;
            Effect.World = transform.Local;

            _cachedOgRasterizerState = g.RasterizerState;

            g.RasterizerState = CullingEnabled ? RasterizerState.CullClockwise : RasterizerState.CullNone;

            return Effect;
        }

        protected override void ResetGraphicsPostDraw(GraphicsDevice g)
        {
            g.RasterizerState = _cachedOgRasterizerState;
        }
    }
}
