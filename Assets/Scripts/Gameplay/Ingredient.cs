using System;
using System.Collections;
using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Ingredient : MonoBehaviour
    {
        [Header("Ingredient Halves")]
        [SerializeField] private float halfSpeed = 2.0f;
        [SerializeField] private float halfSpin = 200.0f;

        [Header("Spawn Flash")]
        [SerializeField] private float flashDuration = 0.2f;

        public bool IsSliced { get; private set; }
        public IngredientData Data => _data;

        public static event Action<Ingredient> Sliced;
        public static event Action OnIngredientMissed;

        private IngredientPool _pool;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private IngredientData _data;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
        }

        public void Configure(IngredientPool pool)
        {
            _pool = pool;
        }

        public void Launch(Vector3 position, Vector2 velocity, IngredientData data)
        {
            _data = data;
            _sr.sprite = data.wholeSprite;
            _sr.color = Color.white;
            gameObject.name = $"Ingredient_{data.ingredientName}";

            transform.SetPositionAndRotation(position, Quaternion.identity);
            IsSliced = false;
            gameObject.SetActive(true);
            _rb.linearVelocity = velocity;
            _rb.angularVelocity = 0f;

            if (data.flashOnSpawn)
            {
                if (_flashRoutine != null)
                {
                    StopCoroutine(_flashRoutine);
                }
                _flashRoutine = StartCoroutine(SpawnFlash());
            }
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

            Sprite a = _data != null ? _data.halfASprite : null;
            Sprite b = _data != null ? _data.halfBSprite : null;

            SpawnHalf(a, perp * halfSpeed, halfSpin, flipX: false);
            SpawnHalf(b, -perp * halfSpeed, -halfSpin, flipX: true);
        }

        private void SpawnHalf(Sprite sprite, Vector2 velocity, float spin, bool flipX)
        {
            if(_pool == null)
            {
                return;
            }
            var half = _pool.Halves.Get();
            half.Launch(transform.position, transform.rotation, velocity, spin, flipX, sprite);
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

        private IEnumerator SpawnFlash()
        {
            float t = 0f;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(1f - t / flashDuration);
                _sr.color = Color.Lerp(Color.white, new Color(2f, 2f, 2f, 1f), k);
                yield return null;
            }
            _sr.color = Color.white;
            _flashRoutine = null;
        }
    }
}