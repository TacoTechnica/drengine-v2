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
        private static readonly Color DefaultColor = Color.Green;

        private static SpriteFont _debugFont;

        public static void DrawLine3D(GamePlus game, Camera3D cam, Vector3 from, Vector3 to, Color fromC, Color toC)
        {
            game.DebugEffect.View = cam.ViewMatrix;
            game.DebugEffect.Projection = cam.ProjectionMatrix;

            game.DebugEffect.CurrentTechnique.Passes[0].Apply();
            var vertices = new[] {new VertexPositionColor(from, fromC), new VertexPositionColor(to, toC)};
            game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public static void DrawLine3D(GamePlus game, Camera3D cam, Vector3 from, Vector3 to)
        {
            DrawLine3D(game, cam, from, to, DefaultColor, DefaultColor);
        }

        public static void DrawLines(GamePlus game, Camera3D cam, Vector3[] lines, Color color)
        {
            game.DebugEffect.View = cam.ViewMatrix;
            game.DebugEffect.Projection = cam.ProjectionMatrix;

            game.DebugEffect.CurrentTechnique.Passes[0].Apply();
            var vertices = new VertexPositionColor[lines.Length];
            for (var i = 0; i < lines.Length; ++i) vertices[i] = new VertexPositionColor(lines[i], color);
            game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, vertices.Length / 2);
        }

        public static void DrawLines(GamePlus game, Camera3D cam, Vector3[] lines)
        {
            DrawLines(game, cam, lines, DefaultColor);
        }

        public static void DrawAABB(GamePlus game, Camera3D cam, BoundingBox b, Color color)
        {
            Vector3 min = b.Min,
                max = b.Max;
            var delta = max - min;
            Vector3 dx = delta.X * Vector3.UnitX,
                dy = delta.Y * Vector3.UnitY,
                dz = delta.Z * Vector3.UnitZ;
            DrawLines(game, cam,
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

        public static void DrawAABB(GamePlus game, Camera3D cam, BoundingBox b)
        {
            DrawAABB(game, cam, b, DefaultColor);
        }

        public static void DrawText(GamePlus game, string text, Vector2 pos, Color color)
        {
            game.DebugSpriteBatch.Begin();
            game.DebugSpriteBatch.DrawString(GetFont(game), text, pos, color);
            game.DebugSpriteBatch.End();
        }

        public static void DrawText(GamePlus game, string text, Vector2 pos)
        {
            DrawText(game, text, pos, DefaultColor);
        }

        public static void DrawText(GamePlus game, string text, float x, float y, Color color)
        {
            DrawText(game, text, new Vector2(x, y), color);
        }

        public static void DrawText(GamePlus game, string text, float x, float y)
        {
            DrawText(game, text, x, y, DefaultColor);
        }

        private static SpriteFont GetFont(GamePlus game)
        {
            if (_debugFont == null) _debugFont = game.Content.Load<SpriteFont>("Debug/DebugFont");

            return _debugFont;
        }
    }
}