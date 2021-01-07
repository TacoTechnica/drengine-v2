using System;
using System.Collections.Generic;

namespace GameEngine.Util
{
    /// <summary>
    ///     Basically an event handler but you can control it better. Yeah.
    /// </summary>
    public class EventManager
    {
        private List<Action> _events = new List<Action>();

        private bool _unsubscribeOnInvoke;

        public EventManager(bool unsubscribeOnInvoke = false)
        {
            _unsubscribeOnInvoke = unsubscribeOnInvoke;
        }

        public static EventManager operator +(EventManager e, Action a)
        {
            e.AddListener(a);
            return e;
        }

        public static EventManager operator -(EventManager e, Action a)
        {
            e.RemoveListener(a);
            return e;
        }

        public void AddListener(Action a)
        {
            _events.Add(a);
        }

        public void RemoveListener(Action a)
        {
            _events.Remove(a);
        }

        public void InvokeAll()
        {
            // Allow removal while iterating.
            for (int i = _events.Count - 1; i >= 0; --i)
            {
                _events[i].Invoke();
            }

            if (_unsubscribeOnInvoke)
            {
                ClearAll();
            }
        }

        public void ClearAll()
        {
            _events.Clear();
        }

    }
}
