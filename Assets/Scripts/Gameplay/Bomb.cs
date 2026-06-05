using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bomb : MonoBehaviour
    {
        public bool IsSliced { get; private set; }
        public static event Action<Bomb> Sliced;
        public static event Action DetonationStarted;

        [Header("Detonation FX")]
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private GameObject bombParticlePrefab;
        [SerializeField] private RedVignettePulse redVignette;

        [Header("Field-clear timing")]
        [SerializeField] private float freezeSeconds = 0.2f;
        [SerializeField] private float postFreezeGravityScale = 3f;
        [SerializeField] private float dropClearSeconds = 0.5f;

        private IngredientPool _pool;
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private Vector3 _baseScale;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
            _baseScale = transform.localScale;

            if(impulseSource == null)
            {
                impulseSource = GetComponent<CinemachineImpulseSource>();
            }
            if(redVignette == null)
            {
                redVignette = FindAnyObjectByType<RedVignettePulse>();
            }
        }

        private void Update()
        {
            if(IsSliced)
            {
                return;
            }
            transform.localScale = _baseScale * (1f + 0.05f * Mathf.Sin(Time.time * 4f));
        }

        public void Configure(IngredientPool pool)
        {
            _pool = pool;
        }

        public void Launch(Vector3 position, Vector2 velocity)
        {
            transform.SetPositionAndRotation(position, Quaternion.identity);
            IsSliced = false;
            gameObject.SetActive(true);
            transform.localScale = _baseScale;
            _rb.simulated = true;
            _rb.gravityScale = 1f;
            _rb.linearVelocity = velocity;
            _rb.angularVelocity = 0f;
            if(_sr != null)
            {
                _sr.enabled = true;
            }
        }

        public void Slice(Vector2 swipeDirection)
        {
            if (IsSliced)
            {
                return;
            }

            IsSliced = true;
            StartCoroutine(DetonationSequence());
        }

        public void ForceRelease() => ReturnToPool();

        private IEnumerator DetonationSequence()
        {
            DetonationStarted?.Invoke();
            _rb.simulated = false;

            List<Rigidbody2D> frozen = new List<Rigidbody2D>(_pool.ActiveIngredients.Count);
            foreach (Ingredient ingredient in _pool.ActiveIngredients)
            {
                var rb = ingredient.GetComponent<Rigidbody2D>();
                if(rb == null || !rb.simulated)
                {
                    continue;
                }
                rb.simulated = false;
                frozen.Add(rb);
            }

            yield return new WaitForSecondsRealtime(freezeSeconds);

            foreach(Rigidbody2D rb in frozen)
            {
                if(rb == null)
                {
                    continue;
                }
                rb.simulated = true;
                rb.gravityScale = postFreezeGravityScale;
            }

            if(impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }

            if(bombParticlePrefab != null)
            {
                Instantiate(bombParticlePrefab, transform.position, Quaternion.identity);
            }

            if(redVignette != null)
            {
                redVignette.Pulse();
            }

            if(_sr != null)
            {
                _sr.enabled = false;
            }

            yield return new WaitForSecondsRealtime(dropClearSeconds);

            Sliced?.Invoke(this);
            ReturnToPool();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("MissZone"))
            {
                return;
            }

            if (IsSliced)
            {
                return;
            }

            if (_rb != null && _rb.linearVelocity.y >= 0f)
            {
                return;
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Bombs.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}