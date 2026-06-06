using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSlice
{
    [DisallowMultipleComponent]
    public class SliceDetector : MonoBehaviour
    {
        [SerializeField] private SwipeInput swipeInput;
        [SerializeField] private LayerMask sliceableLayers;
        [SerializeField, Min(0.01f)] private float bladeRadius = 0.18f;

        private readonly HashSet<Ingredient> _slicedThisStroke = new();
        private readonly HashSet<Bomb> _bombsSlicedThisStroke = new();

        private void Reset()
        {
            swipeInput = GetComponent<SwipeInput>();
        }

        private void Awake()
        {
            if(swipeInput == null)
            {
                swipeInput = GetComponent<SwipeInput>();
            }

            if (swipeInput == null)
            {
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if(swipeInput == null)
            {
                return;
            }

            swipeInput.OnPressStarted += HandlePressStarted;
            swipeInput.OnPressReleased += HandlePressReleased;
        }

        private void OnDisable()
        {
            if (swipeInput == null)
            {
                return;
            }

            swipeInput.OnPressStarted -= HandlePressStarted;
            swipeInput.OnPressReleased -= HandlePressReleased;
        }

        private void Update()
        {
            if(swipeInput == null)
            {
                return;
            }

            if(!swipeInput.TryGetCurrentSegment(out Vector2 from, out Vector2 to))
            {
                return;
            }

            Vector2 swipeVector = to - from;
            float swipeDistance = swipeVector.magnitude;

            if (swipeDistance <= 0.001f)
            {
                return;
            }

            Vector2 swipeDirection = swipeVector / swipeDistance;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(from, bladeRadius, swipeDirection, swipeDistance, sliceableLayers);

            if (hits == null || hits.Length == 0) {
                return;
            }

            for(int i = 0; i < hits.Length; i++)
            {
                Collider2D hitCollider = hits[i].collider;

                if (hitCollider == null) 
                {
                    continue;
                }

                ResolveHit(hitCollider, swipeDirection);
            }

        }
    
        private void HandlePressStarted()
        {
            _slicedThisStroke.Clear();
            _bombsSlicedThisStroke.Clear();
        }

        private void HandlePressReleased()
        {
            _slicedThisStroke.Clear();
            _bombsSlicedThisStroke.Clear();
        }

        private void ResolveHit(Collider2D hitCollider, Vector2 swipeDirection)
        {
            if(hitCollider.CompareTag("Bomb"))
            {
                ResolveBombHit(hitCollider, swipeDirection);
                return;
            }

            if(hitCollider.CompareTag("Ingredient"))
            {
                ResolveIngredientHit(hitCollider, swipeDirection);
            }
        }

        private void ResolveIngredientHit(Collider2D hitCollider, Vector2 swipeDirection)
        {
            Ingredient ingredient = hitCollider.GetComponent<Ingredient>();

            if(ingredient == null)
            {
                return;
            }

            if(!_slicedThisStroke.Add(ingredient))
            {
                return;
            }

            ingredient.Slice(swipeDirection);
        }

        private void ResolveBombHit(Collider2D hitCollider, Vector2 swipeDirection)
        {
            Bomb bomb = hitCollider.GetComponent<Bomb>();

            if (bomb == null)
            {
                return;
            }

            if (!_bombsSlicedThisStroke.Add(bomb))
            {
                return;
            }

            bomb.Slice(swipeDirection);
        }
    }
}
