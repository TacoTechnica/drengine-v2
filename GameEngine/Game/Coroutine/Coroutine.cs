using System;
using System.Collections;
using System.Collections.Generic;

namespace GameEngine.Game.Coroutine
{
    public class Coroutine
    {
        private readonly IEnumerator _enumerator;

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
            var toRun = new Stack<IEnumerator>();
            toRun.Push(enumerator);
            while (toRun.Count != 0)
            {
                var current = toRun.Peek();
                if (!current.MoveNext())
                {
                    toRun.Pop();
                    continue;
                }

                if (current.Current is IEnumerator inner) toRun.Push(inner);

                // That's one iteration down.
                yield return null;
            }
        }
    }

    public abstract class CustomCoroutine : IEnumerator
    {
        protected readonly GamePlus Game;

        public CustomCoroutine(GamePlus game)
        {
            Game = game;
        }

        public abstract bool MoveNext();

        public void Reset()
        {
        }

        public object Current => null;
    }

    public class WaitForSeconds : CustomCoroutine
    {
        private readonly float _time;
        private readonly float _start;

        public WaitForSeconds(GamePlus game, float time) : base(game)
        {
            _time = time;
            _start = game.Time;
        }

        public override bool MoveNext()
        {
            return Game.Time < _start + _time;
        }
    }

    public class WaitUntilCondition : CustomCoroutine
    {
        private readonly Func<bool> _condition;

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