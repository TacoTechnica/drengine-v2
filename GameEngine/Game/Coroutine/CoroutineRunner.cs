using System.Collections;
using System.Collections.Generic;

namespace GameEngine.Game.Coroutine
{
    public class CoroutineRunner
    {
        private readonly List<Coroutine> _routines = new List<Coroutine>();

        public Coroutine Run(IEnumerator enumerator)
        {
            var c = new Coroutine(enumerator);
            _routines.Add(c);
            return c;
        }

        public void StopAll()
        {
            _routines.Clear();
        }

        public void Stop(Coroutine c)
        {
            _routines.Remove(c);
        }

        // TODO: Pause & Resume coroutine for the extra spice

        public void OnTick()
        {
            for (var i = _routines.Count - 1; i >= 0; --i)
                if (!_routines[i].UpdateNext())
                    _routines.RemoveAt(i);
            //_routines.RemoveAll(c => !c.UpdateNext());
        }
    }
}