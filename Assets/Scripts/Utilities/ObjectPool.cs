using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame
{
    /// <summary>
    /// Generic object pool implementation for Unity components.
    /// Reduces garbage collection by reusing objects instead of instantiating new ones.
    /// </summary>
    /// <typeparam name="T">The type of Component to pool</typeparam>
    public class ObjectPool<T> where T : Component
    {
        private readonly Stack<T> _pool = new Stack<T>();
        private readonly T _prefab;
        private readonly Transform _root;

        /// <summary>
        /// Creates a new object pool
        /// </summary>
        /// <param name="prefab">The prefab to instantiate when the pool is empty</param>
        /// <param name="root">The parent transform for pooled objects</param>
        /// <param name="prewarm">Number of objects to pre-instantiate (default: 0)</param>
        public ObjectPool(T prefab, Transform root, int prewarm = 0)
        {
            _prefab = prefab; _root = root;
            // Lazy by default. Prewarm only if explicitly requested.
            for (int i = 0; i < prewarm; i++)
            {
                var c = GameObject.Instantiate(_prefab, _root);
                c.gameObject.SetActive(false);
                _pool.Push(c);
            }
        }

        /// <summary>
        /// Gets an object from the pool or creates a new one if the pool is empty
        /// </summary>
        /// <returns>A component instance</returns>
        public T Get()
        {
            if (_pool.Count > 0)
            {
                var c = _pool.Pop();
                c.gameObject.SetActive(true);
                return c;
            }
            return GameObject.Instantiate(_prefab, _root);
        }

        /// <summary>
        /// Returns an object to the pool for reuse
        /// </summary>
        /// <param name="obj">The object to return to the pool</param>
        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_root, false);
            _pool.Push(obj);
        }
    }
}