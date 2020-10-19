using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using YamlDotNet.Core.Tokens;

namespace DREngine.Game
{
    public class TestCoroutine : IGameStarter
    {
        private GamePlus _game;

        private GameObjectRender3D _testObj;

        private Coroutine _spinRoutine = null;
        public void Initialize(GamePlus game)
        {
            _game = game;
            new Camera3D(game, Vector3.Backward * 100, Math.FromEuler(0, 0, 0));

            _testObj = new TestTriangleObject(_game, Vector3.Zero, Quaternion.Identity);

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
            if (Input.IsPressed(Keys.Space))
            {
                _testObj.StopAllCoroutines();
                _testObj.StartCoroutine(TestRoutine());
            }

            if (Input.IsPressed(Keys.G))
            {
                if (_spinRoutine != null) _testObj.StopCoroutine(_spinRoutine);
                _spinRoutine = _testObj.StartCoroutine(Spin());
            }
        }

        private IEnumerator TestRoutine()
        {
            Debug.Log("STARTED ROUTINE");
            Debug.Log("1...");
            yield return Coroutine.WaitForSeconds(_game, 2f);
            Debug.Log("2...");
            yield return Coroutine.WaitForSeconds(_game, 2f);
            //yield return new WaitForSeconds(_game, 2f);
            Debug.Log("3!");
        }

        private IEnumerator Spin()
        {
            _testObj.Rotation = Quaternion.Identity;
            while (true)
            {
                Vector3 e = Math.ToEuler(_testObj.Rotation);
                e.Z += 45f;
                _testObj.Rotation = Math.FromEuler(e);
                yield return Coroutine.WaitUntilCondition(_game, () => Input.IsPressed(Keys.W));
            }
        }

        public void Draw()
        {
            
        }
    }
}