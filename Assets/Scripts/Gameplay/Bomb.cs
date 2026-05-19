using System;
using UnityEngine;

namespace SamuraiSlice
{
    public class Bomb : MonoBehaviour
    {
        public bool IsSliced { get; private set; }
        public event Action<Bomb> Sliced;
        public static event Action OnBombSliced;

        public void Slice(Vector2 swipeDirection)
        {
            if (IsSliced)
            {
                return;
            }

            IsSliced = true;
            Sliced?.Invoke(this);
            OnBombSliced?.Invoke();
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("MissZone"))
            {
                return;
            }

            if (IsSliced)
            {
                Destroy(gameObject);
                return;
            }

            //Ensure that MissZone does not trigger if the ingredient has just spawned
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.y >= 0f)
            {
                return;
            }

            Destroy(gameObject);
        }
    }
}
