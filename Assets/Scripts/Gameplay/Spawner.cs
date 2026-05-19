using System.Collections;
using UnityEngine;

namespace SamuraiSlice
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject ingredientPrefab;
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private float spawnInterval = 1.0f;
        [SerializeField] private float bombChance = 0.5f;

        [Header("Ingredient Velocity")]
        [SerializeField] private float minXVel = -3.0f;
        [SerializeField] private float maxXVel = 3.0f;
        [SerializeField] private float minYVel = 9.0f;
        [SerializeField] private float maxYVel = 12.0f;

        [SerializeField] private Transform[] spawnPoints;

        private GameObject _spawned;

        private void OnEnable()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (enabled)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                if(Random.value < bombChance)
                {
                    _spawned = Instantiate(ingredientPrefab, spawnPoint.position, Quaternion.identity);
                } else
                {
                    _spawned = Instantiate(bombPrefab, spawnPoint.position, Quaternion.identity);
                }
                _spawned.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Random.Range(minXVel, maxXVel), Random.Range(minYVel, maxYVel));
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
