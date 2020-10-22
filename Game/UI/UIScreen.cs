using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace DREngine.Game.UI
{
    public class UIScreen : UIBaseComponent
    {
        public BasicEffect DrawEffect { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public Color DefaultDrawColor = Color.White;

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
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Do nothing here.
            //DrawRect(0, 0, targetRect.Width / 2, 100f, Color.Red, Color.Green, Color.Blue, Color.Yellow);
        }

        #region Drawing Utilities

        public void SpriteBatchBegin()
        {
            SpriteBatch.Begin(transformMatrix: CurrentWorld);
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

        public void DrawRectOutline(float x, float y, float width, float height, Color color)
        {
            DrawEffect.World = CurrentWorld * OpenGL2Pixel;
            DrawEffect.CurrentTechnique.Passes[0].Apply();

            var vertices = new[] {
                new VertexPositionColor(new Vector3(x, y, 0), color),
                new VertexPositionColor(new Vector3(x+width, y, 0), color),
                new VertexPositionColor(new Vector3(x+width, y+height, 0), color),
                new VertexPositionColor(new Vector3(x, y+height, 0), color),
                new VertexPositionColor(new Vector3(x, y, 0), color),
            };
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 4);
        }

        public void DrawRectOutline(float x, float y, float width, float height)
        {
            DrawRectOutline(x, y, width, height, DefaultDrawColor);
        }

        #endregion

    }
}
