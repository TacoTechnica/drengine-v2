
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


        /*
        public static IEnumerator WaitForSeconds(GamePlus _game, float duration)
        {
            float start = _game.Time;
            while (_game.Time < start + duration)
            {
                yield return null;
            }
        }
        */

    }

    public abstract class CustomCoroutine : IEnumerator
    {
        protected readonly GamePlus _game;

        public CustomCoroutine(GamePlus game)
        {
            _game = game;
        }
        public abstract bool MoveNext();

        public void Reset()
        {
        }

        public object? Current { get; }
    }

    public class WaitForSeconds : CustomCoroutine
    {
        private readonly float _time;
        private float _start;
        public WaitForSeconds(GamePlus game, float time) : base(game)
        {
            _time = time;
            _start = game.Time;
        }
        public override bool MoveNext()
        {
            return _game.Time < _start + _time;
        }
    }

    public class WaitUntilCondition : CustomCoroutine
    {
        private Func<bool> _condition;

        public WaitUntilCondition(GamePlus game, Func<bool> condition) : base(game)
        {
            _condition = condition;
        }

        public override bool MoveNext()
        {
            return !_condition.Invoke();
        }
    }

}
