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
            Node = node;
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
        private readonly HashSet<ObjectContainerNode<T>> _toDisable = new HashSet<ObjectContainerNode<T>>();
        private readonly HashSet<ObjectContainerNode<T>> _toEnable = new HashSet<ObjectContainerNode<T>>();

        private readonly LinkedList<T> _disabledObjects = new LinkedList<T>();

        public int Count => _objects.Count;

        public bool Contains(ObjectContainerNode<T> node)
        {
            return node != null && node.Node.List == _objects;
        }

        public ObjectContainerNode<T> Add(T obj)
        {
            ObjectContainerNode<T> n = new ObjectContainerNode<T>(_objects.AddLast(obj));
            return n;
        }

        public void RemoveEnqueue(ObjectContainerNode<T> objNode)
        {
            if (objNode == null) return;
            if (objNode.Node.List != _objects && objNode.Node.List != _disabledObjects)
                throw new InvalidOperationException("Tried enqueuing invalid object to delete! Deletion object must come from Add(T obj) or be disabled!");
            _toDelete.Add(objNode);
        }

        public ObjectContainerNode<T> RemoveImmediate(ObjectContainerNode<T> objNode, Action<T> afterDestroy = null)
        {
            if (objNode == null || objNode.Node == null) return null;
            if (objNode.Node.List != _objects && objNode.Node.List != _disabledObjects)
                throw new InvalidOperationException("Invalid object to delete/disable! Deletion object must come from Add(T obj) or be disabled!");

            System.Diagnostics.Debug.Assert(objNode.Node.List != null, "objNode.Node.List != null");
            objNode.Node.List.Remove(objNode.Node);
            afterDestroy?.Invoke(objNode.Node.Value);
            // yes, we return null so we un-set the calling object's previous reference.
            return null;
        }

        public void DisableEnqueue(ObjectContainerNode<T> objNode)
        {
            if (objNode == null) return;
            if (objNode.Node.List != _objects)
                throw new InvalidOperationException("Tried enqueuing invalid object to disable! Disable object must come from Add(T obj)!");
            _toDisable.Add(objNode);
            // Make sure we're not being enabled.
            _toEnable.Remove(objNode);
        }

        public void EnableEnqueue(ObjectContainerNode<T> objNode)
        {
            if (objNode == null) return;
            if (objNode.Node.List != _disabledObjects)
                throw new InvalidOperationException("Tried enqueuing invalid object to enable! Enable object must already be disabled!");
            _toEnable.Add(objNode);
            // Make sure we're not being disabled.
            _toDisable.Remove(objNode);
        }

        public ObjectContainerNode<T> DisableImmediate(ObjectContainerNode<T> objNode)
        {
            // Add to disabled object list.
            ObjectContainerNode<T> result = null;
            // Object might be destroyed
            if (objNode != null)
            {
                result = new ObjectContainerNode<T>(_disabledObjects.AddLast(objNode.Node.Value));
                // Behaviour here is exactly the same. Remove from OBJECT list.
                if (objNode.Node.List != _objects)
                    throw new InvalidOperationException(
                        "Tried disabling an object that wasn't enabled in the first place!");
            }

            RemoveImmediate(objNode);
            return result;
        }

        public ObjectContainerNode<T> EnableImmediate(ObjectContainerNode<T> objNode)
        {
            // Add back to object list
            ObjectContainerNode<T> result = new ObjectContainerNode<T>(_objects.AddLast(objNode.Node.Value));
            // Behaviour here is exactly the same. Remove from DISABLED OBJECT list.
            RemoveImmediate(objNode);
            return result;
        }

        public void LoopThroughAllAndDeleteQueued(Action<T> toRun, Action<T> afterDestroy = null)
        {
            LoopThroughAll(toRun);
            RemoveAllQueuedImmediate(afterDestroy);
        }

        public void LoopThroughAll(Action<T> toRun, bool includeDisabled = false)
        {
            foreach (T obj in _objects)
            {
                toRun.Invoke(obj);
            }

            if (includeDisabled)
            {
                foreach (T obj in _disabledObjects)
                {
                    toRun.Invoke(obj);
                }
            }
        }

        internal void EnableAllQueuedImmediate(Action<T, ObjectContainerNode<T>> afterEnable)
        {
            EmptyOutHashSetWhileModifying(_toEnable, (node) =>
            {
                var newNode = EnableImmediate(node);
                afterEnable?.Invoke(node.Node.Value, newNode);
            });
        }

        internal void DisableAllQueuedImmediate(Action<T, ObjectContainerNode<T>> afterDisable = null)
        {
            EmptyOutHashSetWhileModifying(_toDisable, (node) =>
            {
                var newNode = DisableImmediate(node);
                afterDisable?.Invoke(node.Node.Value, newNode);
            });
        }

        internal void RemoveAllQueuedImmediate(Action<T> afterDestroy = null)
        {
            EmptyOutHashSetWhileModifying(_toDelete, (node) =>
            {
                RemoveImmediate(node, afterDestroy);
            });
        }

        private void EmptyOutHashSetWhileModifying(HashSet<ObjectContainerNode<T>> setOfNodes, Action<ObjectContainerNode<T>> duringEmpty)
        {
            if (setOfNodes.Count == 0) return;
            // Go through all disabled objects and properly delete em
            // Make a copy of all the things we gotta delete, cause we might be modifying this.
            IEnumerable<ObjectContainerNode<T>> diffSet = new List<ObjectContainerNode<T>>(setOfNodes);
            do
            {
                foreach (ObjectContainerNode<T> node in diffSet)
                {
                    duringEmpty.Invoke(node);
                }

                setOfNodes.ExceptWith(diffSet);
                diffSet = new List<ObjectContainerNode<T>>(setOfNodes);
            } while (diffSet.FirstOrDefault() != null); // while not empty

            setOfNodes.Clear();
        }

    }
}
