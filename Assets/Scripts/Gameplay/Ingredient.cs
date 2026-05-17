using System;
using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Collider2D))]
    public class Ingredient : MonoBehaviour
    {
        public bool IsSliced { get; private set; }
        public event Action<Ingredient> Sliced;

        public void Slice()
        {
            if (IsSliced) {
                return;
            }

            IsSliced = true;
            Debug.Log($"Sliced {name}!");
            gameObject.GetComponent<Collider2D>().enabled = false;
            Sliced?.Invoke(this);
            Destroy(gameObject, 0.5f);
        }
    }
}
