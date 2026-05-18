using System;
using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Collider2D))]
    public class Ingredient : MonoBehaviour
    {
        [Header("Ingredient Halves")]
        [SerializeField] private GameObject halfIngredient;
        [SerializeField] private float halfSpeed = 2.0f;
        [SerializeField] private float halfSpin = 200.0f;
        [SerializeField] private float halfLifetime = 1.5f;

        public bool IsSliced { get; private set; }
        public event Action<Ingredient> Sliced;

        public void Slice(Vector2 swipeDirection)
        {
            if (IsSliced) {
                return;
            }

            IsSliced = true;
            Sliced?.Invoke(this);
            SpawnHalves(swipeDirection);
            Destroy(gameObject);
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
            GameObject half = Instantiate(halfIngredient, transform.position, transform.rotation);

            if(flipX)
            {
                Vector3 s = half.transform.localScale;
                half.transform.localScale = new Vector3(-s.x, s.y, s.z);
            }

            if(half.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = velocity;
                rb.angularVelocity = spin;
            }

            Destroy(half, halfLifetime);
        }
    }
}