using System;
using System.Collections.Generic;
using System.Linq;

namespace DREngine.Game
{
    /// <summary>
    ///     Represents a container of gameobjects that we can manage.
    /// </summary>
    /// <typeparam name="T"> The gameobject type. </typeparam>
    public class ObjectContainer<T>
    {
        private readonly LinkedList<T> _objects = new LinkedList<T>();
        private readonly HashSet<LinkedListNode<T>> _toDelete = new HashSet<LinkedListNode<T>>();

        public int Count => _objects.Count;

        public LinkedListNode<T> Add(T obj)
        {
            return _objects.AddLast(obj);
        }

        public void RemoveEnqueue(LinkedListNode<T> objNode)
        {
            if (objNode == null) return;
            if (objNode.List != _objects) throw new InvalidOperationException("Tried enqueuing invalid object to delete! Deletion object must come from Add(T obj)!");
            _toDelete.Add(objNode);
        }

        public LinkedListNode<T> RemoveImmediate(LinkedListNode<T> objNode, Action<T> afterDestroy = null)
        {
            if (objNode == null) return null;
            if (objNode.List != _objects) throw new InvalidOperationException("Invalid object to delete! Deletion object must come from Add(T obj)!");

            _objects.Remove(objNode);
            afterDestroy?.Invoke(objNode.Value);
            // yes, we return null so we un-set the calling object's previous reference.
            return null;
        }

        public void LoopThroughAllAndDeleteQueued(Action<T> toRun, Action<T> afterDestroy)
        {
            LoopThroughAll(toRun);
            RemoveAllQueuedImmediate(afterDestroy);
        }

        public void LoopThroughAll(Action<T> toRun)
        {
            foreach (T obj in _objects)
            {
                toRun.Invoke(obj);
            }
        }

        internal void RemoveAllQueuedImmediate(Action<T> afterDestroy = null)
        {
            // Make a copy of all the things we gotta delete, cause we might be modifying this.
            IEnumerable<LinkedListNode<T>> diffSet = new List<LinkedListNode<T>>(_toDelete);
            do
            {
                foreach (LinkedListNode<T> node in diffSet)
                {
                    RemoveImmediate(node, afterDestroy);
                }

                _toDelete.ExceptWith(diffSet);
                diffSet = new List<LinkedListNode<T>>(_toDelete);
            } while (diffSet.FirstOrDefault() != null); // while not empty

            _toDelete.Clear();
        }

    }
}
