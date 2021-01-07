using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace GameEngine.Game.Objects.Rendering
{
    public class SimpleMeshRenderer<TVertex> : BaseMeshRenderer<TVertex> where TVertex : struct, IVertexType
    {
        private RasterizerState _cachedOgRasterizerState;

        private BasicEffect _effect;

        public SimpleMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
        }

        private BasicEffect Effect
        {
            get
            {
                if (_effect == null)
                {
                    Assert.IsNotNull(Game.GraphicsDevice);
                    _effect = new BasicEffect(Game.GraphicsDevice)
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

        [JsonIgnore] public bool CullingEnabled = true;

        #endregion
    }
}