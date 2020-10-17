using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class TestGame : IGameStarter
    {
        private GraphicsDevice _graphics;

        private Camera3D _cam;
        private TestTriangleObject _testObj;

        public void Initialize(GamePlus game)
        {
            _graphics = game.GraphicsDevice;
            Debug.LogDebug("TestGame Initialize()");

            _cam = new Camera3D(game);
            _testObj = new TestTriangleObject(game, Vector3.Zero);
            new TestTriangleObject(game, new Vector3(10, 10, 10));

        }

        public void Update(float deltaTime)
        {

        }

        public void Draw()
        {

        }
    }
}
