using System.Collections;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private float bombSpawnChance = 0.1f;
    [SerializeField] private float horizontalJitter = 2f;
    [SerializeField] private Vector2 upwardForceRange = new Vector2(8f, 12f);
    [SerializeField] private float sidewaysForceMax = 3f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnIngredient), 1f, spawnInterval);
    }

    private void SpawnIngredient()
    {
        GameObject ingredientToSpawn = Random.value < bombSpawnChance ? bombPrefab : ingredientPrefab;
        Launch(ingredientToSpawn);
    }

    private void Launch(GameObject prefab)
    {
        /* 
        Spawns an ingredient at a random position based on the jitter then applies a force to create a curve effect
        */
        Vector3 pos = transform.position + Vector3.right * Random.Range(-horizontalJitter, horizontalJitter);
        GameObject ingredient = Instantiate(prefab, pos, Quaternion.identity);
        Rigidbody2D rb = ingredient.GetComponent<Rigidbody2D>();
        Vector2 force = new Vector2(Random.Range(-sidewaysForceMax, sidewaysForceMax), Random.Range(upwardForceRange.x, upwardForceRange.y));
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
