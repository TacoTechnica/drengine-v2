using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace GameEngine.Game.UI
{
    public class UIScreen : UIComponentBase
    {
        public BasicEffect DrawEffect { get; private set; }
        public AlphaTestEffect DrawEffectAlpha { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public Color DefaultDrawColor = Color.White;

        // Do we need to update selectables during the draw phase?
        public bool NeedToUpdateControl = false;

        public float ZPosition2D = 0;

        public Matrix CurrentWorld
        {
            get => DrawEffect.World;
            set => DrawEffect.World = value;
        }

        public Matrix OpenGL2Pixel;

        private UIComponent _rootNode;

        public UIScreen(GamePlus game) : base(game)
        {
            Layout = Layout.FullscreenLayout();
        }

        // Debug stuff
        private static int _debugActiveDrawCounter = 0;
        private static int _debugTotalDrawCounter = 0;
        private static int _debugActiveDrawCount = 0;
        private static int _debugTotalDrawCount = 0;

        public void Initialize()
        {
            GraphicsDevice = _game.GraphicsDevice;
            DrawEffect = new BasicEffect(GraphicsDevice);
            DrawEffectAlpha = new AlphaTestEffect(GraphicsDevice);
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            DrawEffect.VertexColorEnabled = true;

            AddChild(_rootNode = new UIRootNode(_game));
        }

        public void Draw()
        {
            // Debug stuff
            _debugActiveDrawCounter = 0;
            _debugTotalDrawCounter = 0;

            Rect screenRect = GetParentRect();
            float w = screenRect.Width,
                  h = screenRect.Height;
            OpenGL2Pixel = Matrix.CreateScale(2f / w, -2f / h, 1) * Matrix.CreateTranslation(-1, 1, 0);
            CurrentWorld = Matrix.Identity;
            DoDraw(this, CurrentWorld, screenRect);
            NeedToUpdateControl = false;

            // Debug stuff
            _debugActiveDrawCount = _debugActiveDrawCounter;
            _debugTotalDrawCount = _debugTotalDrawCounter;
        }

        public void Update()
        {
            NeedToUpdateControl = true;
        }

        internal void OnUIDraw(bool active)
        {
            if (active) ++_debugActiveDrawCounter;
            ++_debugTotalDrawCounter;
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Do nothing here.
            //DrawRect(0, 0, targetRect.Width / 2, 100f, Color.Red, Color.Green, Color.Blue, Color.Yellow);
        }

        protected override Rect GetParentRect()
        {
            return new Rect(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        public void AddRootChild(UIComponent ui, bool forceBase = false)
        {
            if (forceBase)
            {
                AddChild(ui);
            }
            else
            {
                _rootNode.AddChild(ui);
            }
        }

        /// <summary>
        /// All UI elements will be added to this node. This is here to help deal with root level ordering.
        /// </summary>
        class UIRootNode : UIComponent
        {
            public UIRootNode(GamePlus game, UIComponent parent = null) : base(game, parent)
            {
            }

            protected override void Draw(UIScreen screen, Rect targetRect) { }
        }

        #region Drawing Utilities

        public void SpriteBatchBegin()
        {
            SpriteBatch.Begin(transformMatrix: CurrentWorld, depthStencilState: GraphicsDevice.DepthStencilState);
        }

        public void SpriteBatchEnd()
        {
            SpriteBatch.End();
        }

        public void DrawLine(float x0, float y0, float x1, float y1, Color c0, Color c1)
        {
            DrawEffect.World = CurrentWorld * OpenGL2Pixel;
            DrawEffect.CurrentTechnique.Passes[0].Apply();

            float z = ZPosition2D;

            var vertices = new[] { new VertexPositionColor(new Vector3(x0, y0, z), c0),  new VertexPositionColor(new Vector3(x1, y1, z), c1) };
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public void DrawLine(float x0, float y0, float x1, float y1, Color c)
        {
            DrawLine(x0, y0, x1, y1, c, c);
        }

        public void DrawLine(float x0, float y0, float x1, float y1)
        {
            DrawLine(x0, y0, x1, y1, DefaultDrawColor);
        }

        public void DrawRect(float x, float y, float width, float height, Color c0, Color c1, Color c2, Color c3)
        {
            DrawEffect.World = CurrentWorld *OpenGL2Pixel;
            DrawEffect.CurrentTechnique.Passes[0].Apply();

            float z = ZPosition2D;

            var vertices = new[] {
                new VertexPositionColor(new Vector3(x, y, z), c0),
                new VertexPositionColor(new Vector3(x+width, y+height, z), c2),
                new VertexPositionColor(new Vector3(x, y+height, z), c1),

                new VertexPositionColor(new Vector3(x+width, y+height, z), c2),
                new VertexPositionColor(new Vector3(x, y, z), c0),
                new VertexPositionColor(new Vector3(x+width, y, z), c3)
            };
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
        }

        public void DrawRect(float x, float y, float width, float height, Color c)
        {
            DrawRect(x, y, width, height, c,c,c,c);
        }

        public void DrawRect(float x, float y, float width, float height)
        {
            DrawRect(x, y, width, height, DefaultDrawColor);
        }

        public void DrawRect(Vector2 pos, Vector2 size, Color color0, Color color1, Color color2, Color color3)
        {
            DrawRect(pos.X, pos.Y, size.X, size.Y, color0, color1, color2, color3);
        }

        public void DrawRect(Vector2 pos, Vector2 size, Color color)
        {
            DrawRect(pos, size, color, color, color, color);
        }

        public void DrawRect(Rect r, Color color0, Color color1, Color color2, Color color3)
        {
            DrawRect(r.X, r.Y, r.Width, r.Height, color0, color1, color2, color3);
        }

        public void DrawRect(Rect r, Color color)
        {
            DrawRect(r, color, color, color, color);
        }

        public void DrawRect(Rect r)
        {
            DrawRect(r, DefaultDrawColor);
        }

        public void DrawRectInvisible(float x, float y, float width, float height, Color c0, Color c1, Color c2, Color c3)
        {
            DrawEffectAlpha.World = CurrentWorld *OpenGL2Pixel;
            DrawEffectAlpha.CurrentTechnique.Passes[0].Apply();
            DrawEffectAlpha.Alpha = 1f / 255f;

            float z = ZPosition2D;

            var vertices = new[] {
                new VertexPositionColor(new Vector3(x, y, z), c0),
                new VertexPositionColor(new Vector3(x+width, y+height, z), c2),
                new VertexPositionColor(new Vector3(x, y+height, z), c1),

                new VertexPositionColor(new Vector3(x+width, y+height, z), c2),
                new VertexPositionColor(new Vector3(x, y, z), c0),
                new VertexPositionColor(new Vector3(x+width, y, z), c3)
            };
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
        }

        public void DrawRectOutline(float x, float y, float width, float height, Color c0, Color c1, Color c2, Color c3)
        {
            DrawEffect.World = CurrentWorld * OpenGL2Pixel;
            DrawEffect.CurrentTechnique.Passes[0].Apply();

            // Offsets to surround the rect
            y -= 1;
            x -= 1;
            width += 1;
            height += 1;

            float z = ZPosition2D;

            var vertices = new[] {
                new VertexPositionColor(new Vector3(x, y, z), c0),
                new VertexPositionColor(new Vector3(x+width, y, z), c3),
                new VertexPositionColor(new Vector3(x+width, y+height, z), c2),
                new VertexPositionColor(new Vector3(x, y+height, z), c1),
                new VertexPositionColor(new Vector3(x, y, z), c0),
            };
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 4);
        }

        public void DrawRectOutline(float x, float y, float width, float height, Color c)
        {
            DrawRectOutline(x, y, width, height, c, c, c, c);
        }

        public void DrawRectOutline(float x, float y, float width, float height)
        {
            DrawRectOutline(x, y, width, height, DefaultDrawColor);
        }

        public void DrawRectOutline(Rect rect, Color c0, Color c1, Color c2, Color c3)
        {
            DrawRectOutline(rect.X, rect.Y, rect.Width,rect.Height, c0, c1, c2, c3);
        }

        public void DrawRectOutline(Rect rect, Color color)
        {
            DrawRectOutline(rect, color, color, color, color);
        }

        public void DrawRectOutline(Rect rect)
        {
            DrawRectOutline(rect, DefaultDrawColor);
        }

        #endregion

        // Debug stuff
        public int GetTotalUICountAfterDraw() { return _debugTotalDrawCount; }
        public int GetActiveUICountAfterDraw() { return _debugActiveDrawCount; }

    }
}
