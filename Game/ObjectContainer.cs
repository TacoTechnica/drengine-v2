using System;
using System.Collections.Generic;
using System.Linq;

namespace DREngine.Game
{

    /// <summary>
    ///     Whenever we add an object to an object container we return a reference
    ///     for where that object is. This is to potentially swap out the
    ///     implementation later.
    /// </summary>
    public class ObjectContainerNode<T>
    {
        internal LinkedListNode<T> Node;
        public ObjectContainerNode(LinkedListNode<T> node)
        {
            Node = Node;
        }
    }

    /// <summary>
    ///     Represents a container of gameobjects that we can manage.
    /// </summary>
    /// <typeparam name="T"> The gameobject type. </typeparam>
    public class ObjectContainer<T>
    {
        private readonly LinkedList<T> _objects = new LinkedList<T>();
        private readonly HashSet<ObjectContainerNode<T>> _toDelete = new HashSet<ObjectContainerNode<T>>();

        public int Count => _objects.Count;

        public ObjectContainerNode<T> Add(T obj)
        {
            ObjectContainerNode<T> n = new ObjectContainerNode<T>(_objects.AddLast(obj));
            return n;
        }

        public void RemoveEnqueue(ObjectContainerNode<T> objNode)
        {
            if (objNode == null) return;
            if (objNode.Node.List != _objects) throw new InvalidOperationException("Tried enqueuing invalid object to delete! Deletion object must come from Add(T obj)!");
            _toDelete.Add(objNode);
        }

        public ObjectContainerNode<T> RemoveImmediate(ObjectContainerNode<T> objNode, Action<T> afterDestroy = null)
        {
            if (objNode == null || objNode.Node == null) return null;
            if (objNode.Node.List != _objects) throw new InvalidOperationException("Invalid object to delete! Deletion object must come from Add(T obj)!");

            _objects.Remove(objNode.Node);
            afterDestroy?.Invoke(objNode.Node.Value);
            // yes, we return null so we un-set the calling object's previous reference.
            return null;
        }

        public void LoopThroughAllAndDeleteQueued(Action<T> toRun, Action<T> afterDestroy = null)
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
            IEnumerable<ObjectContainerNode<T>> diffSet = new List<ObjectContainerNode<T>>(_toDelete);
            do
            {
                foreach (ObjectContainerNode<T> node in diffSet)
                {
                    RemoveImmediate(node, afterDestroy);
                }

                _toDelete.ExceptWith(diffSet);
                diffSet = new List<ObjectContainerNode<T>>(_toDelete);
            } while (diffSet.FirstOrDefault() != null); // while not empty

            _toDelete.Clear();
        }
    }
}
