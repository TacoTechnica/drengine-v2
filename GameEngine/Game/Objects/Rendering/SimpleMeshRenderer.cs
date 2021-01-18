using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace GameEngine.Game.Objects.Rendering
{
    public class SimpleMeshRenderer<TVertex> : BaseMeshRenderer<TVertex> where TVertex : struct, IVertexType
    {
        private RasterizerState _cachedOgRasterizerState;
        private DepthStencilState _cachedOgStencilState;

        private BasicEffect _effect;
        
        

        public SimpleMeshRenderer(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation)
        {
        }

        private BasicEffect Effect
        {
            get
            {
                if (_effect == null && Game != null)
                {
                    Assert.IsNotNull(Game.GraphicsDevice);
                    _effect = new BasicEffect(Game.GraphicsDevice)
                    {
                        Alpha = 1.0f,
                        VertexColorEnabled = true,
                        LightingEnabled = false,
                        TextureEnabled = (typeof(TVertex) == typeof(VertexPositionColorTexture) || typeof(TVertex) == typeof(VertexPositionTexture) || typeof(TVertex) == typeof(VertexPositionNormalTexture))
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
            if (Effect != null)
            {
                Effect.Projection = cam.ProjectionMatrix;
                Effect.View = cam.ViewMatrix;
                Effect.World = transform.Local;

                _cachedOgRasterizerState = g.RasterizerState;
                _cachedOgStencilState = g.DepthStencilState;

                if (IgnoreDepth)
                {
                    g.DepthStencilState = DepthStencilState.None; 
                }

                g.RasterizerState = CullingEnabled ? RasterizerState.CullClockwise : RasterizerState.CullNone;
            }

            return Effect;
        }

        protected override void ResetGraphicsPostDraw(GraphicsDevice g)
        {
            g.RasterizerState = _cachedOgRasterizerState;
            g.DepthStencilState = _cachedOgStencilState;
        }

        #region Public access to our effect

        [JsonIgnore]
        public bool VertexColorEnabled
        {
            get => Effect?.VertexColorEnabled ?? false;
            set
            {
                if (Effect != null) Effect.VertexColorEnabled = value;
            }
        }

        [JsonIgnore]
        public bool LightingEnabled
        {
            get => Effect?.LightingEnabled ?? false;
            set
            {
                if (Effect != null) Effect.LightingEnabled = value;
            }

        }

        [JsonIgnore]
        public bool TextureEnabled
        {
            get => Effect?.TextureEnabled ?? false;
            set
            {
                if (Effect != null) Effect.TextureEnabled = value;
            }
        }

        [JsonIgnore]
        public Texture2D Texture
        {
            get => Effect?.Texture;
            set
            {
                if (Effect != null) Effect.Texture = value;
            }
        }

        [JsonIgnore] public bool CullingEnabled = true;

        [JsonIgnore] public bool IgnoreDepth = false;

        #endregion
    }
}