using GameEngine.Game;
using GameEngine.Game.Objects;
using GameEngine.Game.Objects.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.CoreScenes.SceneEditor
{
    public class SceneEditorGrid : GameObjectRender3D
    {
        private DRGame _game;
        public SceneEditorGrid(DRGame game) : base(game, Vector3.Zero, Quaternion.Identity)
        {
            _game = game;
        }

        public override void Draw(Camera3D cam, GraphicsDevice g, Transform3D transform)
        {
            // Draw big lines
            float range = 10000;
            Color color = Color.White;

            UnitLine(Vector3.UnitX);
            UnitLine(Vector3.UnitY);
            UnitLine(Vector3.UnitZ);

            void UnitLine(Vector3 unit)
            {
                DebugDrawer.DrawLine3D(_game, cam, -1 * unit * range, unit * range, color);
            }
        }
    }
}