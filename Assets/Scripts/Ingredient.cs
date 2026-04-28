using System.Collections;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private float lifetime = 3.0f;

    private void Start()
    {
        StartCoroutine(DestroyIngredient());
    }

    private IEnumerator DestroyIngredient()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
