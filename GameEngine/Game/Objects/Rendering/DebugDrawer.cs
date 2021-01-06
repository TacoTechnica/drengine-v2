using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.Objects.Rendering
{
    /// <summary>
    ///     Since this relies on passing a game object like everywhere, consider not making this static.
    ///     Might want to keep it like this though so it's easier to remove or something.
    /// </summary>
    public static class DebugDrawer
    {
        private static readonly Color DEFAULT_COLOR = Color.Green;

        private static SpriteFont _debugFont;

        public static void DrawLine3D(GamePlus _game, Camera3D cam, Vector3 from, Vector3 to, Color fromC, Color toC)
        {
            _game.DebugEffect.View = cam.ViewMatrix;
            _game.DebugEffect.Projection = cam.ProjectionMatrix;

            _game.DebugEffect.CurrentTechnique.Passes[0].Apply();
            var vertices = new[] {new VertexPositionColor(from, fromC), new VertexPositionColor(to, toC)};
            _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public static void DrawLine3D(GamePlus _game, Camera3D cam, Vector3 from, Vector3 to)
        {
            DrawLine3D(_game, cam, from, to, DEFAULT_COLOR, DEFAULT_COLOR);
        }

        public static void DrawLines(GamePlus _game, Camera3D cam, Vector3[] lines, Color color)
        {
            _game.DebugEffect.View = cam.ViewMatrix;
            _game.DebugEffect.Projection = cam.ProjectionMatrix;

            _game.DebugEffect.CurrentTechnique.Passes[0].Apply();
            var vertices = new VertexPositionColor[lines.Length];
            for (var i = 0; i < lines.Length; ++i) vertices[i] = new VertexPositionColor(lines[i], color);
            _game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, vertices.Length / 2);
        }

        public static void DrawLines(GamePlus _game, Camera3D cam, Vector3[] lines)
        {
            DrawLines(_game, cam, lines, DEFAULT_COLOR);
        }

        public static void DrawAABB(GamePlus _game, Camera3D cam, BoundingBox b, Color color)
        {
            Vector3 min = b.Min,
                max = b.Max;
            var delta = max - min;
            Vector3 dx = delta.X * Vector3.UnitX,
                dy = delta.Y * Vector3.UnitY,
                dz = delta.Z * Vector3.UnitZ;
            DrawLines(_game, cam,
                new[]
                {
                    min, min + dx,
                    min, min + dy,
                    min, min + dz,
                    max, max - dx,
                    max, max - dy,
                    max, max - dz,

                    min + dx, min + dx + dy,
                    min + dx, min + dx + dz,
                    min + dy, min + dy + dx,
                    min + dy, min + dy + dz,
                    min + dz, min + dz + dx,
                    min + dz, min + dz + dy
                }, color
            );
        }

        public static void DrawAABB(GamePlus _game, Camera3D cam, BoundingBox b)
        {
            DrawAABB(_game, cam, b, DEFAULT_COLOR);
        }

        public static void DrawText(GamePlus _game, string text, Vector2 pos, Color color)
        {
            _game.DebugSpriteBatch.Begin();
            _game.DebugSpriteBatch.DrawString(GetFont(_game), text, pos, color);
            _game.DebugSpriteBatch.End();
        }

        public static void DrawText(GamePlus _game, string text, Vector2 pos)
        {
            DrawText(_game, text, pos, DEFAULT_COLOR);
        }

        public static void DrawText(GamePlus _game, string text, float x, float y, Color color)
        {
            DrawText(_game, text, new Vector2(x, y), color);
        }

        public static void DrawText(GamePlus _game, string text, float x, float y)
        {
            DrawText(_game, text, x, y, DEFAULT_COLOR);
        }

        private static SpriteFont GetFont(GamePlus _game)
        {
            if (_debugFont == null) _debugFont = _game.Content.Load<SpriteFont>("Debug/DebugFont");

            return _debugFont;
        }
    }
}