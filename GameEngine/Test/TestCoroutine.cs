using System.Collections;
using GameEngine.Game;
using GameEngine.Game.Coroutine;
using GameEngine.Game.Input;
using GameEngine.Game.Objects;
using GameEngine.Game.Objects.Rendering;
using GameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
{
    public class TestCoroutine : IGameTester
    {
        private GamePlus _game;

        private GameObjectRender3D _testObj;

        private Coroutine _spinRoutine;
        public void Initialize(GamePlus game)
        {
            _game = game;
            new Camera3D(game, Vector3.Backward * 100, Math.FromEuler(0, 0, 0));

            _testObj = new ExampleTriangleObject(_game, Vector3.Zero, Quaternion.Identity);

            IEnumerator test = Wtf0();
            while (test.MoveNext())
            {
                if (test.Current is IEnumerator)
                {
                    while ( ((IEnumerator)test.Current).MoveNext()) {}
                }
            }

        }

        private IEnumerator Wtf0()
        {
            Debug.Log("WTF Start");
            yield return Wtf1();
            Debug.Log("WTF End");
        }

        private IEnumerator Wtf1()
        {
            Debug.Log("INNER START");
            yield return null;
            Debug.Log("INNER END");
        }

        public void Update(float deltaTime)
        {
            if (RawInput.KeyPressed(Keys.Space))
            {
                _testObj.StopAllCoroutines();
                _testObj.StartCoroutine(TestRoutine());
            }

            if (RawInput.KeyPressed(Keys.G))
            {
                if (_spinRoutine != null) _testObj.StopCoroutine(_spinRoutine);
                _spinRoutine = _testObj.StartCoroutine(Spin());
            }

            if (RawInput.KeyPressed(Keys.S))
            {
                _testObj.StartCoroutine(StressTest());
            }
        }

        private IEnumerator TestRoutine()
        {
            Debug.Log("STARTED ROUTINE");
            Debug.Log("1...");
            yield return new WaitForSeconds(_game, 1);//Coroutine.WaitForSeconds(_game, 1f);
            Debug.Log("2...");
            yield return new WaitForSeconds(_game, 1);
            Debug.Log("3!");
        }

        private IEnumerator Spin()
        {
            _testObj.Transform.Rotation = Quaternion.Identity;
            while (true)
            {
                Vector3 e = Math.ToEuler(_testObj.Transform.Rotation);
                e.Z += 45f;
                _testObj.Transform.Rotation = Math.FromEuler(e);
                yield return new WaitUntilCondition(_game, () => RawInput.KeyPressed(Keys.W));
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator StressTest()
        {
            Debug.Log("RUNNING STRESS TEST to check for memory.");
            while (true)
            {
                yield return new WaitUntilCondition(_game, () => true);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public void Draw()
        {

        }
    }
}
