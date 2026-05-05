using System.Collections.Generic;
using UnityEngine;

namespace MarioGame.Utils.Pooling
{
    public sealed class ObjectPool
    {
        private readonly Stack<GameObject> _inactive = new();
        private readonly Transform _root;
        private readonly System.Func<GameObject> _create;

        public ObjectPool(string name, System.Func<GameObject> create, Transform parent = null, int prewarm = 0)
        {
            _create = create;
            _root = new GameObject(name + "_Pool").transform;
            if (parent != null) _root.SetParent(parent, false);
            for (var i = 0; i < prewarm; i++)
            {
                var go = _create();
                Return(go);
            }
        }

        public GameObject Get(Vector3 position)
        {
            var go = _inactive.Count > 0 ? _inactive.Pop() : _create();
            go.transform.SetParent(null, true);
            go.transform.position = position;
            go.SetActive(true);
            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            for (var i = 0; i < poolables.Length; i++)
                poolables[i].OnSpawned();
            return go;
        }

        public void Return(GameObject go)
        {
            if (go == null) return;
            var poolables = go.GetComponentsInChildren<IPoolable>(true);
            for (var i = 0; i < poolables.Length; i++)
                poolables[i].OnDespawned();
            go.SetActive(false);
            go.transform.SetParent(_root, false);
            _inactive.Push(go);
        }
    }
}
