using UnityEngine;
using UnityEngine.Pool;

namespace SamuraiSlice
{
    public class SliceParticlePool : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SliceParticles prefab;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 20;

        private IObjectPool<SliceParticles> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<SliceParticles>(
                createFunc: CreateInstance,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroyInstance,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private void OnEnable()
        {
            if (scoreManager != null)
                scoreManager.OnSliceScored += HandleSliceScored;
        }

        private void OnDisable()
        {
            if (scoreManager != null)
                scoreManager.OnSliceScored -= HandleSliceScored;
        }

        private void HandleSliceScored(int finalPoints, int multiplier,
                                       Vector3 worldPos, Color accentColour)
        {
            SliceParticles burst = _pool.Get();
            burst.transform.position = worldPos;
            burst.Play(accentColour, ReturnToPool);
        }

        private void ReturnToPool(SliceParticles burst) => _pool.Release(burst);

        private SliceParticles CreateInstance()
        {
            SliceParticles instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private static void OnGet(SliceParticles instance) => instance.gameObject.SetActive(true);
        private static void OnRelease(SliceParticles instance) => instance.gameObject.SetActive(false);
        private static void OnDestroyInstance(SliceParticles i) => Destroy(i.gameObject);
    }
}