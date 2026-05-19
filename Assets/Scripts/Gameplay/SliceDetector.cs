using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSlice
{
    [DisallowMultipleComponent]
    public class SliceDetector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SwipeTracker swipeTracker;

        [Header("Filtering")]
        [SerializeField] private LayerMask sliceableLayers;

        private readonly HashSet<Ingredient> _slicedThisFrame = new HashSet<Ingredient>();

        private void Reset()
        {
            swipeTracker = GetComponent<SwipeTracker>();
        }

        private void Awake()
        {
            if(swipeTracker == null)
            {
                swipeTracker = GetComponent<SwipeTracker>();
            }

            if (swipeTracker == null)
            {
                Debug.LogError("[SliceDetector] No SwipeTracker reference assigned — disabling.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            _slicedThisFrame.Clear();

            IReadOnlyList<Vector2> path = swipeTracker.CurrentPath;
            if(path == null || path.Count < 2)
            {
                return;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector2 a = path[i];
                Vector2 b = path[i + 1];

                RaycastHit2D hit = Physics2D.Linecast(a, b, sliceableLayers);
                Vector2 swipeDirection = b - a;

                if (hit.collider == null)
                {
                    continue;
                }

                if(hit.collider.CompareTag("Bomb"))
                {
                    Bomb _bomb = hit.collider.GetComponent<Bomb>();
                    if(_bomb != null )
                    {
                        _bomb.Slice(swipeDirection);
                    }
                    continue;
                }

                if(!hit.collider.CompareTag("Ingredient"))
                {
                    continue;
                }

                Ingredient ingredient = hit.collider.GetComponent<Ingredient>();

                if(ingredient == null)
                {
                    continue;
                }

                if(!_slicedThisFrame.Add(ingredient))
                {
                    continue;
                }

                ingredient.Slice(swipeDirection);
            }
        }
    }
}
