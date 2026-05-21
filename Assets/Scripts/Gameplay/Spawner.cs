using System.Collections;
using UnityEngine;

namespace SamuraiSlice
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private IngredientPool pool;
        [SerializeField] private float spawnInterval = 1.0f;
        [SerializeField, Range(0f, 1f)] private float bombChance = 0.05f;

        [Header("Ingredient Velocity")]
        [SerializeField] private float minXVel = -3.0f;
        [SerializeField] private float maxXVel = 3.0f;
        [SerializeField] private float minYVel = 9.0f;
        [SerializeField] private float maxYVel = 12.0f;

        [SerializeField] private Transform[] spawnPoints;

        private bool _isSpawning;
        private Coroutine _spawnRoutine;
        private WaitForSeconds _wait;

        private void OnEnable()
        {
            StartSpawning();
        }

        private void OnDisable()
        {
            StopSpawning();
        }

        public void StartSpawning()
        {
            if (_isSpawning)
            {
                return;
            }
            _isSpawning = true;
            _wait = new WaitForSeconds(spawnInterval);
            _spawnRoutine = StartCoroutine(Spawn());
        }

        public void StopSpawning()
        {
            _isSpawning = false;
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }
        }

        private IEnumerator Spawn()
        {
            while (_isSpawning)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Vector2 velocity = new(Random.Range(minXVel, maxXVel), Random.Range(minYVel, maxYVel));

                if (Random.value < bombChance)
                {
                    var bomb = pool.Bombs.Get();
                    bomb.Launch(spawnPoint.position, velocity);
                } else
                {
                    var ingredient = pool.Ingredients.Get();
                    ingredient.Launch(spawnPoint.position, velocity);
                }

                yield return _wait;
            }
        }
    }
}