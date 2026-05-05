using UnityEngine;

namespace MarioGame.Utils.Pooling
{
    public sealed class PooledObject : MonoBehaviour
    {
        public ObjectPool Pool { get; set; }

        public void Despawn()
        {
            if (Pool != null)
                Pool.Return(gameObject);
            else
                Destroy(gameObject);
        }
    }
}

