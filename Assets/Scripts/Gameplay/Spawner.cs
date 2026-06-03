using System.Collections;
using UnityEngine;

namespace SamuraiSlice
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private SpawnCurve spawnCurve;
        [SerializeField] private IngredientPool pool;
        [SerializeField] private IngredientData[] catalog;
        [SerializeField] private float slotDelay = 0.05f;

        [Header("Ingredient Velocity")]
        [SerializeField] private float minXVel = -3.0f;
        [SerializeField] private float maxXVel = 3.0f;
        [SerializeField] private float minYVel = 9.0f;
        [SerializeField] private float maxYVel = 12.0f;

        [SerializeField] private Transform[] spawnPoints;

        private bool _isSpawning;
        private Coroutine _spawnRoutine;
        private float _runStartTime;

        public void StartSpawning()
        {
            if (_isSpawning)
            {
                return;
            }

            if (spawnCurve == null)
            {
                Debug.LogError("[Spawner] SpawnCurve reference missing.", this);
                return;
            }

            if(pool == null)
            {
                Debug.LogError("[Spawner] Pool reference missing.", this);
                return;
            }

            if(spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("[Spawner] spawnPoints not assigned or empty.", this);
                return;
            }

            if (catalog == null || catalog.Length == 0)
            {
                Debug.LogError("[Spawner] catalog empty.", this); 
                return;
            }

            if (spawnCurve.ingredientWeights == null || spawnCurve.ingredientWeights.Length != catalog.Length)
            {
                Debug.LogError("[Spawner] SpawnCurve ingredientWeights must match catalog length.", this);
                return;
            }

            _isSpawning = true;
            _runStartTime = Time.time;
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
                float elapsedRunTime = Time.time - _runStartTime;
                Debug.Log($"[Spawner] t={elapsedRunTime:0.0}, interval={spawnCurve.GetSpawnInterval(elapsedRunTime):0.00}, bomb={spawnCurve.GetBombProbability(elapsedRunTime):0.00}");

                Vector2 groupSizeRange = spawnCurve.GetGroupSizeRange(elapsedRunTime);
                int minGroupSize = Mathf.RoundToInt(groupSizeRange.x);
                int maxGroupSize = Mathf.RoundToInt(groupSizeRange.y);

                minGroupSize = Mathf.Max(1, minGroupSize);
                maxGroupSize = Mathf.Max(minGroupSize, maxGroupSize);

                int groupSize = Random.Range(minGroupSize, maxGroupSize + 1);

                for (int i = 0; i < groupSize; i++)
                {
                    SpawnSlot(elapsedRunTime);

                    if (i < groupSize - 1)
                    {
                        yield return new WaitForSeconds(slotDelay);
                    }
                }

                float interval = spawnCurve.GetSpawnInterval(elapsedRunTime);
                yield return new WaitForSeconds(interval);
            }
        }

       private void SpawnSlot(float elapsedRunTime)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector2 velocity = new(Random.Range(minXVel, maxXVel), Random.Range(minYVel, maxYVel));
            float bombProbability = spawnCurve.GetBombProbability(elapsedRunTime);

            if(Random.value < bombProbability)
            {
                var bomb = pool.Bombs.Get();
                bomb.Launch(spawnPoint.position, velocity);
                return;
            }

            IngredientData data = PickWeighted(elapsedRunTime);

            if(data ==  null)
            {
                return;
            }

            var ingredient = pool.Ingredients.Get();
            ingredient.Launch(spawnPoint.position, velocity, data);
        }

        private IngredientData PickWeighted(float elapsedRunTime)
        {
            float totalWeight = 0f;

            for (int i = 0; i < catalog.Length; i++)
            {
                IngredientData data = catalog[i];

                if(data == null || data.isHazard)
                {
                    continue;
                }

                totalWeight += spawnCurve.GetIngredientWeight(i, elapsedRunTime);
            }

            if(totalWeight <= 0f)
            {
                return null;
            }

            float roll = Random.value * totalWeight;
            float cumulative = 0f;

            for (int i = 0; i < catalog.Length; i++)
            {
                IngredientData data = catalog[i];

                if (data == null || data.isHazard)
                {
                    continue;
                }

                cumulative += spawnCurve.GetIngredientWeight(i, elapsedRunTime);

                if (roll <= cumulative)
                {
                    return data;
                }
            }

            for (int i = catalog.Length - 1; i >= 0; i--)
            {
                if (catalog[i] != null && !catalog[i].isHazard)
                {
                    return catalog[i];
                }
            }

            return null;
        }
    }
}