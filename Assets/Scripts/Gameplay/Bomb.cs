using System;
using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bomb : MonoBehaviour
    {
        public bool IsSliced { get; private set; }
        public static event Action<Bomb> Sliced;

        private IngredientPool _pool;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Configure(IngredientPool pool)
        {
            _pool = pool;
        }

        public void Launch(Vector3 position, Vector2 velocity)
        {
            transform.SetPositionAndRotation(position, Quaternion.identity);
            IsSliced = false;
            gameObject.SetActive(true);
            _rb.linearVelocity = velocity;
            _rb.angularVelocity = 0f;
        }

        public void Slice(Vector2 swipeDirection)
        {
            if (IsSliced)
            {
                return;
            }

            IsSliced = true;
            Sliced?.Invoke(this);
            ReturnToPool();
        }

        public void ForceRelease() => ReturnToPool();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("MissZone"))
            {
                return;
            }

            if (IsSliced)
            {
                ReturnToPool();
                return;
            }

            if (_rb != null && _rb.linearVelocity.y >= 0f)
            {
                return;
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Bombs.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}