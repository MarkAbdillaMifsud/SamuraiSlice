using System.Collections;
using UnityEngine;

namespace SamuraiSlice
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject ingredientPrefab;
        [SerializeField] private GameObject bombPrefab;
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
                GameObject prefab = Random.value < bombChance ? bombPrefab : ingredientPrefab;

                GameObject spawned = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                spawned.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Random.Range(minXVel, maxXVel), Random.Range(minYVel, maxYVel));

                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}