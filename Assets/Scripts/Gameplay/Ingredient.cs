using System;
using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Collider2D))]
    public class Ingredient : MonoBehaviour
    {
        public bool IsSliced { get; private set; }
        public event Action<Ingredient> Sliced;

        private void Slice()
        {
            if (IsSliced) {
                return;
            }

            IsSliced = true;
            gameObject.GetComponent<Collider2D>().enabled = false;
            Sliced?.Invoke(this);
            Destroy(gameObject, 0.5f);
        }
    }
}
