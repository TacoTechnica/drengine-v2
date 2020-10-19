
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

namespace DREngine.Game
{

    public class Coroutine
    {
        private IEnumerator _enumerator;

        internal Coroutine(IEnumerator enumerator)
        {
            _enumerator = RecursiveIterate(enumerator);
        }

        public bool UpdateNext()
        {
            return _enumerator.MoveNext();
        }

        private IEnumerator RecursiveIterate(IEnumerator enumerator)
        {
            Stack<IEnumerator> toRun = new Stack<IEnumerator>();
            toRun.Push(enumerator);
            while (toRun.Count != 0)
            {
                IEnumerator current = toRun.Peek(); 
                if (!current.MoveNext())
                {
                    toRun.Pop();
                    continue;
                }

                if (current.Current is IEnumerator inner)
                {
                    toRun.Push(inner);
                }

                // That's one iteration down.
                yield return null;
            }
        }


        // TODO: Figure out how to make these classes, it will look nicer.


        public static IEnumerator WaitForSeconds(GamePlus _game, float duration)
        {
            float start = _game.Time;
            while (_game.Time < start + duration)
            {
                yield return null;
            }
        }

        public static IEnumerator WaitUntilCondition(GamePlus _game, Func<bool> condition)
        {
            while (!condition.Invoke())
            {
                yield return null;
            }
        }
    }
}
