using UnityEngine;
using UnityEngine.Pool;

namespace SamuraiSlice
{
    public class ScorePopupPool : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScorePopup popupPrefab;
        [SerializeField] private ScoreManager scoreManager;

        [Header("Pool settings")]
        [SerializeField, Min(1)] private int defaultCapacity = 10;
        [SerializeField, Min(1)] private int maxSize = 20;

        private ObjectPool<ScorePopup> _pool;

        private void Awake()
        {
            if (popupPrefab == null)
            {
                Debug.LogError("[ScorePopupPool] popupPrefab not assigned.", this);
                enabled = false;
                return;
            }

            _pool = new ObjectPool<ScorePopup>(
                createFunc: CreatePopup,
                actionOnGet: null,
                actionOnRelease: p => p.gameObject.SetActive(false),
                actionOnDestroy: p => Destroy(p.gameObject),
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize);
        }

        private void OnEnable()
        {
            if (scoreManager != null)
            {
                scoreManager.OnSliceScored += HandleSliceScored;
            }
        }

        private void OnDisable()
        {
            if (scoreManager != null)
            {
                scoreManager.OnSliceScored -= HandleSliceScored;
            }
        }

        private void HandleSliceScored(int finalPoints, int multiplier, Vector3 worldPos, Color accentColour)
        {
            ScorePopup popup = _pool.Get();
            popup.Init(finalPoints, multiplier, worldPos, accentColour);
        }

        private ScorePopup CreatePopup()
        {
            ScorePopup p = Instantiate(popupPrefab, transform);
            p.SetPool(_pool);
            p.gameObject.SetActive(false);
            return p;
        }
    }
}