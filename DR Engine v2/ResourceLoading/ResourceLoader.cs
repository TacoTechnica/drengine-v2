using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Resources;

namespace DREngine
{
    /// <summary>
    /// The "Content Loader" that loads project resources.
    /// </summary>
    public class ResourceLoader
    {

        private ResourceLoaderData _data;

        public ResourceLoader(ResourceLoaderData data)
        {
            _data = data;
        }

        #region Public Access

        public T GetResource<T>(Path path) where T : IGameResource
        {
            if (!ResourceLoaded(path))
            {
                LoadNewResource<T>(path);
            }

            AddResourceDependent(path);

            return GetCachedResource<T>(path);
        }

        // Non generic version
        public object GetResource(Path path, Type t)
        {
            Assert.IsTrue(typeof(IGameResource).IsAssignableFrom(t), $"Not an IGameResource. Type Passed: {t}");
            if (!ResourceLoaded(path))
            {
                LoadNewResource(path, t);
            }
            AddResourceDependent(path);
            object resource = GetCachedResource(path);
            Assert.IsInstanceOf(resource, t);
            return resource;
        }

        public void FinishUsingResource<T>(T resource) where T : IGameResource
        {
            RemoveResourceDependent(resource.Path);
        }

        #endregion

        #region Internal Control

        private Dictionary<string, IGameResource> _resources = new Dictionary<string, IGameResource>();

        private bool ResourceLoaded(Path path)
        {
            return _resources.ContainsKey(path.ToString());
        }

        private T GetCachedResource<T>(Path path) where T : IGameResource
        {
            object resource = GetCachedResource(path);
            if (resource != null)
            {
                Assert.IsInstanceOf<T>(resource);
                return (T) resource;
            }

            return default; // null
        }

        private object GetCachedResource(Path path)
        {
            if (_resources.ContainsKey(path.ToString()))
            {
                IGameResource resource = _resources[path.ToString()];
                return _resources[path.ToString()];
            }
            return null;
        }
        private void LoadNewResource<T>(Path path) where T : IGameResource
        {
            LoadNewResource(path, typeof(T));
        }
        private void LoadNewResource(Path path, Type t)
        {
            if (Assert.IsNotNull(t.GetConstructors().FirstOrDefault(), $"No constructors found for type: {t}"))
            {

                object newResource;
                var parameters = new object[0];
                try
                {
                    newResource = Activator.CreateInstance(t, parameters);
                }
                catch (MissingMethodException e)
                {
                    throw new MissingMethodException($"{t} NEEDS TO HAVE AN EMPTY CONSTRUCTOR SO IT CAN BE DESERIALIZED!");
                }

                Assert.IsNotNull(newResource);


                if (Assert.IsInstanceOf<IGameResource>(newResource) && newResource != null)
                {
                    IGameResource res = (IGameResource) newResource;
                    Assert.IsFalse(_resources.ContainsKey(path));
                    res.Path = path;
                    res.Load(_data);
                    _resources.Add(path, res);
                }
            }
        }

        private void AddResourceDependent(Path path)
        {
            // TODO: Some kind of counter for how many people are using this resource.
        }

        private void RemoveResourceDependent(Path path)
        {
            // TODO: Some kind of counter for how many people are using this resource.
            // Consider "unloading" a resource if its usages go to zero.
            // Or, maybe this should be kept as an assertion (ex. don't unload if somebody else is using the resource)
        }

        #endregion

    }
}
