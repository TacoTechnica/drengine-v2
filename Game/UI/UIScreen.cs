using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace DREngine.Game.UI
{
    public class UIScreen : UIComponentBase
    {
        public BasicEffect DrawEffect { get; private set; }
        public AlphaTestEffect DrawEffectAlpha { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public Color DefaultDrawColor = Color.White;

        // Do we need to update selectables during the draw phase?
        public bool NeedToUpdateSelectables = false;

        // Testing
        private SpriteFont _textFont => ((DRGame)_game).GameProjectData.OverridableResources.DialogueFont.Font;

        public Matrix CurrentWorld
        {
            get => DrawEffect.World;
            set => DrawEffect.World = value;
        }

        public Matrix OpenGL2Pixel;

        public UIScreen(GamePlus game) : base(game)
        {
            Layout = Layout.FullscreenLayout();
        }

        public void Initialize()
        {
            GraphicsDevice = _game.GraphicsDevice;
            DrawEffect = new BasicEffect(GraphicsDevice);
            DrawEffectAlpha = new AlphaTestEffect(GraphicsDevice);
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            DrawEffect.VertexColorEnabled = true;
        }

        public void Draw()
        {
            float w = GraphicsDevice.Viewport.Width,
                h = GraphicsDevice.Viewport.Height;
            OpenGL2Pixel = Matrix.CreateScale(2f / w, -2f / h, 1) * Matrix.CreateTranslation(-1, 1, 0);
            CurrentWorld = Matrix.Identity;
            Rect screenRect = new Rect(
                0, 0,
                w,
                h
            );
            DoDraw(this, CurrentWorld, screenRect);
            NeedToUpdateSelectables = false;
        }

        public void Update()
        {
            NeedToUpdateSelectables = true;
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Do nothing here.
            //DrawRect(0, 0, targetRect.Width / 2, 100f, Color.Red, Color.Green, Color.Blue, Color.Yellow);
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

            var vertices = new[] { new VertexPositionColor(new Vector3(x0, y0, 0), c0),  new VertexPositionColor(new Vector3(x1, y1, 0), c1) };
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

            var vertices = new[] {
                new VertexPositionColor(new Vector3(x, y, 0), c0),
                new VertexPositionColor(new Vector3(x+width, y+height, 0), c2),
                new VertexPositionColor(new Vector3(x, y+height, 0), c1),

                new VertexPositionColor(new Vector3(x+width, y+height, 0), c2),
                new VertexPositionColor(new Vector3(x, y, 0), c0),
                new VertexPositionColor(new Vector3(x+width, y, 0), c3)
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

            var vertices = new[] {
                new VertexPositionColor(new Vector3(x, y, 0), c0),
                new VertexPositionColor(new Vector3(x+width, y+height, 0), c2),
                new VertexPositionColor(new Vector3(x, y+height, 0), c1),

                new VertexPositionColor(new Vector3(x+width, y+height, 0), c2),
                new VertexPositionColor(new Vector3(x, y, 0), c0),
                new VertexPositionColor(new Vector3(x+width, y, 0), c3)
            };
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);
        }

        public void DrawRectOutline(float x, float y, float width, float height, Color c0, Color c1, Color c2, Color c3)
        {
            DrawEffect.World = CurrentWorld * OpenGL2Pixel;
            DrawEffect.CurrentTechnique.Passes[0].Apply();

            var vertices = new[] {
                new VertexPositionColor(new Vector3(x, y, 0), c0),
                new VertexPositionColor(new Vector3(x+width, y, 0), c3),
                new VertexPositionColor(new Vector3(x+width, y+height, 0), c2),
                new VertexPositionColor(new Vector3(x, y+height, 0), c1),
                new VertexPositionColor(new Vector3(x, y, 0), c0),
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

    }
}
