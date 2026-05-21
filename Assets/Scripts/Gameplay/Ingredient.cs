using System;
using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ingredient : MonoBehaviour
    {
        [Header("Ingredient Halves")]
        [SerializeField] private float halfSpeed = 2.0f;
        [SerializeField] private float halfSpin = 200.0f;

        public bool IsSliced { get; private set; }
        public static event Action<Ingredient> Sliced;
        public static event Action OnIngredientMissed;

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
            if (IsSliced) {
                return;
            }

            IsSliced = true;
            Sliced?.Invoke(this);
            SpawnHalves(swipeDirection);
            ReturnToPool();
        }

        public void ForceRelease()
        {
            ReturnToPool();
        }

        private void SpawnHalves(Vector2 swipeDirection)
        {
            Vector2 dir = swipeDirection.sqrMagnitude > 0.0001f ? swipeDirection.normalized : Vector2.right;
            Vector2 perp = Vector2.Perpendicular(dir);

            SpawnHalf(perp * halfSpeed, halfSpin, flipX: false);
            SpawnHalf(-perp * halfSpeed, -halfSpin, flipX: true);
        }

        private void SpawnHalf(Vector2 velocity, float spin, bool flipX)
        {
            if(_pool == null)
            {
                return;
            }
            var half = _pool.Halves.Get();
            half.Launch(transform.position, transform.rotation, velocity, spin, flipX);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!collision.CompareTag("MissZone"))
            {
                return;
            }

            if(IsSliced)
            {
                ReturnToPool();
                return;
            }

            //Ensure that MissZone does not trigger if the ingredient has just spawned
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.y >= 0f)
            {
                return;
            }

            OnIngredientMissed?.Invoke();
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if(_pool != null)
            {
                _pool.Ingredients.Release(this);
            } else
            {
                Destroy(gameObject);
            }
        }
    }
}