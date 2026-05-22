using UnityEngine;
using UnityEngine.Pool;

namespace SamuraiSlice
{
    public class IngredientPool : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private Ingredient ingredientPrefab;
        [SerializeField] private IngredientHalf halfPrefab;
        [SerializeField] private Bomb bombPrefab;

        [Header("Pool Sizes")]
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxSize = 50;

        private ObjectPool<Ingredient> _ingredients;
        private ObjectPool<IngredientHalf> _halves;
        private ObjectPool<Bomb> _bombs;
        private bool _initialized;

        public ObjectPool<Ingredient> Ingredients { get { EnsureInitialized(); return _ingredients; } }
        public ObjectPool<IngredientHalf> Halves { get { EnsureInitialized(); return _halves; } }
        public ObjectPool<Bomb> Bombs { get { EnsureInitialized(); return _bombs; } }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if(_initialized)
            {
                return;
            }
            _initialized = true;

            if (ingredientPrefab == null) Debug.LogError("[IngredientPool] ingredientPrefab not assigned.", this);
            if (halfPrefab == null) Debug.LogError("[IngredientPool] halfPrefab not assigned.", this);
            if (bombPrefab == null) Debug.LogError("[IngredientPool] bombPrefab not assigned.", this);


            _ingredients = new ObjectPool<Ingredient>(
                            createFunc: CreateIngredient,
                            actionOnGet: null,
                            actionOnRelease: i => i.gameObject.SetActive(false),
                            actionOnDestroy: i => Destroy(i.gameObject),
                            collectionCheck: false,
                            defaultCapacity: defaultCapacity,
                            maxSize: maxSize);

            _halves = new ObjectPool<IngredientHalf>(
                createFunc: CreateHalf,
                actionOnGet: null,
                actionOnRelease: h => h.gameObject.SetActive(false),
                actionOnDestroy: h => Destroy(h.gameObject),
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize);

            _bombs = new ObjectPool<Bomb>(
                createFunc: CreateBomb,
                actionOnGet: null,
                actionOnRelease: b => b.gameObject.SetActive(false),
                actionOnDestroy: b => Destroy(b.gameObject),
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize);
        }

        private Ingredient CreateIngredient()
        {
            var ing = Instantiate(ingredientPrefab, transform);
            ing.Configure(this);
            ing.gameObject.SetActive(false);
            return ing;
        }

        private IngredientHalf CreateHalf()
        {
            var half = Instantiate(halfPrefab, transform);
            half.Configure(this);
            half.gameObject.SetActive(false);
            return half;
        }

        private Bomb CreateBomb()
        {
            var bomb = Instantiate(bombPrefab, transform);
            bomb.Configure(this);
            bomb.gameObject.SetActive(false);
            return bomb;
        }
    }
}
