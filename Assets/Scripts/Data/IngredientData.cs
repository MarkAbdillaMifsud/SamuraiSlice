using UnityEngine;

[CreateAssetMenu(menuName = "Samurai Slice/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public string ingredientName;
    public Sprite wholeSprite;
    public Sprite halfASprite;
    public Sprite halfBSprite;
    public int points;
    public float spawnWeight;
    public bool isHazard;
    public bool flashOnSpawn;

    private void OnValidate()
    {
        if(!isHazard && (halfASprite == null || halfBSprite == null)) {
            Debug.LogWarning($"[IngredientData] {name}: missing half sprites.", this);
        }
    }
}
