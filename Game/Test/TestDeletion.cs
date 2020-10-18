using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace DREngine.Game
{
    public class TestDeletion : IGameStarter
    {
        private GraphicsDevice _graphics;

        private Camera3D _cam;
        private TestTriangleObject _testObj;
        private TestTriangleObject _testObj2;

        private GamePlus _game;

        public void Initialize(GamePlus game)
        {
            _game = game;
            _graphics = game.GraphicsDevice;
            Debug.LogDebug("TestGame Initialize()");

            _cam = new Camera3D(game);
            _cam.Rotation = Math.FromEuler(0, 0, 0);
            _testObj = new TestTriangleObject(game, new Vector3(0, 0, -90), Quaternion.Identity);
            _testObj2 = new TestTriangleObject(game, new Vector3(10, 10, -100), Quaternion.Identity);
            //_testObj.AddChild();
        }

        public void Update(float deltaTime)
        {
            // Test basic camera rotation & FOV stuff

            if (Input.IsPressing(Keys.R))
            {
                Vector3 e = Math.ToEuler(_cam.Rotation);
                e.Y += 90f * deltaTime;
                _cam.Rotation = Math.FromEuler(e);
            }
            if (Input.IsPressing(Keys.W))
            {
                _cam.Fov += 10f * deltaTime;
            } else if (Input.IsPressing(Keys.S))
            {
                _cam.Fov -= 10f * deltaTime;
            }

            // Test deletion

            if (Input.IsPressed(Keys.G))
            {
                Debug.Log("POOF 1");
                if (_testObj != null) _testObj.Destroy();
            }
            if (Input.IsPressed(Keys.H))
            {
                Debug.Log("POOF 2");
                if (_testObj2 != null) _testObj2.Destroy();
            }

            if (Input.IsPressed(Keys.J))
            {
                Debug.Log("KABOOM");
                _game.SceneManager.UnloadScene();
            }
        }

        public void Draw()
        {

        }
    }

}
