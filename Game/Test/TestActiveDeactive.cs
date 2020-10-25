using DREngine.Game.Input;
using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game
{
    /// <summary>
    ///     This was surprisingly challenging.
    ///
    ///     Tests disabling system. Use numpad 0, 1, 2 and 7 to disable/enable the nodes along with their children.
    ///     The behaviour here should be completely succinct.
    /// </summary>
    public class TestActiveDeactive : IGameRunner
    {
        private Camera3D _cam;
        private TestObject2 _exampleObjRoot;
        private TestObject2 _exampleObj1;
        private TestObject2 _exampleObj1_1;
        private TestObject2 _exampleObj2;

        private GamePlus _game;

        public void Initialize(GamePlus game)
        {
            _game = game;
            Debug.LogDebug("TestGame Initialize()");

            _cam = new Camera3D(game);
            _cam.Rotation = Math.FromEuler(0, 0, 180);
            _exampleObjRoot = new TestObject2(game, new Vector3(0, 0, -90), Quaternion.Identity);

            _exampleObj1 = new TestObject2(game, new Vector3(0, 50, -140), Quaternion.Identity);
            _exampleObjRoot.AddChild(_exampleObj1);

            _exampleObj2 = new TestObject2(game, new Vector3(0, 100, -140), Quaternion.Identity);
            _exampleObjRoot.AddChild(_exampleObj2);

            _exampleObj1_1 = new TestObject2(game, new Vector3(-50, 50, -200), Quaternion.Identity );
            _exampleObj1.AddChild(_exampleObj1_1);

        }

        public void Update(float deltaTime)
        {
            // Test basic camera rotation & FOV stuff

            if (RawInput.KeyPressing(Keys.R))
            {
                Vector3 e = Math.ToEuler(_cam.Rotation);
                e.Y += 90f * deltaTime;
                _cam.Rotation = Math.FromEuler(e);
            }
            if (RawInput.KeyPressing(Keys.W))
            {
                _cam.Fov += 10f * deltaTime;
            } else if (RawInput.KeyPressing(Keys.S))
            {
                _cam.Fov -= 10f * deltaTime;
            }

            // Test deletion

            if (RawInput.KeyPressed(Keys.NumPad0))
            {
                Debug.Log($"SET ROOT TO {!_exampleObjRoot.Active}");
                if (_exampleObjRoot != null) _exampleObjRoot.SetActive(!_exampleObjRoot.Active);
            }
            if (RawInput.KeyPressed(Keys.NumPad1))
            {
                Debug.Log($"SET 1 TO {!_exampleObj1.Active}");
                if (_exampleObj1 != null) _exampleObj1.SetActive(!_exampleObj1.Active);
            }
            if (RawInput.KeyPressed(Keys.NumPad2))
            {
                Debug.Log($"SET 2 TO {!_exampleObj2.Active}");
                if (_exampleObj2 != null) _exampleObj2.SetActive(!_exampleObj2.Active);
            }
            if (RawInput.KeyPressed(Keys.NumPad7))
            {
                Debug.Log($"SET 1_1 TO {!_exampleObj1_1.Active}");
                if (_exampleObj1_1 != null) _exampleObj1_1.SetActive(!_exampleObj1_1.Active);
            }

            if (RawInput.KeyPressed(Keys.J))
            {
                Debug.Log("KABOOM");
                _game.SceneManager.UnloadSceneAtEndOfFrame();
            }
        }

        public void Draw()
        {

            _game.SceneManager.GameRenderObjects.LoopThroughAll(ren =>
            {
                if (ren is TestObject2 obj)
                {
                    DebugDrawer.DrawText(_game, $"{obj.Active}, {obj._parentActive}, {obj._activeAsChild}", -obj.Transform.Position.X * 3,
                        obj.Transform.Position.Y);
                }
            }, true);

        }

        class TestObject2 : ExampleTriangleObject
        {
            public TestObject2(GamePlus game, Vector3 position, Quaternion rotation) : base(game, position, rotation)
            {
            }

            public override void OnEnable()
            {
                Debug.Log("TRIANGLE ENABLED");
            }

            public override void OnDisable()
            {
                Debug.Log("TRIANGLE disABLED");
            }

        }
    }

}
